using System;
using System.IO;

namespace Net.Reflection
{
    public class AssemblyPreloaderSettings
    {
        public string Path { get; set; }
        public Func<FileInfo, bool> Filter { get; set; }
    }
}