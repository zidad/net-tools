﻿using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Owin;
using Net.EasyNetQ.Subscribing;
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
            var conventions = new Conventions(new TypeNameSerializer());
            var subscriber = new AdvancedAutoSubscriber("prefix", bus, conventions)
            {
                SubscriptionConfiguration = (c, s) => c.WithAutoDelete()
            };
        }
    }
}