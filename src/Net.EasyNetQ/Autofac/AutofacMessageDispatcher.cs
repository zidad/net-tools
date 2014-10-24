using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Net.Collections;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly ILifetimeScope _component;
        public const string PerMessageLifeTimeScopeTag = "AutofacMessageScope";
        public const string GlobalPipeTag = "global";

        public AutofacMessageDispatcher(ILifetimeScope component)
        {
            _component = component;
        }

        private static IEnumerable<IErrorHandler> GetErrorHandlers<TConsumer>(TConsumer consumer, IComponentContext scope)
        {
            var errorPipes = consumer.GetType()
                .GetAttributes<ErrorHandlerAttribute>()
                .OrderBy(attribute => attribute.Order)
                .Select(attribute => attribute.Initialize((IErrorHandler)scope.Resolve(attribute.ErrorHandlerType)))
                .Union(scope.ResolveNamed<IEnumerable<IErrorHandler>>(GlobalPipeTag), a => a.GetType()); // perform the distinction in the union on GetType so we only get 1 handler of the same type

            return errorPipes;
        }

        private static IEnumerable<IPipe> GetPipeLine<TConsumer>(TConsumer consumer, IComponentContext scope)
        {
            var pipeLine = consumer.GetType()
                .GetAttributes<PipeAttribute>()
                .OrderBy(attribute => attribute.Order)
                .Select(attribute => attribute.Initialize((IPipe)scope.Resolve(attribute.PipeType)))
                .Union(scope.ResolveNamed<IEnumerable<IPipe>>(GlobalPipeTag), a => a.GetType()); // perform the distinction in the union on GetType so we only get 1 handler of the same type

            return pipeLine;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            using (var scope = _component.BeginLifetimeScope(PerMessageLifeTimeScopeTag, RegisterMessageContext(message)))
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

        private static Action<ContainerBuilder> RegisterMessageContext<TMessage>(TMessage message) where TMessage : class
        {
            return builder => builder.RegisterInstance(new MessageContext(message)).As<IMessageContext>().AsSelf();
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            using (var scope = _component.BeginLifetimeScope(PerMessageLifeTimeScopeTag, RegisterMessageContext(message)))
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
    }
}