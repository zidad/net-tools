// MIT Licensed from http://github.com/zidad/net-tools
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Core;
using EasyNetQ;
using EasyNetQ.DI;
using Net.Autofac;
using Net.Collections;
using Net.EasyNetQ.Persistence;
using Net.EasyNetQ.Subscribing;
using Serilog;
using Serilog.Events;
using AutofacMessageDispatcher = Net.EasyNetQ.Autofac.AutofacMessageDispatcher;
using IContainer = Autofac.IContainer;
using Module = Autofac.Module;

namespace SampleMessages.App_Start
{

    public class SerilogModule : Module
    {
        private readonly string instanceName;

        public SerilogModule(string instanceName)
        {
            this.instanceName = instanceName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(x => CreateLogger(instanceName))
                .As<ILogger>()
                .SingleInstance();
        }

        private static string GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly != null ? assembly.GetName(false).Version.ToString() : string.Empty;
        }

        private static ILogger CreateLogger(string applicationName)
        {
            var loggerConfig = new LoggerConfiguration()
                 .Enrich.WithThreadId()
                 .Enrich.WithMachineName()
                 .Enrich.WithProcessId()
                 .Enrich.WithProperty("AppName", applicationName)
                 .Enrich.WithProperty("AppVersion", GetVersion())
                 .Enrich.FromLogContext()
                 .MinimumLevel.Debug();

            loggerConfig.WriteTo.ColoredConsole(LogEventLevel.Debug);
            loggerConfig.Enrich.WithProperty("UserName", string.Concat(Environment.UserDomainName, @"\", Environment.UserName));

            return loggerConfig.CreateLogger();
        }

        private static LogEventLevel ParseLevel(string value, LogEventLevel defaultLevel = LogEventLevel.Information)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultLevel;

            LogEventLevel l;
            return Enum.TryParse(value, true, out l) ? l : defaultLevel;
        }

        protected override void AttachToComponentRegistration(
            IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Preparing += Preparing;
        }

        private static void Preparing(object sender, PreparingEventArgs args)
        {
            var forType = args.Component.Activator.LimitType;

            var logParameter = new ResolvedParameter
            (
                ParameterTypePredicate,
                (p, c) => ResolveLoggerForContext(c, forType)
            );

            args.Parameters = args.Parameters.Concat(logParameter);
        }

        private static ILogger ResolveLoggerForContext(IComponentContext c, Type forType)
        {
            var resolve = c.Resolve<ILogger>();
            ILogger resolveLoggerForContext = resolve.ForContext(forType);
            return resolveLoggerForContext;
        }

        private static bool ParameterTypePredicate(ParameterInfo p, IComponentContext c)
        {
            return p.ParameterType == typeof(ILogger);
        }
    }
    public class AutofacConfig
    {
        public static AutofacBusInstance Bus { get; private set; }

        public static ILifetimeScope Initialize(Type applicationType)
        {
            Bus = new AutofacBusInstance();
            Bus.Start
            (bus => bus
                    .Name(applicationType.Namespace)
                    .IsGuiInstance()
                    .IsWebInstance()
                    .ConfigureContainer(Configure)
                    .LoadConsumersFrom(applicationType.Assembly)
            );

            Log.Logger = Bus.Container.Resolve<ILogger>();

            Log.Information("Starting application");

            return Bus.Container;

        }

        private static void Configure(ContainerBuilder builder)
        {
            builder
                .RegisterGeneric(typeof(KeyGeneratingMemoryRepository<>))
                .As(typeof(IRepository<,>))
                .SingleInstance();
        }
    }

    public class AutofacBusInstance : IDisposable
    {
        public IBus Bus { get; private set; }
        public IContainer Container { get; private set; }

        public void Start(Action<IConfigureBus> configuration = null)
        {
            if (configuration == null)
                configuration = bus => { };

            var busInstanceConfiguration = new AutofacBusInstanceConfiguration();

            configuration(busInstanceConfiguration);

            InitializeDependencyInjection(busInstanceConfiguration);

            if (busInstanceConfiguration.BusEnabled)
                InitializeBus(busInstanceConfiguration);
        }

        private void InitializeBus(AutofacBusInstanceConfiguration configuration)
        {
            Bus = Container.Resolve<IBus>();

            var subscriptionIdPrefix = configuration.NamedInstance;

            if (string.IsNullOrWhiteSpace(subscriptionIdPrefix))
                throw new ApplicationException("Named Instance must be specified on AutofacBusInstanceConfiguration");

            if (configuration.IsGuiInstance)
                subscriptionIdPrefix = string.Format("{0}-{1}", Environment.MachineName, subscriptionIdPrefix);

            var factory = Container.Resolve<AdvancedAutoSubscriber.Factory>();
            var subscriber = factory(subscriptionIdPrefix);

            subscriber.AutoSubscriberMessageDispatcher = new AutofacMessageDispatcher(Container);
            subscriber.GenerateSubscriptionId = info => string.Format("{0}:{1}", subscriptionIdPrefix, info.ConcreteType.Name);
            subscriber.SubscriptionConfiguration = (c, s) => c
                .Durable(!configuration.IsGuiInstance)
                .WithAutoDelete(configuration.IsGuiInstance);

            foreach (var consumerAssembly in configuration.ConsumerAssemblies)
            {
                subscriber.Subscribe(consumerAssembly);
                subscriber.SubscribeAsync(consumerAssembly);
            }
        }

