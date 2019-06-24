using System.Collections.Generic;
using Autofac;
using Net.EasyNetQ.ErrorHandling;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacGlobalErrorHandlerBuilder : IErrorHandlerBuilder 
    {
        readonly IComponentContext scope;

        public AutofacGlobalErrorHandlerBuilder(IComponentContext scope)
        {
            this.scope = scope;
        }

        public IEnumerable<IErrorHandler> Build<TConsumer>(TConsumer consumer)
        {
            return scope.ResolveNamed<IEnumerable<IErrorHandler>>("global");
        }
    }
}