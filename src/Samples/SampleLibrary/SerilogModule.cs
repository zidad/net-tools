using System;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Net.Collections;
using Serilog;
using Serilog.Events;
using Module = Autofac.Module;

namespace SampleLibrary
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
                .MinimumLevel.Information();

            loggerConfig.WriteTo.ColoredConsole(LogEventLevel.Information);
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
}