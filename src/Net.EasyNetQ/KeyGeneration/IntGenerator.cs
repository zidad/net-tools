using System.Threading;

namespace Net.EasyNetQ.KeyGeneration
{
    public class IntGenerator : IKeyGenerator<int>
    {
        int id = 0;

        public int NewKey()
        {
            return Interlocked.Increment(ref id);
        }
    }
}