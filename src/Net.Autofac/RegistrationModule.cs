using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using Autofac;
using Autofac.Util;
using Net.Annotations;
using Net.DependencyInjection;
using Net.Reflection;
using Module = Autofac.Module;

namespace Net.Autofac
{
    /// <summary>
    ///     Registers all types from all assemblies in the current AppDomain that have the AutofacRegistrationAttribute
    ///     applied.
    ///     - Ignores types that have the 'Ignore' bit set
    /// </summary>
    [UsedImplicitly]
    public class RegistrationModule : Module
    {
        private readonly RegistrationModuleSettings settings;

        public RegistrationModule()
        {
            settings = new RegistrationModuleSettings();
        }

        public IEnumerable<string> AssemblyNamespacesToIgnoreFromAttributes
        {
            get
            {
                return GetAssembliesMarkedForAutomaticDiscovery()
                    .SelectMany(a => a.GetAttribute<RegisterAssemblyInContainer>().IgnoreNamespaces);
            }
        }

        public IEnumerable<string> NamespacesToIgnore
        {
            get { return settings.IgnoreNamespaces.Union(AssemblyNamespacesToIgnoreFromAttributes); }
        }

        private Func<Type, bool> TypeIsNonDefaultImplementationPredicate
        {
            get
            {
                return t =>
                {
                    //if (typeof(Attribute).IsAssignableFrom(t))
                    //return false;

                    if (t.IsAnonymousType())
                        return false;

                    if (t.IsNested)
                        return false;

                    if (t.IsAbstract)
                        return false;

                    if (NamespacesToIgnore.Any(ns => (t.Namespace ?? "").StartsWith(ns)))
                        return false;

                    var attribute = t.GetAttribute<RegisterAttribute>();
                    if (attribute == null)
                        return settings.TypeFilter(t);

                    if (attribute.Ignore || attribute.IsDefaultImplementation)
                        return false;

                    return settings.TypeFilter(t);
                };
            }
        }

        private static Func<Type, bool> TypeIsDefaultImplementationPredicate
        {
            get
            {
                return t =>
                {
                    var attribute = t.GetAttribute<RegisterAttribute>();

                    if (attribute == null)
                        return false;

                    if (attribute.Ignore)
                        return false;

                    if (!string.IsNullOrEmpty(attribute.Key))
                    {
                        string appSetting = ConfigurationManager.AppSettings["Registration:" + attribute.Key];

                        return !string.IsNullOrEmpty(appSetting)
                            ? appSetting == attribute.Value
                            : attribute.IsDefaultImplementation;
                    }

                    return !attribute.Ignore && attribute.IsDefaultImplementation;
                };
            }
        }

        public Assembly[] AssembliesToInclude
        {
            get
            {
                return settings.AssembliesToInclude ?? (settings.AssembliesToInclude =
                    GetAssembliesMarkedForAutomaticDiscovery()
                        .Union(settings.AdditionalAssemblies)
                        .Distinct()
                        .ToArray());
            }
        }

        public IEnumerable<Assembly> GetAssembliesMarkedForAutomaticDiscovery()
        {
            if (settings.AssembliesMarkedForAutomaticDiscovery != null)
                return settings.AssembliesMarkedForAutomaticDiscovery;

            var assemblies =
                HttpContext.Current != null
                    ? BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                    : AppDomain.CurrentDomain.GetAssemblies();

            return
                settings.AssembliesMarkedForAutomaticDiscovery =
                    assemblies.Where(settings.AssemblyFilter).Where(a => a.GetAttribute<RegisterAssemblyInContainer>() != null).ToList();
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterOpenGenericTypes(builder);

            RegisterNonDefaultImplementations(builder);

            RegisterDefaultImplementations(builder);
        }

        private void RegisterDefaultImplementations(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(AssembliesToInclude)
                .Where(TypeIsDefaultImplementationPredicate)
                .AsSelf()
                .AsBaseType()
                .AsImplementedInterfaces()
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
        }

        private void RegisterNonDefaultImplementations(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(AssembliesToInclude)
                .Where(TypeIsNonDefaultImplementationPredicate)
                .AsSelf()
                .AsBaseType()
                .AsImplementedInterfaces()
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues)
                .WithMetadata(GetMetaData);
        }

        private void RegisterOpenGenericTypes(ContainerBuilder builder)
        {
            var openGenericTypes = AssembliesToInclude
                .SelectMany(a => a.GetLoadableTypes())
                .Select(t => new { Type = t, Attribute = t.GetAttribute<RegisterAttribute>() })
                .Where(t => 
                    t.Type.IsClass &&
                    t.Type.IsGenericTypeDefinition &&
                    !t.Type.IsAnonymousType() &&
                    !t.Type.IsAbstract
                );

            foreach (var openGenericType in openGenericTypes)
                builder.RegisterGeneric(openGenericType.Type)
                    .AsSelf()
                    .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);
        }

        private static IEnumerable<KeyValuePair<string, object>> GetMetaData(Type type)
        {
            var registerServiceAttribute = type.GetAttribute<RegisterAttribute>();

            if (registerServiceAttribute == null)
            {
                yield return new KeyValuePair<string, object>("Order", 100);
                yield break;
            }

            yield return new KeyValuePair<string, object>("Order", registerServiceAttribute.Order);
            if (!string.IsNullOrEmpty(registerServiceAttribute.Key))
                yield return new KeyValuePair<string, object>(registerServiceAttribute.Key, registerServiceAttribute.Value);
        }

        public RegistrationModule IncludeAssemblies(params Assembly[] additionalAssemblies)
        {
            settings.AdditionalAssemblies = additionalAssemblies;
            return this;
        }

        public RegistrationModule FilterTypes(Func<Type, bool> typeFilter)
        {
            settings.TypeFilter = typeFilter;
            return this;
        }

        public RegistrationModule FilterAssemblies(Func<Assembly, bool> assemblyFilter)
        {
            settings.AssemblyFilter = assemblyFilter;
            return this;
        }
    }
}