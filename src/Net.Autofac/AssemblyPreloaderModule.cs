using System;
using System.IO;
using Autofac;
using Net.Reflection;

namespace Net.Autofac
{
    public class AssemblyPreloaderModule : Module
    {
        private readonly AssemblyPreloader preloader;

        public AssemblyPreloaderModule(Action<IAssemblyPreloadSettings> assemblyPreloadConfigurator)
        {
            preloader = new AssemblyPreloader(assemblyPreloadConfigurator);
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            preloader.PreLoadAssembliesFromPath();
        }
    }
}