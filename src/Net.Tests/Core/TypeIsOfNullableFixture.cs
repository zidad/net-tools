using Net.Reflection;
using Xunit;

namespace Net.Tests.Core
{

    public class TypeIsOfNullableFixture
    {
        [Fact]
        public void NullableDecimalIsNullableType()
        {
            Assert.True(typeof (decimal?).IsNullable());
        }
    }
}