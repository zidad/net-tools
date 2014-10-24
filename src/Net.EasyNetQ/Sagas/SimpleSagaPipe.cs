using System;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ.AutoSubscribe;
using Net.Reflection;

namespace Net.EasyNetQ
{
    public class SimpleSagaPipe : IPipe
    {
        private readonly IComponentContext context;
        private ICorrelatedStateHandler handler;

        public SimpleSagaPipe(IComponentContext context)
        {
            this.context = context;
        }

        public void OnBeforeConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            handler.LoadState(consumer, message);
        }

        public void OnAfterConsume<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsume<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            handler.SaveState(consumer, message);
        }

        public async Task OnBeforeConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            await handler.LoadStateAsync(consumer, message);
        }

        public async Task OnAfterConsumeAsync<TMessage, TConsumer>(TConsumer consumer, TMessage message, Exception exception)
            where TMessage : class
            where TConsumer : IConsumeAsync<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            await handler.SaveStateAsync(consumer, message);
        }

        private bool CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>()
        {
            Type sagaType;

            if (!typeof(TConsumer).IsOfGenericType(typeof(ISaga<>), out sagaType))
                return false;

            var stateType = sagaType.GetGenericArguments()[0];

            Type correlationType;

            if (!typeof(TMessage).IsOfGenericType(typeof(ICorrelateBy<>), out correlationType))
                return false;

            var correlationIdType = correlationType.GetGenericArguments()[0];

            handler = (ICorrelatedStateHandler)context.Resolve(typeof(CorrelatedStateHandler<,>).MakeGenericType(stateType, correlationIdType));

            return true;
        }
    }
 
    public class CleanupAfterFinishPipe 
    { 
    
    }
}