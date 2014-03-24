using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ.AutoSubscribe;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILifetimeScope component;

        public AutofacMessageDispatcher(ILifetimeScope component)
        {
            this.component = component;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            using (var scope = component.BeginLifetimeScope("message"))
            {
                var consumer = scope.Resolve<TConsumer>();
                var hooks = scope.Resolve<IEnumerable<IMessageDispatcherHook>>().ToArray();
                foreach (var hook in hooks) 
                    hook.OnBeforeConsume<TMessage, TConsumer>(consumer, message);
                consumer.Consume(message);
                foreach (var hook in hooks.Reverse()) 
                    hook.OnAfterConsume<TMessage, TConsumer>(consumer, message);
            }
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            using (var scope = component.BeginLifetimeScope("async-message"))
            {
                var consumer = scope.Resolve<TConsumer>();
                var hooks = scope.Resolve<IEnumerable<IMessageDispatcherHook>>().ToArray();
                foreach (var hook in hooks) 
                    await hook.OnBeforeConsumeAsync<TMessage, TConsumer>(consumer, message);
                await consumer.Consume(message);
                foreach (var hook in hooks.Reverse()) 
                    await hook.OnAfterConsumeAsync<TMessage, TConsumer>(consumer, message);
            }
        }
    }
}