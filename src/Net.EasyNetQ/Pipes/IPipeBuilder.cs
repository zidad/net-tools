using System.Collections.Generic;

namespace Net.EasyNetQ.Autofac
{
    public interface IPipeBuilder
    {
        IEnumerable<IPipe> Build<TConsumer>(TConsumer consumer);
    }
}