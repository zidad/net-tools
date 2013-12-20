using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Net.Reflection
{
    public class AssemblyPreloader
    {
        private AssemblyPreloaderSettings Settings { get; set; }

        public AssemblyPreloader()
        {
            Settings = new AssemblyPreloaderSettings
            {
                Filter = x => true,
                Path = AppDomain.CurrentDomain.BaseDirectory
            };
        }

        public void PreLoadAssembliesFromPath()
        {
            PreLoadAssembliesFromPath(Settings.Path);
        }

        public void PreLoadAssembliesFromPath(string p)
        {
            //get all .dll files from the specified path and load the lot
            //you might not want recursion - handy for localised assemblies 
            //though especially.
            var assemblyFiles = new DirectoryInfo(p).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var assemblyName in assemblyFiles
                .Where(Settings.Filter)
                .Select(fi => fi.FullName)
                .Select(AssemblyName.GetAssemblyName)
                .Where(a => !AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Any(assembly => AssemblyName.ReferenceMatchesDefinition(a, assembly.GetName()))))
            {
                //crucial - USE THE ASSEMBLY NAME.
                //in a web app, this assembly will automatically be bound from the 
                //Asp.Net Temporary folder from where the site actually runs.
                try
                {
                    Assembly.Load(assemblyName);
                }
                catch (BadImageFormatException)
                {
                    //Debug.WriteLine(e); // TODO: just ignore this?
                }
            }
        }

        public AssemblyPreloader Filter(Func<FileInfo, bool> filter)
        {
            Settings.Filter = filter;
            return this;
        }
    }
}