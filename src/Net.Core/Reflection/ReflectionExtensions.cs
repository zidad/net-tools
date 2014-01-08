using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Net.Reflection
{
    public static class ReflectionExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this MemberDescriptor member) where TAttribute : Attribute
        {
            return member.Attributes.OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// Get's all the properties with a certain attribute present, and returns in some Tuple
        /// </summary>
        public static IEnumerable<ReflectionProperty<TAttribute>> GetPropertiesWithAttribute<TAttribute>(this Type member) where TAttribute : Attribute
        {
            return member
                .GetProperties()
                .Select(p => new ReflectionProperty<TAttribute>
                {
                    Attribute = p.GetAttribute<TAttribute>(),
                    Property = p
                })
                .Where(a => a.Attribute != null);
        }

        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider member, bool inherit = true) where TAttribute : Attribute
        {
            return member.GetAttributes<TAttribute>().FirstOrDefault();
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider member, bool inherit = true) where TAttribute : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
        }

        public static TAttribute GetAttribute<TAttribute>(this ICustomTypeDescriptor typeDescriptor) where TAttribute : Attribute
        {
            return typeDescriptor.GetAttributes().OfType<TAttribute>().FirstOrDefault();
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberDescriptor member) where TAttribute : Attribute
        {
            return member.Attributes.OfType<TAttribute>();
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomTypeDescriptor typeDescriptor) where TAttribute : Attribute
        {
            return typeDescriptor.GetAttributes().OfType<TAttribute>();
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
                return true;

            return (type.IsGenericType && !type.IsGenericTypeDefinition && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static Boolean IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;
            return isAnonymousType;
        }

        public static bool IsOfGenericType(this Type typeToCheck, Type genericType)
        {
            while (true)
            {
                if (genericType == null)
                    throw new ArgumentNullException("genericType");

                if (!genericType.IsGenericTypeDefinition)
                    throw new ArgumentException("The definition needs to be a GenericTypeDefinition", "genericType");

                if (typeToCheck == null || typeToCheck == typeof(object))
                    return false;

                if (typeToCheck == genericType)
                    return true;

                if ((typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck) == genericType)
                    return true;

                if (genericType.IsInterface)
                    return typeToCheck.GetInterfaces().Any(i => i.IsOfGenericType(genericType));

                typeToCheck = typeToCheck.BaseType;
            }
        }

        public static bool IsOfGenericType(this Type typeToCheck, Type genericType, out Type enumerableType)
        {
            while (true)
            {
                enumerableType = null;

                if (genericType == null)
                    throw new ArgumentNullException("genericType");

                if (!genericType.IsGenericTypeDefinition)
                    throw new ArgumentException("The definition needs to be a GenericTypeDefinition", "genericType");

                if (typeToCheck == null || typeToCheck == typeof(object))
                    return false;

                if (typeToCheck == genericType)
                {
                    enumerableType = typeToCheck;
                    return true;
                }

                if ((typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck) == genericType)
                {
                    enumerableType = typeToCheck;
                    return true;
                }

                if (genericType.IsInterface)
                {
                    foreach (Type i in typeToCheck.GetInterfaces())
                    {
                        if (i.IsOfGenericType(genericType, out enumerableType))
                        {
                            return true;
                        }
                    }
                }

                typeToCheck = typeToCheck.BaseType;
            }
        }
    }
}