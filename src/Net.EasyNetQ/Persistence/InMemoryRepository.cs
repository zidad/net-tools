using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public class InMemoryRepository<TKey, TState> : IRepository<TKey, TState>
        where TState : ICorrelateBy<TKey>, new()
    {
        private readonly ConcurrentDictionary<TKey, TState> _store = new ConcurrentDictionary<TKey, TState>();

        public virtual IQueryable<TState> GetAll()
        {
            return _store.Values.AsQueryable();
        }

        public virtual TState Get(TKey key)
        {
            return _store.GetOrAdd(key, o => new TState());
        }

        public virtual TKey Set(TState state)
        {
            _store.AddOrUpdate(state.Id, o => state, (o, k) => state);

            return state.Id;
        }

        public virtual TState Remove(TKey key)
        {
            TState value;
            _store.TryRemove(key, out value);
            return value;
        }

        public virtual Task<IQueryable<TState>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        public virtual Task<TState> GetAsync(TKey key)
        {
            return Task.FromResult(Get(key));
        }

        public Task<TState> FindAsync(TKey id)
        {
            return Task.FromResult(Find(id));
        }

        private TState Find(TKey id)
        {
            TState value;
            _store.TryGetValue(id, out value);
            return value;
        }

        public virtual Task<TKey> SetAsync(TState state)
        {
            return Task.FromResult(Set(state));
        }

        public virtual Task<TState> RemoveAsync(TKey key)
        {
            return Task.FromResult(Remove(key));
        }
    }
}