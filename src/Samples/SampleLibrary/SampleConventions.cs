using EasyNetQ;

namespace SampleLibrary
{
    public class SampleConventions : Conventions
    {
        public SampleConventions(ITypeNameSerializer typeNameSerializer)
            : base(typeNameSerializer)
        {
            QueueNamingConvention = (messageType, subscriptionId) => string.Format("{1}({0})", messageType.FullName, subscriptionId);
        }
    }
}