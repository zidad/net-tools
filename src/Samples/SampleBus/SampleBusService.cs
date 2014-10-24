using Autofac;
using Net.EasyNetQ.Autofac;
using SampleLibrary;
using Serilog.Extras.Topshelf;
using Topshelf;
using Topshelf.Logging;

namespace SampleBus
{
    internal sealed class SampleBusService : ServiceControl
    {
        public AutofacBusInstance BusInstance { get; private set; }
        private IContainer Container { get; set; }

        public SampleBusService()
        {
            BusInstance = new AutofacBusInstance();
        }

        public bool Start(HostControl hostControl)
        {
            BusInstance.Start(bus =>
            {
                bus.ConfigureContainer(builder =>
                {
                    builder.RegisterType<SerilogHostLoggerConfigurator>().AsSelf();
                    // builder.RegisterType<>().As<LogWriterFactory>().AsSelf();
                })
                    .ConfigureContainer(ConfigureContainer)
                    .ContainerCreated(container => Container = container)
                    .ContainerCreated(container => HostLogger.UseLogger(container.Resolve<SerilogHostLoggerConfigurator>()))
                    .ContainerCreated(ContainerCreated);
                Configure(bus);
            });
               
            return true;
        }

        private void ContainerCreated(IContainer container)
        {

        }

        private void Configure(IConfigureBus configuration)
        {
        
        }

        public bool Stop(HostControl hostControl)
        {
            BusInstance.Stop();
            return true;
        }

        private void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterModule(new SagaModule());
            builder.RegisterModule(new ElasticSearchSagaModule());
        }
    }
}