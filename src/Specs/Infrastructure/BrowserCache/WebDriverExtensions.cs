using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Specs.Infrastructure.BrowserCache
{
    public static class WebDriverExtensions
    {

        public static void ClearCache(this IWebDriver driver)
        {
            var cache = GetCache(driver);
            cache.Clear();
        }

        private static IBrowserCache GetCache(IWebDriver driver)
        {
            if (driver.GetType().IsAssignableFrom(typeof(FirefoxDriver)))
                return new FirefoxBrowserCache((FirefoxDriver) driver);
            if (driver.GetType().IsAssignableFrom(typeof(ChromeDriver)))
                return new ChromeBrowserCache();
            if (driver.GetType().IsAssignableFrom(typeof(InternetExplorerDriver)))
                return new InternetExplorerBrowserCache();
            throw new NotImplementedException("Browser cache hasn't been implemented for this driver.");
        }

    }
}
