using System.Threading;

namespace Net.EasyNetQ.Persistence
{
    public class IntGenerator : IKeyGenerator<int>
    {
        private int id = 0;

        public int NewKey()
        {
            return Interlocked.Increment(ref id);
        }
    }
}