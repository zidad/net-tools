using System;
using Net.Annotations;

namespace Net.DependencyInjection
{
    /// <summary>
    /// mark a class or interface as a service to be automatically discovered
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class RegisterAttribute : Attribute
    {
        public RegisterAttribute()
        {
            IsDefaultImplementation = false;
            Ignore = false;
        }

        public int Order { get; set; }

        public bool IsDefaultImplementation { get; set; }
        public bool Ignore { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public string LifeTimeScopeTag { get; set; }
    }
}