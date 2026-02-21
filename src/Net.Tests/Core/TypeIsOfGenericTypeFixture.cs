using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Net.Reflection;
using Xunit;

namespace Net.Tests.Core
{
    public class TypeIsOfGenericTypeFixture
    {
        [Fact]
        public void SimpleGenericInterfaces()
        {
            Assert.True(typeof(List<string>).IsOfGenericType(typeof(ICollection<>)));
            Assert.True(typeof(List<string>).IsOfGenericType(typeof(IEnumerable<>)));
            Assert.True(typeof(IList<string>).IsOfGenericType(typeof(ICollection<>)));
            Assert.True(typeof(IList<string>).IsOfGenericType(typeof(IEnumerable<>)));

            Assert.True(typeof(List<string>).IsOfGenericType(typeof(IEnumerable<>), out var concreteType));
            Assert.Equal(typeof(IEnumerable<string>), concreteType);

            Assert.True(typeof(Table<string>).IsOfGenericType(typeof(IQueryable<>), out concreteType));
            Assert.Equal(typeof(IQueryable<string>), concreteType);

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IQueryable<string>), out concreteType);
            });

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IEnumerable<string>), out concreteType);
            });
        }

        [Fact]
        public void TypeIsOfGenericInterfaceWithMultipleParameters()
        {
            Assert.True(typeof (ClassImplementingInterfaceWithMultipleGenericParameters)
                .IsOfGenericType(typeof (IInterfaceWithMultipleGenericParameters<,>)));
        }

        [Fact]
        public void TypeIsOfGenericTypeWithMultipleParameters()
        {
            Assert.True(typeof(ClassInheritingClassWithMultipleGenericParameters)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>)));
        }

        [Fact]
        public void TypeIsOfGenericInterfaceWithMultipleParametersAndOutputsConcreteType()
        {
            Type concreteType;
            Assert.True(typeof (ClassImplementingInterfaceWithMultipleGenericParameters)
                .IsOfGenericType(typeof (IInterfaceWithMultipleGenericParameters<,>), out concreteType));

            Assert.Equal(concreteType, typeof(IInterfaceWithMultipleGenericParameters<string, object>));
        }

        [Fact]
        public void TypeIsOfGenericTypeWithMultipleParametersAndOutputsConcreteType()
        {
            Type concreteType;
            Assert.True(typeof(ClassInheritingClassWithMultipleGenericParameters)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.Equal(concreteType, typeof(ClassWithMultipleGenericParameters<string, string>));
        }

        [Fact]
        public void TypeIsOfGenericTypeWithMultipleParametersAndOutputsConcreteType2()
        {
            Type concreteType;
            Assert.True(typeof(ClassWithMultipleGenericParameters<,>)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.True(typeof(ClassWithMultipleGenericParameters<,>)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>)));

            Assert.Equal(concreteType, typeof(ClassWithMultipleGenericParameters<,>));
        }

        [Fact]
        public void ValidateParameters()
        {

            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IQueryable<string>));
            });
            Assert.Throws<ArgumentException>(() => {
                typeof(Table<string>).IsOfGenericType(typeof(IEnumerable<string>));
            });

            Type concreteType;
            Assert.False(((Type)null)
                .IsOfGenericType(typeof(ClassWithMultipleGenericParameters<,>), out concreteType));

            Assert.False(((Type)null)
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

    public class Table<T> : EnumerableQuery<T>
    {
        public Table() : base(Enumerable.Empty<T>()) { }
    }

    public interface IInterfaceWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassWithMultipleGenericParameters<T1, T2> { }
    
    public class ClassInheritingClassWithMultipleGenericParameters : ClassWithMultipleGenericParameters<string, string> { }

    public class ClassImplementingInterfaceWithMultipleGenericParameters : IInterfaceWithMultipleGenericParameters<string, object> { }
}