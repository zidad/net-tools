using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SampleNancyFrontend.SampleStartup))]

namespace SampleNancyFrontend
{
    public class SampleStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            app.UseNancy();

            var bus = RabbitHutch.CreateBus("localhost");
            var autoSubscriber = new AutoSubscriber(bus, "test");

        }
    }
}
