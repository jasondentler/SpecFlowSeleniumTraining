using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Specs.Infrastructure.BrowserCache
{
    public class ChromeBrowserCache : DirectoryBasedBrowserCache
    {
        public override void Clear()
        {
            Process.GetProcessesByName("chrome")
                .OrderByDescending(p => p.StartTime)
                .Select(p => p.MainModule.FileName)
                .Distinct()
                .Select(ChromePathToCacheFolder)
                .ToList()
                .ForEach(ClearDirectory);
        }

        private string ChromePathToCacheFolder(string chromePath)
        {
            var applicationDirectory = Path.GetDirectoryName(chromePath) ?? "";
            var appRelativePath = @"..\User Data\Default\Cache";
            var cachePath = Path.GetFullPath(Path.Combine(applicationDirectory, appRelativePath));
            return cachePath;
        }

    }
}
