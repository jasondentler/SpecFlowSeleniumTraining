using System;
using OpenQA.Selenium;
using SharpTestsEx;
using Specs.Infrastructure;
using TechTalk.SpecFlow;

namespace Specs
{
    [Binding]
    public class WidgetSteps : WebSteps 
    {

        [Given(@"I have added one widget")]
        [Given(@"I have added a widget")]
        public void GivenIHaveAddedAWidget()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"I have added two widgets")]
        public void GivenIHaveAddedTwoWidgets()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I create a widget")]
        public void WhenICreateAWidget()
        {
            CreateWidget();
        }

        [When(@"I display the widget")]
        public void WhenIDisplayTheWidget()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I view the widget list")]
        public void WhenIViewTheWidgetList()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I display a widget that doesn't exist")]
        public void WhenIDisplayAWidgetThatDoesnTExist()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the widget details are displayed")]
        [Then(@"the widget is displayed")]
        public void ThenTheWidgetIsDisplayed()
        {
            RelativeUrl().ToString()
                .Should().Match(@"/widgets/\d+");
        }

        [Then(@"the error message is ""(.*)""")]
        public void ThenTheErrorMessageIs(string p0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"two widgets are listed")]
        public void ThenTwoWidgetsAreListed()
        {
            ScenarioContext.Current.Pending();
        }

        private void CreateWidget()
        {
            Browser.Navigate().GoToUrl("/widgets/create");
            Browser.FindElement(By.Name("Name")).SendKeys(Guid.NewGuid().ToString());
            Browser.FindElement(By.Name("Size")).SendKeys("34.6");
            Browser.FindElement(By.Name("Submit")).Click();
        }

    }
}
