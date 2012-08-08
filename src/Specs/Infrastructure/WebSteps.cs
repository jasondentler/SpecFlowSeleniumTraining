using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Specs.Infrastructure
{
    public abstract class WebSteps
    {

        public IWebDriver Browser { get { return GetWebConfiguration().WebDriver; } }
        public Uri BaseUrl { get { return GetWebConfiguration().Url; } }

        private WebConfiguration GetWebConfiguration()
        {
            return (WebConfiguration) ScenarioContext.Current.GetBindingInstance(typeof (WebConfiguration));
        }

    }
}
