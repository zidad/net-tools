using System.Threading.Tasks;
using Net.EasyNetQ.Persistence;

namespace Net.EasyNetQ
{
    public class CorrelatedStateHandler<TState, TCorrelationId> : ICorrelatedStateHandler
          where TState : class, ICorrelateBy<TCorrelationId>, new()
    {
        private readonly IRepository<TCorrelationId, TState> repository;

        public CorrelatedStateHandler(IRepository<TCorrelationId, TState> repository)
        {
            this.repository = repository;
        }

        public void LoadState(object consumer, object message)
        {
            Consumer(consumer).State = repository.GetOrNew(CorrelationId(message));
        }

        public void SaveState(object consumer, object message)
        {
            repository.Set(Consumer(consumer).State);
        }

        public async Task LoadStateAsync(object consumer, object message)
        {
            Consumer(consumer).State = await repository.GetOrNewAsync(CorrelationId(message));
        }

        public async Task SaveStateAsync(object consumer, object message)
        {
            await repository.SetAsync(Consumer(consumer).State);
        }

        private static TCorrelationId CorrelationId(object message)
        {
            return ((ICorrelateBy<TCorrelationId>)message).Id;
        }

        private static ISaga<TState> Consumer(object consumer)
        {
            return ((ISaga<TState>)consumer);
        }
    }
}