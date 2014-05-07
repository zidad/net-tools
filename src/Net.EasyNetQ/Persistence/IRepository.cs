using System.Linq;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public interface IRepository<TKey, TState>
        where TState : ICorrelateBy<TKey>, new()
    {
        IQueryable<TState> GetAll();
        TState Get(TKey key);
        TKey Set(TState state);
        TState Remove(TKey key);

        Task<IQueryable<TState>> GetAllAsync();
        Task<TState> GetAsync(TKey key);
        Task<TKey> SetAsync(TState state);
        Task<TState> RemoveAsync(TKey key);
    }
}
