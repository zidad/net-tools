using System.Collections.Generic;
using Autofac;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacGlobalErrorHandlerBuilder : IErrorHandlerBuilder 
    {
        private readonly IComponentContext scope;

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