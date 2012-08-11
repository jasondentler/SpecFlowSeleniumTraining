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

        public enum Browsers
        {
            Firefox,
            Chrome,
            InternetExplorer
        }

        private const string IISExpressPathKey = "IISExpressPath";
        private const string AppPoolHardResetKey = "RecycleAppPoolBetweenTests";
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

        public static bool RecycleAppPoolBetweenTests
        {
            get 
            { 
                var value = GetValue(AppPoolHardResetKey) ?? "";
                return value.ToLower() == "true";
            }
        }

        public static string RavenDbExecutablePath
        {
            get { return GetValue(RavenDbExecutablePathKey) ?? GetRavenDbServicePath(); }
        }

        public static string ChromeExecutablePath { get { return GetValue(ChromeExecutableKey); } }

        public static string TestOutputDirectory { get { return GetValue(TestOutputDirectoryKey) ?? "."; } }

        public static Browsers Browser { 
            get { 
                var name = GetValue(BrowserKey) ?? "Firefox";
                switch (name.ToLower())
                {
                    case "firefox":
                        return Browsers.Firefox;
                    case "chrome":
                        return Browsers.Chrome;
                    case "internetexplorer":
                    case "ie":
                        return Browsers.InternetExplorer;
                    default:
                        throw new NotSupportedException(
                            string.Format("{0} is not a supported browser. Valid values are Firefox, Chrome, and IE",
                                          name));
                }
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