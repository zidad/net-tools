using System.Collections.Generic;
using System.Linq;
using Autofac;
using Net.EasyNetQ.Pipes;
using Net.Reflection;

namespace Net.EasyNetQ.Autofac
{
    public class AttributePipeBuilder : IPipeBuilder 
    {
        readonly IComponentContext scope;

        public AttributePipeBuilder(IComponentContext scope)
        {
            this.scope = scope;
        }

        public IEnumerable<IPipe> Build<TConsumer>(TConsumer consumer)
        {
            return consumer.GetType()
                .GetAttributes<PipeAttribute>()
                .OrderBy(attribute => attribute.Order)
                .Select(attribute => attribute.Initialize((IPipe) scope.Resolve(attribute.PipeType)));
        }
    }
}