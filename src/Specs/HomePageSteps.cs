using TechTalk.SpecFlow;

namespace Specs
{
    [Binding]
    public class HomePageSteps
    {
        
        [When(@"I visit the home page")]
        public void WhenIVisitTheHomePage()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the page redirects to the widget list")]
        public void ThenThePageRedirectsToTheWidgetList()
        {
            ScenarioContext.Current.Pending();
        }

    }
}
