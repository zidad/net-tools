using Net.Reflection;
using NUnit.Framework;

namespace Net.Tests.Core
{
    [TestFixture]
    public class TypeIsOfGenericTypeFixture
    {
        [Test]
        public void TypeIsOfGenericTypeWithMultipleParameters()
        {
            Assert.IsTrue(typeof (ClassImplementingInterfaceWithMultipleGenericParameters)
                .IsOfGenericType(typeof (IInterfaceWithMultipleGenericParameters<,>)));
        }
    }

    public interface IInterfaceWithMultipleGenericParameters<T1, T2> { }

    public class ClassImplementingInterfaceWithMultipleGenericParameters : IInterfaceWithMultipleGenericParameters<string, object> { }
}