using System.Globalization;
using System.Threading;
using Net.System;
using Xunit;

namespace Net.Tests.Core
{
    public class ToExtensionsFixture
    {
        public ToExtensionsFixture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        }

        [Fact]
        public void DoubleTests() 
        { 
            Assert.Equal(1.23, "1.23".To<double>());
            Assert.Equal(1.23, "1.23".To<double?>());
            Assert.Equal(null, "".To<double?>());
            Assert.Equal(0, "".To<double>());
        }

        [Fact]
        public void IntTests() 
        { 
            Assert.Equal(1, "1".To<int>());
            Assert.Equal(1, "1".To<int?>());
            Assert.Equal(null, "".To<int?>());
            Assert.Equal(0, "".To<int>());

            Assert.Equal("2", 2.To<string>("#"));
            Assert.Equal("1.23", 1.23.To<string>());
        }

        [Fact]
        public void Formatting()
        {
            Assert.Equal("<2>", 2.To<string>("<#>"));
        }
        
        [Fact]
        public void StringTests() 
        {
            Assert.Equal("", ((object)null).To<string>());
        }   
    }
}
