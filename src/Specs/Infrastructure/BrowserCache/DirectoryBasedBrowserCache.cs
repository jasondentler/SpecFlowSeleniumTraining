using System.IO;
using System.Linq;

namespace Specs.Infrastructure.BrowserCache
{
    public abstract class DirectoryBasedBrowserCache : IBrowserCache
    {
        public abstract void Clear();

        protected void ClearDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            Directory.EnumerateDirectories(directory)
                .ToList()
                .ForEach(dir => Directory.Delete(dir, true));

            Directory.EnumerateFiles(directory)
                .ToList()
                .ForEach(File.Delete);
        }

    }
}