        public void Stop()
        {
            if (Bus != null)
                Bus.Dispose();

            if (Container != null)
                Container.Dispose();
        }

        private void InitializeDependencyInjection(AutofacBusInstanceConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            var autofacRegistrationModule = new RegistrationModule();

            if (configuration.ConfigureModule != null)
                configuration.ConfigureModule(autofacRegistrationModule);

            builder.RegisterModule(autofacRegistrationModule);
            builder.RegisterModule(new SerilogModule(configuration.NamedInstance));

            builder.RegisterType<SampleConventions>().As<IConventions>();

            foreach (var action in configuration.RegisterServices)
                action(builder);

            var autofacAdapter = new AutofacAdapter(builder);

            RabbitHutch.SetContainerFactory(() => autofacAdapter);

            Container = autofacAdapter.Container;

            var bus = CreateBus();

            autofacAdapter.Register(provider => bus);

            foreach (var action in configuration.ContainerCreatedActions)
                action(Container);
        }

        private static IBus CreateBus()
        {
            var hostname = ConfigurationManager.AppSettings["Bus.Host"] ?? "localhost";
            var virtualHost = ConfigurationManager.AppSettings["Bus.VirtualHost"] ?? "dev";

            var rabbitConfig = new Dictionary<string, string>
            {
                {"host", hostname},
                {"virtualHost", virtualHost},
                //{"userName", "guest"},
                //{"password", "guest"}
            };

            string connectionString = rabbitConfig.ConcatToString(pair => string.Format("{0}={1}", pair.Key, pair.Value), ";");

            IBus bus = RabbitHutch.CreateBus(connectionString);

            return bus;
        }

        public void Dispose()
        {
            Stop();
        }
    }

    public class AutofacBusInstanceConfiguration : IConfigureBus
    {
        internal IList<Assembly> ConsumerAssemblies { get; private set; }
        internal IList<Action<ContainerBuilder>> RegisterServices { get; private set; }
        internal IList<Action<IContainer>> ContainerCreatedActions { get; private set; }
        internal Action<RegistrationModule> ConfigureModule { get; private set; }
        internal string NamedInstance { get; set; }
        internal bool BusEnabled { get; set; }
        internal bool IsGuiInstance { get; set; }
        internal bool IsWebInstance { get; set; }

        public AutofacBusInstanceConfiguration()
        {
            RegisterServices = new List<Action<ContainerBuilder>>();
            ContainerCreatedActions = new List<Action<IContainer>>();
            ConsumerAssemblies = new List<Assembly>();
            BusEnabled = true;
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                NamedInstance = entryAssembly.GetName().Name;
                ConsumerAssemblies.Add(entryAssembly);
            }
        }

        IConfigureBus IConfigureBus.ContainerCreated(Action<IContainer> container)
        {
            ContainerCreatedActions.Add(container);
            return this;
        }

        IConfigureBus IConfigureBus.ConfigureContainer(Action<ContainerBuilder> builder)
        {
            RegisterServices.Add(builder);
            return this;
        }

        IConfigureBus IConfigureBus.LoadConsumersFrom(params Assembly[] assemblies)
        {
            ConsumerAssemblies.AddRange(assemblies);
            return this;
        }

        IConfigureBus IConfigureBus.Name(string name)
        {
            NamedInstance = name;
            return this;
        }

        IConfigureBus IConfigureBus.DisableBus(bool disable)
        {
            BusEnabled = !disable;
            return this;
        }

        IConfigureBus IConfigureBus.IsGuiInstance(bool isGuiInstance)
        {
            IsGuiInstance = isGuiInstance;
            return this;
        }

        IConfigureBus IConfigureBus.IsWebInstance(bool isWebInstance)
        {
            IsWebInstance = isWebInstance;
            return this;
        }

        IConfigureBus IConfigureBus.ConfigureModule(Action<RegistrationModule> module)
        {
            ConfigureModule = module;
            return this;
        }
    }

    public interface IConfigureBus
    {
        IConfigureBus ContainerCreated(Action<IContainer> container);
        IConfigureBus ConfigureContainer(Action<ContainerBuilder> builder);
        IConfigureBus LoadConsumersFrom(params Assembly[] assemblies);
        IConfigureBus Name(string ns);
        IConfigureBus DisableBus(bool disable = true);
        IConfigureBus IsGuiInstance(bool isGuiInstance = true);
        IConfigureBus IsWebInstance(bool isWebInstance = true);
        IConfigureBus ConfigureModule(Action<RegistrationModule> module);
    }


    public class SampleConventions : Conventions
    {
        public SampleConventions(ITypeNameSerializer typeNameSerializer)
            : base(typeNameSerializer)
        {
            QueueNamingConvention = (messageType, subscriptionId) => string.Format("{1}({0})", messageType.FullName, subscriptionId);
        }
    }

}