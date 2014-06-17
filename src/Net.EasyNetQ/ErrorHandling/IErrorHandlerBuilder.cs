using System.Collections.Generic;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    public interface IErrorHandlerBuilder
    {
        IEnumerable<IErrorHandler> Build<TConsumer>(TConsumer consumer);
    }
}