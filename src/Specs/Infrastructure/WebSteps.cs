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

        public Uri RelativeUrl()
        {
            return RelativeUrl(Browser.Url);
        }

        public Uri RelativeUrl(string absoluteUrl)
        {
            return RelativeUrl(new Uri(absoluteUrl, UriKind.Absolute));
        }

        public Uri RelativeUrl(Uri absoluteUri)
        {
            return absoluteUri.MakeRelativeUri(BaseUrl);
        }

    }
}
