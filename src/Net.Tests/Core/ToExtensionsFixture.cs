using System;
using System.Globalization;
using System.Threading;
using Net.System;
using NUnit.Framework;

namespace Net.Tests.Core
{
    [TestFixture]
    public class ToExtensionsFixture
    {
        [SetUp]
        public void Before() 
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        }

        [Test]
        public void DoubleTests() 
        { 
            Assert.AreEqual(1.23, "1.23".To<double>());
            Assert.AreEqual(1.23, "1.23".To<double?>());
            Assert.AreEqual(null, "".To<double?>());
            Assert.AreEqual(0, "".To<double>());
        }

        [Test]
        public void IntTests() 
        { 
            Assert.AreEqual(1, "1".To<int>());
            Assert.AreEqual(1, "1".To<int?>());
            Assert.AreEqual(null, "".To<int?>());
            Assert.AreEqual(0, "".To<int>());

            Assert.AreEqual("2", 2.To<string>("#"));
            Assert.AreEqual("1.23", 1.23.To<string>());
        }

        [Test]
        public void Formatting()
        {
            Assert.AreEqual("<2>", 2.To<string>("<#>"));
        }
        
        [Test]
        public void StringTests() 
        {
            Assert.AreEqual("", ((object)null).To<string>());
        }   
    }
}
