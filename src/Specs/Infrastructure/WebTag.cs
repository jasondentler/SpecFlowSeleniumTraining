using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Specs.Infrastructure
{
    [Binding]
    public class WebTag
    {

        private static readonly IISExpress IISExpressInstance = new IISExpress();
        private static readonly Raven RavenInstance = new Raven();
        private static readonly Browser BrowserInstance = new Browser();

        private static IEnumerable<IInfrastructure> AllInfrastructure()
        {
            yield return IISExpressInstance;
            yield return RavenInstance;
            yield return BrowserInstance;
        }


        [BeforeScenario("web")]
        public void Setup()
        {
            AllInfrastructure().ToList().ForEach(i => i.Start());
            ScenarioContext.Current.Set(BrowserInstance.Driver);
            ScenarioContext.Current.Set<Uri>(IISExpressInstance.Url);
        }

        [AfterScenario("web")]
        public void AfterWebScenario()
        {
            CaptureErrorInformation();
            AllInfrastructure().ToList().ForEach(i => i.Stop());
        }

        private static void CaptureErrorInformation()
        {
            var ctx = ScenarioContext.Current;
            if (ctx.TestError == null)
                return;

            var driver = ScenarioContext.Current.Get<IWebDriver>();

            CaptureHtml(driver);


        }

        private static void CaptureScreenshot()
        {
            var path = GetOutputDirectoryFilePath("bmp");
            BrowserInstance.TakeErrorScreenshot(path);
            Console.Error.WriteLine("Screenshot: [\"" + path + "\"]");
        }

        private static void CaptureHtml(IWebDriver driver)
        {
            var path = GetOutputDirectoryFilePath("html");
            BrowserInstance.CaptureHtml(path);
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
            get { return BrowserInstance.Driver.GetType().Name.Replace("Driver", string.Empty); }
        }

    }
}