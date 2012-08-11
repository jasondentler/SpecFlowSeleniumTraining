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
        private static BrowserInstance _browserInstance;
        private RavenInstance _ravenInstance;

        [BeforeTestRun]
        public static void Startup()
        {
            _iisProcess = new IISProcess(8082);
            _iisProcess.Start();

            StartBrowser();
        }

        [AfterTestRun]
        public static void Shutdown()
        {
            _iisProcess.Stop();
            _iisProcess.Dispose();
            StopBrowser();
        }

        [BeforeScenario("web")]
        public void Setup()
        {
            _ravenInstance = new RavenInstance();

            ScenarioContext.Current.Set(_browserInstance.Browser);
            ScenarioContext.Current.Set(_iisProcess.Url);

            _browserInstance.Reset();
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

        private static string BrowserName
        {
            get { return _browserInstance.Browser.GetType().Name.Replace("Driver", string.Empty); }
        }

        private static void StartBrowser()
        {
            _browserInstance = new BrowserInstance();
        }

        private static void StopBrowser()
        {
            _browserInstance.Dispose();
            _browserInstance = null;
        }

        public IWebDriver Browser
        {
            get { return _browserInstance.Browser; }
        }

        public Uri Url
        {
            get { return _iisProcess.Url; }
        }
    }
}