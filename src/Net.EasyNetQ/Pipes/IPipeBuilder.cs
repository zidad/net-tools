using System.Collections.Generic;

namespace Net.EasyNetQ.Pipes
{
    public interface IPipeBuilder
    {
        IEnumerable<IPipe> Build<TConsumer>(TConsumer consumer);
    }
}