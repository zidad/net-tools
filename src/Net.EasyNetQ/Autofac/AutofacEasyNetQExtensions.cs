using System;
using Autofac;
using Net.EasyNetQ.ErrorHandling;
using Net.EasyNetQ.KeyGeneration;
using Net.EasyNetQ.Persistence;
using Net.EasyNetQ.Persistence.InMemory;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    public static class AutofacEasyNetQExtensions 
    {
        public static void ConfigureSagas(this ContainerBuilder builder, Action<IAutofacSagaConfigurator> configuration)
        {
            var autofacSagaConfigurator = new AutofacSagaConfigurator();

            configuration(autofacSagaConfigurator);

            autofacSagaConfigurator.Configure(builder);
        }
    }

    public class SagaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterGeneric(typeof(InMemoryRepository<,>)).AsSelf().As(typeof(IRepository<,>)).SingleInstance();
            builder.RegisterGeneric(typeof(CorrelatedStateHandler<,>)).AsSelf();
            builder.RegisterType<SimpleSagaPipe>().AsImplementedInterfaces().Named<IPipe>("global");
            builder.RegisterType<DelegatingErrorHandler>().AsImplementedInterfaces().Named<IErrorHandler>("global");
            builder.RegisterType<IntGenerator>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StringGenerator>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
