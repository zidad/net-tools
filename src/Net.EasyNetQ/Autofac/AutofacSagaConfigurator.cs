using System;
using System.Collections.Generic;
using Autofac;
using Net.EasyNetQ.Persistence;
using Net.EasyNetQ.Persistence.InMemory;

namespace Net.EasyNetQ.Autofac
{
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
}