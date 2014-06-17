using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ.AutoSubscribe;
using Net.Collections;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private const string MessageLifeTag = "message";
        private readonly ILifetimeScope component;

        public AutofacMessageDispatcher(ILifetimeScope component)
        {
            this.component = component;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            using (var scope = component.BeginLifetimeScope(MessageLifeTag))
            {
                var consumer = scope.Resolve<TConsumer>();
                var pipeLine = GetPipeLine(consumer, scope).ToArray();

                pipeLine.ForEach(p => p.OnBeforeConsume(consumer, message));

                Exception exception = null;
                try
                {
                    consumer.Consume(message);
                }
                catch (Exception e)
                {
                    if (!GetErrorHandlers(consumer, scope).Any(p => p.OnError(consumer, message, e)))
                        throw;

                    exception = e;
                }
                pipeLine.Reverse().ForEach(p => p.OnAfterConsume(consumer, message, exception));
            }
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            using (var scope = component.BeginLifetimeScope(MessageLifeTag))
            {
                var consumer = scope.Resolve<TConsumer>();
                var pipes = GetPipeLine(consumer, scope).ToArray();

                Exception exception = null;

                foreach (var hook in pipes)
                    await hook.OnBeforeConsumeAsync(consumer, message);
                try
                {
                    await consumer.Consume(message);
                }
                catch (Exception e)
                {
                    if (!GetErrorHandlers(consumer, scope).Any(p => p.OnErrorAsync(consumer, message, e)))
                        throw;

                    exception = e;
                }
                foreach (var hook in pipes.Reverse())
                    await hook.OnAfterConsumeAsync(consumer, message, exception);
            }
        }

        private static IEnumerable<IErrorHandler> GetErrorHandlers<TConsumer>(TConsumer consumer, IComponentContext scope)
        {
            return scope
                .Resolve<IEnumerable<IErrorHandlerBuilder>>()
                .SelectMany(e => e.Build(consumer))
                .Distinct(a => a.GetType());
        }

        private static IEnumerable<IPipe> GetPipeLine<TConsumer>(TConsumer consumer, IComponentContext scope)
        {
            return scope
                .Resolve<IEnumerable<IPipeBuilder>>()
                .SelectMany(e => e.Build(consumer))
                .Distinct(a => a.GetType());
        }
    }
}