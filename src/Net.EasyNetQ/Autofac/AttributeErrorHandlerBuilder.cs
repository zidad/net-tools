using System.Collections.Generic;
using System.Linq;
using Autofac;
using Net.EasyNetQ.ErrorHandling;
using Net.Reflection;

namespace Net.EasyNetQ.Autofac
{
    public class AttributeErrorHandlerBuilder : IErrorHandlerBuilder 
    {
        readonly IComponentContext scope;

        public AttributeErrorHandlerBuilder(IComponentContext scope)
        {
            this.scope = scope;
        }

        public IEnumerable<IErrorHandler> Build<TConsumer>(TConsumer consumer)
        {
            return consumer.GetType()
                .GetAttributes<ErrorHandlerAttribute>()
                .OrderBy(attribute => attribute.Order)
                .Select(attribute => attribute.Initialize((IErrorHandler) scope.Resolve(attribute.ErrorHandlerType)));
        }
    }
}