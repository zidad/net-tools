using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public class InMemoryRepository<TKey, TState> : IRepository<TKey, TState> 
        where TState : ICorrelateBy<TKey>, new()
    {
        private readonly ConcurrentDictionary<TKey, TState> store = new ConcurrentDictionary<TKey, TState>();

        public TState Get(TKey key)
        {
            return store.GetOrAdd(key, o => new TState());
        }

        public void Set(TKey key, TState state)
        {
            store.AddOrUpdate(key, o => state, (o,k) => state);
        }

        public void Remove(TKey key)
        {
            TState value;
            store.TryRemove(key, out value);
        }

        public Task<TState> GetAsync(TKey key)
        {
            return Task.Run(()=>Get(key));
        }

        public Task SetAsync(TKey key, TState state)
        {
            return Task.Run(() => Set(key, state));
        }

        public Task RemoveAsync(TKey key)
        {
            return Task.Run(() => Remove(key));
        }
    }
}