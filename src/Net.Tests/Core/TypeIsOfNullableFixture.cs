using Net.Reflection;
using NUnit.Framework;

namespace Net.Tests.Core
{
    [TestFixture]
    public class TypeIsOfNullableFixture
    {
        [Test]
        public void NullableDecimalIsNullableType()
        {
            Assert.IsTrue(typeof (decimal?).IsNullable());
        }
    }
}