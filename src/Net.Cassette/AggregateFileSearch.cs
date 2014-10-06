using System.Collections.Generic;
using System.Linq;
using Cassette;
using Cassette.IO;
using Net.Collections;

namespace Net.Cassette
{
    public class AggregateFileSearch : IFileSearch
    {
        private readonly IFileSearch[] _patterns;

        public AggregateFileSearch(params IFileSearch[] patterns)
        {
            _patterns = patterns;
        }

        public IEnumerable<IFile> FindFiles(IDirectory directory)
        {
            return _patterns.SelectMany(p => p.FindFiles(directory)).Distinct(f => f.FullPath);
        }

        public bool IsMatch(string filename)
        {
            return _patterns.Any(p => p.IsMatch(filename));
        }
    }
}