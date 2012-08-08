using SharpTestsEx;
using TechTalk.SpecFlow;

namespace Specs
{
    [Binding]
    public class HomePageSteps : Infrastructure.WebSteps
    {
        
        [When(@"I visit the home page")]
        public void WhenIVisitTheHomePage()
        {
            Browser.Navigate().GoToUrl(BaseUrl);
        }

        [Then(@"the page redirects to the widget list")]
        public void ThenThePageRedirectsToTheWidgetList()
        {
            RelativeUrl().ToString()
                .Should().Be.EqualTo("/widgets");
        }

    }
}
