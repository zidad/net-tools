using System;

namespace Net.EasyNetQ.Autofac
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PipeAttribute : Attribute
    {
        public PipeAttribute(Type pipeType, int order = 0)
        {
            PipeType = pipeType;
            Order = order;
        }

        public Type PipeType { get; set; }
        public int Order { get; set; }

        public IPipe Initialize(IPipe pipe)
        {
            return pipe;
        }
    }
}