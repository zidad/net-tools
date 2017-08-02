using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using Net.Reflection;
using NUnit.Framework;

namespace Net.Tests.Core
{
    [TestFixture]
    public class TypeIsOfGenericTypeFixture
    {
        [Test]
        public void SimpleGenericInterfaces()
        {
            Assert.IsTrue(typeof(List<string>).IsOfGenericType(typeof(ICollection<>)));
            Assert.IsTrue(typeof(List<string>).IsOfGenericType(typeof(IEnumerable<>)));
            Assert.IsTrue(typeof(IList<string>).IsOfGenericType(typeof(ICollection<>)));
            Assert.IsTrue(typeof(IList<string>).IsOfGenericType(typeof(IEnumerable<>)));

            Type concreteType;
            Assert.IsTrue(typeof(List<string>).IsOfGenericType(typeof(IEnumerable<>), out concreteType));
            Assert.AreEqual(typeof(IEnumerable<string>), concreteType);

            Assert.IsTrue(typeof(Table<string>).IsOfGenericType(typeof(IQueryable<>), out concreteType));
            Assert.AreEqual(typeof(IQueryable<string>), concreteType);

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IQueryable<string>), out concreteType);
            });

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IEnumerable<string>), out concreteType);
            });
        }

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

        [Test]
        public void TypeIsOfGenericInterfaceWithMultipleParametersAndOutputsConcreteType()
        {
            Type concreteType;
            Assert.IsTrue(typeof (ClassImplementingInterfaceWithMultipleGenericParameters)
                .IsOfGenericType(typeof (IInterfaceWithMultipleGenericParameters<,>), out concreteType));

            Assert.AreEqual(concreteType, typeof(IInterfaceWithMultipleGenericParameters<string, object>));
        }

        [Test]
        public void TypeIsOfGenericTypeWithMultipleParametersAndOutputsConcreteType()
        {
            Type concreteType;
            Assert.IsTrue(typeof(ClassInheritingClassWithMultipleGenericParameters)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.AreEqual(concreteType, typeof(ClassWithMultipleGenericParameters<string, string>));
        }

        [Test]
        public void TypeIsOfGenericTypeWithMultipleParametersAndOutputsConcreteType2()
        {
            Type concreteType;
            Assert.IsTrue(typeof(ClassWithMultipleGenericParameters<,>)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.IsTrue(typeof(ClassWithMultipleGenericParameters<,>)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>)));

            Assert.AreEqual(concreteType, typeof(ClassWithMultipleGenericParameters<,>));
        }

        [Test]
        public void ValidateParameters()
        {

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IQueryable<string>));
            });
            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IEnumerable<string>));
            });

            Type concreteType;
            Assert.IsFalse(((Type)null)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.IsFalse(((Type)null)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>)));

            Assert.Throws<ArgumentNullException>(() => {
                typeof(ClassInheritingClassWithMultipleGenericParameters).IsOfGenericType(null, out concreteType);
            });
            Assert.Throws<ArgumentNullException>(() => {
                typeof(ClassInheritingClassWithMultipleGenericParameters).IsOfGenericType(null);
            });

            Assert.Throws<ArgumentException>(() => {
                typeof(ClassImplementingInterfaceWithMultipleGenericParameters).IsOfGenericType(typeof(string));
            });
            Assert.Throws<ArgumentException>(() => {
                typeof(ClassImplementingInterfaceWithMultipleGenericParameters).IsOfGenericType(typeof(string), out concreteType);
            });

        }
    }

    public interface IInterfaceWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassInheritingClassWithMultipleGenericParameters : ClassWithMultipleGenericParameters<string, string> { }

    public class ClassImplementingInterfaceWithMultipleGenericParameters : IInterfaceWithMultipleGenericParameters<string, object> { }
}