using System.Reflection;

namespace Net.Reflection
{
    public class ReflectionProperty<TAttribute>
    {
        public TAttribute Attribute { get; set; }
        public PropertyInfo Property { get; set; }
    }
}