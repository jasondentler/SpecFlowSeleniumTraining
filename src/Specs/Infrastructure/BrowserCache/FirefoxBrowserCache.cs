using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Firefox;

namespace Specs.Infrastructure.BrowserCache
{
    public class FirefoxBrowserCache : DirectoryBasedBrowserCache
    {
        private readonly FirefoxDriver _driver;

        public FirefoxBrowserCache(FirefoxDriver driver)
        {
            _driver = driver;
        }

        public override void Clear()
        {
            var profile = _driver.GetCurrentProfile();
            var profileDirectory = profile.ProfileDirectory;
            const string relativeCacheDirectory = @".\Cache\";
            var absoluteCacheDirectory = Path.GetFullPath(Path.Combine(profileDirectory, relativeCacheDirectory));
            ClearDirectory(absoluteCacheDirectory);
        }

    }
}
