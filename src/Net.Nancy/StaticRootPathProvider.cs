using Nancy;

namespace Net.Nancy
{
    public class StaticRootPathProvider : IRootPathProvider
    {
        private readonly string path;

        public StaticRootPathProvider(string path)
        {
            this.path = path;
        }

        public string GetRootPath()
        {
            return path;
        }
    }
}
