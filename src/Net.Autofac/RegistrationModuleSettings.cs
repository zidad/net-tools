using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Net.Autofac
{
    internal class RegistrationModuleSettings
    {
        public RegistrationModuleSettings()
        {
            TypeFilter = type => true;
            AssemblyFilter = assembly => true;
            IgnoreNamespaces = new string[] { };
            AdditionalAssemblies = new Assembly[] { };
        }

        public Func<Type, bool> TypeFilter { get; set; }
        public Assembly[] AssembliesToInclude { get; set; }
        public List<Assembly> AssembliesMarkedForAutomaticDiscovery { get; set; }
        public Assembly[] AdditionalAssemblies { get; set; }
        public string[] IgnoreNamespaces { get; set; }
        public Func<Assembly, bool> AssemblyFilter { get; set; }
    }
}