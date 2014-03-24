using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public interface IRepository<in TKey, TState>
        where TState : ICorrelateBy<TKey>, new()
    {
        TState Get(TKey key);
        void Set(TKey key, TState state);
        void Remove(TKey key);

        Task<TState> GetAsync(TKey key);
        Task SetAsync(TKey key, TState state);
        Task RemoveAsync(TKey key);
    }
}
