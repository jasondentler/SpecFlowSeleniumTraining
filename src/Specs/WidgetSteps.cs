using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SharpTestsEx;
using Specs.Infrastructure;
using TechTalk.SpecFlow;
using UI.Models;

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
            var expectedUrl = string.Format(@"widgets/{0}", _widgets.Single().Id);
            RelativeUrl().ToString().Should().Be.EqualTo(expectedUrl);
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

        private readonly List<WidgetDetails> _widgets = new List<WidgetDetails>();

        private void CreateWidget()
        {
            CreateWidget(new WidgetDetails()
                             {
                                 Name = Guid.NewGuid().ToString(),
                                 Size = Convert.ToSingle(new Random().NextDouble()*50)
                             });
        }

        private void CreateWidget(WidgetDetails widget)
        {
            NavigateTo("/widgets/create");
            var form = new MvcFormHelper<WidgetDetails>(Browser);
            form.Set(m => m.Name, Guid.NewGuid().ToString());
            form.Set(m => m.Size, 34.6);
            form.Submit(m => m.Name);

            var match = new Regex(@"^widgets/(?<id>\d+)$").Match(RelativeUrl().ToString());
            if (match.Success)
            {
                widget.Id = Convert.ToInt32(match.Groups["id"].Value);
                _widgets.Add(widget);
            }
        }
    }
}
