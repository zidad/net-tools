using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;
using Net.Reflection;

namespace Net.Autofac
{
    public static class AutofacExtensions
    {
        public static IEnumerable<Type> ImplementationsForGenericType(this IComponentContext components, Type type)
        {
            var registrations = components.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new { r, s });

            var implementations = registrations.Where(rs => rs.s.ServiceType.IsOfGenericType(type))
                .Select(rs => rs.r.Activator.LimitType)
                .Distinct()
                .ToList();

            return implementations;
        }

        public static IEnumerable<Type> ImplementationsFor<T>(this IComponentContext components)
        {
            var registrations = components.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new { r, s });

            var implementations = registrations.Where(rs =>
                    rs.s.ServiceType.IsAssignableTo<T>()
                )
                .Select(rs => rs.r.Activator.LimitType)
                .Distinct()
                .ToList();

            return implementations;
        }

        public static IEnumerable<Type> Implementations(this IComponentContext components)
        {
            var registrations = components.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new { r, s });

            var implementations = registrations
                .Select(rs => rs.s.ServiceType)
                .Distinct()
                .ToList();

            return implementations;
        }

        public static IRegistrationBuilder<TLimit, ScanningActivatorData, DynamicRegistrationStyle>
            AsBaseType<TLimit>(this IRegistrationBuilder<TLimit, ScanningActivatorData, DynamicRegistrationStyle> registration)
        {
            registration.As(t => t.BaseType != typeof(object) ? t.BaseType : t);
            return registration;
        }
    }
}
