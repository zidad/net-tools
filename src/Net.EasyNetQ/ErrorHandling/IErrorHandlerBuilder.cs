using System.Collections.Generic;

namespace Net.EasyNetQ.ErrorHandling
{
    public interface IErrorHandlerBuilder
    {
        IEnumerable<IErrorHandler> Build<TConsumer>(TConsumer consumer);
    }
}