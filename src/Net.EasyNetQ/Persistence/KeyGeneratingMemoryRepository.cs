using System.Globalization;
using Net.EasyNetQ.Persistence.InMemory;

namespace Net.EasyNetQ.Persistence
{
    public class KeyGeneratingMemoryRepository<TState> : InMemoryRepository<string, TState>
        where TState : ICorrelateBy<string>, new()
    {
        private readonly IKeyGenerator<string> keyGenerator;

        public KeyGeneratingMemoryRepository(IKeyGenerator<string> keyGenerator)
        {
            this.keyGenerator = keyGenerator;
        }

        public override string Set(TState state)
        {
            if (string.IsNullOrEmpty(state.Id))
                state.Id = keyGenerator.NewKey().ToString(CultureInfo.InvariantCulture);

            base.Set(state);

            return state.Id;
        }
    }
}