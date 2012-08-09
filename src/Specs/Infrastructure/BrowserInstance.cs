using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;

namespace Specs.Infrastructure
{
    public class BrowserInstance : IDisposable
    {

        private readonly IWebDriver _browser;
        private string _mainWindow;

        public BrowserInstance()
        {
            _browser = Settings.CreateWebDriver();
            _mainWindow = _browser.CurrentWindowHandle;
        }

        public void Dispose()
        {
            CloseWindows(_browser.WindowHandles);
            _browser.Dispose();
            TerminateSeleniumDrivers();
        }

        public void Reset()
        {
            _mainWindow = _browser.WindowHandles.Contains(_mainWindow)
                  ? _mainWindow
                  : _browser.CurrentWindowHandle;
            CloseWindows(_browser.WindowHandles.Except(new[] {_mainWindow}));

            _browser.Navigate().GoToUrl("about:blank");
            _browser.Manage().Cookies.DeleteAllCookies();
        }

        public IWebDriver Browser { get { return _browser; } }

        private void CloseWindows(IEnumerable<string> windowHandles)
        {
            windowHandles
                .ToList()
                .ForEach(handle =>
                             {
                                 _browser.SwitchTo().Window(handle);
                                 _browser.Close();
                             });
        }

        private static void TerminateSeleniumDrivers()
        {
            Process.GetProcessesByName("chromedriver")
                .Union(Process.GetProcessesByName("iedriverserver"))
                .ToList()
                .ForEach(p => p.Kill());
        }
    }
}
