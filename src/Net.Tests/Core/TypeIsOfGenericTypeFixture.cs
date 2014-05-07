using Net.Reflection;
using NUnit.Framework;

namespace Net.Tests.Core
{
    [TestFixture]
    public class TypeIsOfGenericTypeFixture
    {
        [Test]
        public void TypeIsOfGenericInterfaceWithMultipleParameters()
        {
            Assert.IsTrue(typeof (ClassImplementingInterfaceWithMultipleGenericParameters)
                .IsOfGenericType(typeof (IInterfaceWithMultipleGenericParameters<,>)));
        }

        [Test]
        public void TypeIsOfGenericTypeWithMultipleParameters()
        {
            Assert.IsTrue(typeof(ClassInheritingClassWithMultipleGenericParameters)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>)));
        }
    }

    public interface IInterfaceWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassInheritingClassWithMultipleGenericParameters : ClassWithMultipleGenericParameters<string, string>
    { }

    public class ClassImplementingInterfaceWithMultipleGenericParameters : IInterfaceWithMultipleGenericParameters<string, object> { }
}