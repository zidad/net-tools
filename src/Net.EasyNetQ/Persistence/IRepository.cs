using System.Linq;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public interface IRepository<TKey, TState>
        where TState : ICorrelateBy<TKey>, new()
    {
        Task<IQueryable<TState>> GetAllAsync();
        Task<TState> GetAsync(TKey key);
        Task<TState> FindAsync(TKey id);
        Task<TKey> SetAsync(TState state);
        Task<TState> RemoveAsync(TKey key);
    }
}
