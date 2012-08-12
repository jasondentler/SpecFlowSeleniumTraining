using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenQA.Selenium;

namespace Specs.Infrastructure
{
    public class Browser : IInfrastructure
    {
        private string _mainWindow;

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Driver = new WebDriverFactory().CreateDriver(Settings.Browser);

            Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
            Driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(3));
            Driver.Manage().Cookies.DeleteAllCookies();

            _mainWindow = Driver.CurrentWindowHandle;
        }

        public void Stop()
        {
            if (Driver == null)
                return;

            CloseWindows(Driver.WindowHandles);
            Driver.Dispose();
            TerminateSeleniumDrivers();
            Driver = null;
        }

        public void Reset()
        {
            _mainWindow = Driver.WindowHandles.Contains(_mainWindow)
                  ? _mainWindow
                  : Driver.CurrentWindowHandle;
            CloseWindows(Driver.WindowHandles.Except(new[] { _mainWindow }));

            Driver.Navigate().GoToUrl("about:blank");
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        public IWebDriver Driver { get; private set; }

        private void CloseWindows(IEnumerable<string> handles)
        {
            handles
                .ToList()
                .ForEach(handle =>
                             {
                                 Driver.SwitchTo().Window(handle);
                                 Driver.Close();
                             });
        }

        private static void TerminateSeleniumDrivers()
        {
            Process.GetProcessesByName("chromedriver")
                .Union(Process.GetProcessesByName("iedriverserver"))
                .ToList()
                .ForEach(p => p.Kill());
        }

        public void TakeErrorScreenshot(string path)
        {
            var screenshotter = Driver as ITakesScreenshot;

            if (screenshotter == null)
                throw new NotSupportedException("This web driver doesn't support taking screen shots.");

            screenshotter.GetScreenshot().SaveAsFile(path, ImageFormat.Bmp);
        }

        public void CaptureHtml(string path)
        {
            File.WriteAllText(path, Driver.PageSource);
        }

    }
}
