using System;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Owin;
using Net.Autofac;
using Net.DependencyInjection;
using Net.EasyNetQ;
using Net.EasyNetQ.Autofac;
using Net.EasyNetQ.Subscribing;
using Owin;

[assembly: OwinStartup(typeof(SampleNancyFrontend.SampleStartup))]
[assembly: RegisterAssemblyInContainer]

namespace SampleNancyFrontend
{
    public class SampleStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            app.UseNancy();

            var bus = RabbitHutch.CreateBus("localhost");
            var conventions = new Conventions(new TypeNameSerializer());

            var builder = new ContainerBuilder();
            builder.RegisterModule(new RegistrationModule());
            builder.ConfigureSagas(
                x => x.StoreInMemory()
            );

            var subscriber = new AdvancedAutoSubscriber("nancy-sample", bus, conventions)
            {
                SubscriptionConfiguration = (c, s) => c.WithAutoDelete()
            };
        }
    }
}
