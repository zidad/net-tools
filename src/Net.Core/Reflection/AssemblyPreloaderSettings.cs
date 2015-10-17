using System;
using System.IO;

namespace Net.Reflection
{
    public class AssemblyPreloaderSettings : IAssemblyPreloadSettings
    {
        public Func<FileInfo, bool> Filter { get; set; }
        public string Path { get; set; }
        public bool IncludeExecutables { get; set; }

        IAssemblyPreloadSettings IAssemblyPreloadSettings.IncludeExecutables(bool include)
        {
            this.IncludeExecutables = include;
            return this;
        }

        IAssemblyPreloadSettings IAssemblyPreloadSettings.Filter(Func<FileInfo, bool> filter)
        {
            this.Filter = filter;
            return this;
        }
    }
}