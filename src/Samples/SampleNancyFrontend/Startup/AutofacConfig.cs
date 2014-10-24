// MIT Licensed from http://github.com/zidad/net-tools
using System;
using Autofac;
using Net.EasyNetQ.Persistence;
using SampleLibrary;
using Serilog;

namespace SampleMessages.App_Start
{
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
}