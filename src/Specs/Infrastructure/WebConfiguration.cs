using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Specs.Infrastructure
{
    [Binding]
    public class WebConfiguration
    {

        private static IISProcess _iisProcess;
        private static IWebDriver _driver;
        private static string _mainWindow;

        private RavenInstance _ravenInstance;

        [BeforeTestRun]
        public static void Startup()
        {
            _iisProcess = new IISProcess(8082);
            _iisProcess.Start();
            _driver = Settings.CreateWebDriver();
            _mainWindow = _driver.CurrentWindowHandle;
        }

        [AfterTestRun]
        public static void Shutdown()
        {
            _iisProcess.Stop();
            _iisProcess.Dispose();

            _driver.WindowHandles.ToList()
                .ForEach(handle =>
                             {
                                 _driver.SwitchTo().Window(handle);
                                 _driver.Close();
                             });

            _driver.Dispose();
            TerminateSeleniumDrivers();
        }

        [BeforeScenario("web")]
        public void Setup()
        {
            _ravenInstance = new RavenInstance();

            ScenarioContext.Current.Set(_driver);
            ScenarioContext.Current.Set(_iisProcess.Url);
            
            CleanUpBrowserInstance();
        }

        [AfterScenario("web")]
        public void AfterWebScenario()
        {
            _ravenInstance.Dispose();
            CaptureErrorInformation();
        }

        private static void CaptureErrorInformation()
        {
            var ctx = ScenarioContext.Current;
            if (ctx.TestError == null)
                return;

            var driver = ScenarioContext.Current.Get<IWebDriver>();
            
            CaptureHtml(driver);
            
            var screenshotter = driver as ITakesScreenshot;
            if (screenshotter != null)
                TakeErrorScreenshot(screenshotter);
        }

        private static void TakeErrorScreenshot(ITakesScreenshot screenshotter)
        {
            var path = GetOutputDirectoryFilePath("bmp");
            screenshotter.GetScreenshot().SaveAsFile(path, ImageFormat.Bmp);
            Console.Error.WriteLine("Screenshot: [\"" + path + "\"]");
        }
        
        private static void CaptureHtml(IWebDriver driver)
        {
            var path = GetOutputDirectoryFilePath("html");
            File.WriteAllText(path, driver.PageSource);
            Console.Error.WriteLine("Html: [\"" + path + "\"]");
        }

        private static string GetOutputDirectoryFilePath(string extensionWithoutPeriod)
        {

            var testName = ScenarioContext.Current.ScenarioInfo.Title;

            var dir = Path.GetFullPath(Settings.TestOutputDirectory);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = string.Format("{0}.{1}.{2}", testName, BrowserName, extensionWithoutPeriod);

            var path = Path.Combine(dir, fileName);

            return path;
        }

        private static string BrowserName { get { return _driver.GetType().Name.Replace("Driver", string.Empty); } }

        private void CleanUpBrowserInstance()
        {
            _mainWindow = _driver.WindowHandles.Contains(_mainWindow)
                              ? _mainWindow
                              : _driver.CurrentWindowHandle;

            _driver.WindowHandles.Except(new[] { _mainWindow })
                .ToList()
                .ForEach(handle =>
                {
                    _driver.SwitchTo().Window(handle);
                    _driver.Close();
                });

            _driver.Navigate().GoToUrl("about:blank");
            _driver.Manage().Cookies.DeleteAllCookies();
        }

        private static void TerminateSeleniumDrivers()
        {
            Process.GetProcessesByName("chromedriver")
                .Union(Process.GetProcessesByName("iedriverserver"))
                .ToList()
                .ForEach(p => p.Kill());
        }

        public IWebDriver WebDriver { get { return _driver; }}

        public Uri Url { get { return _iisProcess.Url; }}

    }
}