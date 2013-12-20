using Autofac;
using Net.Reflection;

namespace Net.Autofac
{
    public class AssemblyPreloaderModule : Module
    {
        private readonly AssemblyPreloader preloader;

        public AssemblyPreloaderModule()
        {
            preloader = new AssemblyPreloader();
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            preloader.PreLoadAssembliesFromPath();
        }
    }
}