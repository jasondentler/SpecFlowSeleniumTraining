using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace Specs.Infrastructure
{
    public static class Settings
    {

        private const string IISExpressPathKey = "IISExpressPath";
        private const string WebsitePathKey = "WebsitePath";
        private const string BrowserKey = "Browser";
        private const string ChromeExecutableKey = "ChromeExecutable";
        private const string RavenDbExecutablePathKey = "RavenDBExecutablePath";
        private const string TestOutputDirectoryKey = "TestOutputDirectory";

        public static string IISExpressPath
        {
            get
            {
                const string defaultPath = @"C:\Program Files (x86)\IIS Express\iisexpress.exe";
                return GetValue(IISExpressPathKey) ?? defaultPath;
            }
        }

        public static string WebsitePath { get { return Path.GetFullPath(GetValue(WebsitePathKey)); } }

        public static string RavenDbExecutablePath
        {
            get { return GetValue(RavenDbExecutablePathKey) ?? GetRavenDbServicePath(); }
        }

        public static string TestOutputDirectory { get { return GetValue(TestOutputDirectoryKey) ?? "."; } }

        public static IWebDriver CreateWebDriver()
        {
            var name = GetValue(BrowserKey);
            switch (name.ToLowerInvariant())
            {
                case "firefox":
                    return new FirefoxDriver();
                case "chrome":

                    var options = new ChromeOptions();

                    var exePath = GetValue(ChromeExecutableKey);

                    if (File.Exists(exePath))
                        options.BinaryLocation = exePath;

                    return new ChromeDriver(options);

                case "safari":
                    return new SafariDriver();
                case "ie":
                case "ie7":
                case "ie8":
                case "ie9":
                case "ie10":
                case "internet explorer":
                case "internetexplorer":
                    return new InternetExplorerDriver();
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported.", name));
            }
        }

        private static string GetRavenDbServicePath()
        {
            //HKLM\System\CurrentControlSet\Services\<%serviceNa me%>\ImagePath
            const string path = @"System\CurrentControlSet\Services\RavenDB";
            using (var reg = Registry.LocalMachine.OpenSubKey(path, false))
            {
                Debug.Assert(reg != null, "reg != null");
                return (string)reg.GetValue("ImagePath", null);
            }
        }

        private static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key)
                ? ConfigurationManager.AppSettings[key]
                : null;
        }

    }
}