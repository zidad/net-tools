using System.Collections.Generic;
using System.Linq;
using Cassette;
using Cassette.IO;
using Net.Collections;

namespace Net.Cassette
{
    public class RecursiveFileSearch : IFileSearch
    {
        private readonly IFileSearch _pattern;

        public RecursiveFileSearch(IFileSearch pattern)
        {
            _pattern = pattern;
        }

        IEnumerable<IFile> IFileSearch.FindFiles(IDirectory directory)
        {
            return
                _pattern
                    .FindFiles(directory)
                    .Union(directory
                    .GetDirectories()
                    .Descendants(d => d.GetDirectories())
                    .SelectMany(d => _pattern.FindFiles(d)));
        }

        public bool IsMatch(string filename)
        {
            return _pattern.IsMatch(filename);
        }
    }
}
