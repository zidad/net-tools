using System;
using Net.Annotations;

namespace Net.DependencyInjection
{
    /// <summary>
    /// mark an assembly to be automatically scanned for classes and interfaces
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RegisterAssemblyInContainer : Attribute
    {
        public RegisterAssemblyInContainer()
            : this(new string[] { })
        {
        }

        public RegisterAssemblyInContainer(params string[] ignoreNamespaces)
        {
            IgnoreNamespaces = ignoreNamespaces;
        }

        public string[] IgnoreNamespaces { get; set; }
    }
}