using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;

namespace Net.EasyNetQ.Persistence.InMemory
{
    public class InMemoryRepository<TKey, TState> : IRepository<TKey, TState>
        where TState : ICorrelateBy<TKey>, new()
    {
        private readonly ConcurrentDictionary<TKey, byte[]> store = new ConcurrentDictionary<TKey, byte[]>();

        private static byte[] Serialize(TState state)
        {
            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, state);
                return memoryStream.GetBuffer();
            }
        }

        private static TState Deserialize(byte[] state)
        {
            using (var memoryStream = new MemoryStream(state))
                return Serializer.Deserialize<TState>(memoryStream);
        }

        public virtual IQueryable<TState> GetAll()
        {
            return store.Values.Select(Deserialize).AsQueryable();
        }

        public virtual TState GetOrNew(TKey key)
        {
            return Deserialize(store.GetOrAdd(key, o => Serialize(new TState())));
        }

        public virtual TKey Set(TState state)
        {
            store.AddOrUpdate(state.Id, o => Serialize(state), (o, k) => Serialize(state));

            return state.Id;
        }

        public virtual TState Remove(TKey key)
        {
            byte[] value;
            store.TryRemove(key, out value);
            return Deserialize(value);
        }

        public virtual Task<IQueryable<TState>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        public virtual Task<TState> GetOrNewAsync(TKey key)
        {
            return Task.FromResult(GetOrNew(key));
        }

        public Task<TState> FindAsync(TKey key)
        {
            return Task.FromResult(Find(key));
        }

        private TState Find(TKey id)
        {
            byte[] value;
            store.TryGetValue(id, out value);
            return Deserialize(value);
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