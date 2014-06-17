using System.Collections.Generic;
using Autofac;

namespace Net.EasyNetQ.Autofac
{
    public class AutofacGlobalPipeBuilder : IPipeBuilder 
    {
        private readonly IComponentContext scope;

        public AutofacGlobalPipeBuilder(IComponentContext scope)
        {
            this.scope = scope;
        }

        public IEnumerable<IPipe> Build<TConsumer>(TConsumer consumer)
        {
            return scope.ResolveNamed<IEnumerable<IPipe>>("global");
        }
    }
}