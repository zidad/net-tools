using System;
using System.Collections.Generic;
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
                Path = AppDomain.CurrentDomain.BaseDirectory,
                IncludeExecutables = false
            };
        }

        public AssemblyPreloader(Action<IAssemblyPreloadSettings> assemblyPreloadConfigurator) : this()
        {
            if (assemblyPreloadConfigurator != null)
                assemblyPreloadConfigurator(Settings);
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
            var directoryInfo = new DirectoryInfo(p);
            IEnumerable<FileInfo> assemblyFiles = directoryInfo.GetFiles("*.dll", SearchOption.AllDirectories);

            if (Settings.IncludeExecutables)
                assemblyFiles = assemblyFiles.Union(directoryInfo.GetFiles("*.exe", SearchOption.AllDirectories));

            foreach (AssemblyName assemblyName in assemblyFiles
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
    }
}