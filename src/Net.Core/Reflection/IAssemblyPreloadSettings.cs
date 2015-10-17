using System;
using System.IO;

namespace Net.Reflection
{
    public interface IAssemblyPreloadSettings
    {
        IAssemblyPreloadSettings IncludeExecutables(bool include = true);
        IAssemblyPreloadSettings Filter(Func<FileInfo, bool> filter);
    }
}