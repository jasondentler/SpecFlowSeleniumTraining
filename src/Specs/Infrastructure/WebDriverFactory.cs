using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using Specs.Infrastructure.BrowserCache;

namespace Specs.Infrastructure
{
    public class WebDriverFactory
    {

        public const string FirefoxProfileName = "selenium";

        public IWebDriver CreateDriver(Settings.Browsers browser)
        {
            switch (browser)
            {
                case Settings.Browsers.Firefox:
                    return CreateFirefoxDriver();
                case Settings.Browsers.Chrome:
                    return CreateChromeDriver();
                case Settings.Browsers.InternetExplorer:
                    return CreateInternetExplorerDriver();
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported.", browser));
            }
        }

        private FirefoxDriver CreateFirefoxDriver()
        {
            var profileManager = new FirefoxProfileManager();
            var profile = profileManager.GetProfile(FirefoxProfileName);

            if (profile != null)
                profile.Clean(); // If it exists, delete it and recreate fresh

            profileManager.CreateProfile(FirefoxProfileName);

            profile = profileManager.GetProfile(FirefoxProfileName);
            return new FirefoxDriver(new FirefoxBinary(), profile);
        }


        private ChromeDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();

            var exePath = Settings.ChromeExecutablePath;

            if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                options.BinaryLocation = exePath;

            return new ChromeDriver(options);
        }

        private InternetExplorerDriver CreateInternetExplorerDriver()
        {
            var options = new InternetExplorerOptions()
                              {
                                  IgnoreZoomLevel = true
                              };
            return new InternetExplorerDriver(options);
        }

    }
}
