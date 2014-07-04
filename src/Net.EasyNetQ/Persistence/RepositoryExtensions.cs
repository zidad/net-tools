using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Net.EasyNetQ.Persistence
{
    public static class RepositoryExtensions
    {
        public static IQueryable<TState> GetAll<TKey, TState>(this IRepository<TKey, TState> repository)
            where TState : ICorrelateBy<TKey>, new()
        {
            var allAsync = repository.GetAllAsync();
            allAsync.Wait();
            return allAsync.Result;
        }

        public static TKey Set<TKey, TState>(this IRepository<TKey, TState> repository, TState state)
            where TState : ICorrelateBy<TKey>, new()
        {
            var allAsync = repository.SetAsync(state);
            allAsync.Wait();
            return allAsync.Result;
        }

        public static TState Remove<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : ICorrelateBy<TKey>, new()
        {
            var allAsync = repository.RemoveAsync(key);
            allAsync.Wait();
            return allAsync.Result;
        }

        public static TState Find<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : ICorrelateBy<TKey>, new()
        {
            var allAsync = repository.FindAsync(key);
            allAsync.Wait();
            return allAsync.Result;
        }

        public static TState Get<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var allAsync = repository.FindAsync(key);
            allAsync.Wait();
            return allAsync.Result;
        }

        public async static Task<TState> GetAsync<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var state = await repository.FindAsync(key);

            if (state != null)
                return state;

            throw new ArgumentOutOfRangeException("key", key, "unable to find key");
        }

        public static TState GetOrNew<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var orNewAsync = GetOrNewAsync(repository, key);
            orNewAsync.Wait();
            return orNewAsync.Result;
        }

        public async static Task<TState> GetOrNewAsync<TKey, TState>(this IRepository<TKey, TState> repository, TKey key)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var state = await repository.FindAsync(key);
            if (state != null)
                return state;

            var orAdd = new TState();
            await repository.SetAsync(orAdd);
            return orAdd;
        }

        public static TState GetOrSet<TKey, TState>(this IRepository<TKey, TState> repository, TState state)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var orAddAsync = repository.GetOrSetAsync(state);
            orAddAsync.Wait();
            return orAddAsync.Result;
        }

        public async static Task<TState> GetOrSetAsync<TKey, TState>(this IRepository<TKey, TState> repository, TState state)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            if (!EqualityComparer<TKey>.Default.Equals(state.Id, default(TKey)))
            {
                var existingState = await repository.FindAsync(state.Id);

                if (existingState != null)
                    return state;
            }

            await repository.SetAsync(state);

            return state;
        }

        public static IEnumerable<TState> GetOrSet<TKey, TState>(this IRepository<TKey, TState> repository, IEnumerable<TState> states)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            var orAddAsync = repository.GetOrSetAsync(states);
            orAddAsync.Wait();
            return orAddAsync.Result;
        }

        public static Task<TState[]> GetOrSetAsync<TKey, TState>(this IRepository<TKey, TState> repository, IEnumerable<TState> states)
            where TState : class, ICorrelateBy<TKey>, new()
        {
            return Task.WhenAll(states.Select(repository.GetOrSetAsync));
        }

    }
}