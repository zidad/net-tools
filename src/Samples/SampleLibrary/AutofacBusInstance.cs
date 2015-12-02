using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.DI;
using Net.Autofac;
using Net.Collections;
using AutofacMessageDispatcher = Net.EasyNetQ.Autofac.AutofacMessageDispatcher;
using IContainer = Autofac.IContainer;

namespace SampleLibrary
{
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

            var subscriber = Container.Resolve<AutoSubscriber>();

            var busLifetimeScope = Container.BeginLifetimeScope("bus", BusSpecificDependencies);

            subscriber.AutoSubscriberMessageDispatcher = new AutofacMessageDispatcher(busLifetimeScope);
            subscriber.GenerateSubscriptionId = info => string.Format("{0}:{1}", subscriptionIdPrefix, info.ConcreteType.Name);
            subscriber.ConfigureSubscriptionConfiguration = (c) => c
                .WithAutoDelete(configuration.IsGuiInstance);

            foreach (var consumerAssembly in configuration.ConsumerAssemblies)
            {
                subscriber.Subscribe(consumerAssembly);
                subscriber.SubscribeAsync(consumerAssembly);
            }

            subscriber.Subscribe(configuration.ConsumerTypes.ToArray());
            subscriber.SubscribeAsync(configuration.ConsumerTypes.ToArray());
        }

        private static void BusSpecificDependencies(ContainerBuilder builder)
        {
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

            builder.RegisterModule(new AssemblyPreloaderModule(a => { }));
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
}
