using System;
using Net.EasyNetQ.Pipes;

namespace Net.EasyNetQ.Autofac
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ErrorHandlerAttribute : Attribute
    {
        public ErrorHandlerAttribute(Type errorHandlerType, int order = 0)
        {
            ErrorHandlerType = errorHandlerType;
            Order = order;
        }

        public Type ErrorHandlerType { get; set; }
        public int Order { get; set; }

        public virtual IErrorHandler Initialize(IErrorHandler handler)
        {
            return handler;
        }
    }
}