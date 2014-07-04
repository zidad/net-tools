using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Net.EasyNetQ.Persistence;
using Net.EasyNetQ.Persistence.InMemory;

namespace Net.EasyNetQ.Autofac
{
    public interface IAutofacSagaConfigurator
    {
        IAutofacSagaConfigurator StoreInMemory();
    }

    public class AutofacSagaConfigurator : IAutofacSagaConfigurator
    {
        private readonly List<Action<ContainerBuilder>> actions = new List<Action<ContainerBuilder>>();
 
        public void Configure(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(CorrelatedStateHandler<,>)).AsSelf();
            builder.RegisterType<SimpleSagaPipe>().AsImplementedInterfaces().Named<IPipe>("global");

            actions.ForEach(a => a(builder));
        }

        IAutofacSagaConfigurator IAutofacSagaConfigurator.StoreInMemory()
        {
            actions.Add(builder=>builder.RegisterGeneric(typeof(InMemoryRepository<,>)).As(typeof(IRepository<,>)).SingleInstance());
            return this;
        }
    }

    public static class AutofacEasyNetQExtensions 
    {
        public static void ConfigureSagas(this ContainerBuilder builder, Action<IAutofacSagaConfigurator> configuration)
        {
            var autofacSagaConfigurator = new AutofacSagaConfigurator();

            configuration(autofacSagaConfigurator);

            autofacSagaConfigurator.Configure(builder);
        }
    }
}
