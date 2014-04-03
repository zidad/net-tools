using System;
using System.Threading.Tasks;
using Autofac;
using EasyNetQ.AutoSubscribe;
using Net.Reflection;

namespace Net.EasyNetQ.Persistence
{
    public class AutofacStatefulConsumerMessageHook : IMessageHook
    {
        private readonly IComponentContext context;
        private ICorrelatedStateHandler handler;

        public AutofacStatefulConsumerMessageHook(IComponentContext context)
        {
            this.context = context;
        }

        public void OnBeforeConsume<TMessage, TConsumer>(IConsume<TMessage> consumer, TMessage message) where TMessage : class where TConsumer : IConsume<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            handler.LoadState(consumer, message);
        }

        private bool CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>()
        {
            Type stateType;

            if (!typeof(TConsumer).IsOfGenericType(typeof(ISaga<>), out stateType))
                return false;

            Type correlationIdType;

            if (!typeof(TMessage).IsOfGenericType(typeof(ICorrelateBy<>), out correlationIdType))
                return false;

            handler = (ICorrelatedStateHandler) context.Resolve(typeof (CorrelatedStateHandler<,>).MakeGenericType(stateType, correlationIdType));

            return true;
        }

        public void OnAfterConsume<TMessage, TConsumer>(IConsume<TMessage> consumer, TMessage message) where TMessage : class where TConsumer : IConsume<TMessage>
        {
            handler.SaveState(consumer, message);
        }

        public async Task OnBeforeConsumeAsync<TMessage, TConsumer>(IConsumeAsync<TMessage> consumer, TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            if (!CreateStateHandlerIfConsumerRequiresState<TConsumer, TMessage>())
                return;

            await handler.LoadStateAsync(consumer, message);
        }

        public async Task OnAfterConsumeAsync<TMessage, TConsumer>(IConsumeAsync<TMessage> consumer, TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            await handler.SaveStateAsync(consumer, message);
        }

        class CorrelatedStateHandler<TState, TCorrelationId> : ICorrelatedStateHandler 
            where TState : ICorrelateBy<TCorrelationId>, new()
        {
            private readonly IRepository<TCorrelationId, TState> repository;

            public CorrelatedStateHandler(IRepository<TCorrelationId, TState> repository)
            {
                this.repository = repository;
            }

            public void LoadState(object consumer, object message)
            {
                Consumer(consumer).State = repository.Get(CorrelationId(message));
            }

            public void SaveState(object consumer, object message)
            {
                repository.Set(CorrelationId(message), Consumer(consumer).State);
            }

            public async Task LoadStateAsync(object consumer, object message)
            {
                Consumer(consumer).State = await repository.GetAsync(CorrelationId(message));
            }

            public async Task SaveStateAsync(object consumer, object message)
            {
                await repository.SetAsync((TCorrelationId) message, Consumer(consumer).State);
            }

            private static TCorrelationId CorrelationId(object message)
            {
                return ((ICorrelateBy<TCorrelationId>)message).CorrelationId;
            }

            private static ISaga<TState> Consumer(object consumer)
            {
                return ((ISaga<TState>)consumer);
            }
        }

        interface ICorrelatedStateHandler
        {
            void LoadState(object consumer, object message);
            void SaveState(object consumer, object message);
            Task LoadStateAsync(object consumer, object message);
            Task SaveStateAsync(object consumer, object message);
        }
    }
}