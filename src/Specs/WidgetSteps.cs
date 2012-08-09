using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
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
            CreateWidget();
        }

        [Given(@"I have added two widgets")]
        public void GivenIHaveAddedTwoWidgets()
        {
            CreateWidget();
            CreateWidget();
        }

        [When(@"I create a widget")]
        public void WhenICreateAWidget()
        {
            CreateWidget();
        }

        [When(@"I display the widget")]
        public void WhenIDisplayTheWidget()
        {
            var widget = _widgets.Single();
            var url = string.Format("/widgets/{0}", widget.Id);
            NavigateTo(url);
        }

        [When(@"I view the widget list")]
        public void WhenIViewTheWidgetList()
        {
            NavigateTo("/widgets");
        }

        [When(@"I display a widget that doesn't exist")]
        public void WhenIDisplayAWidgetThatDoesnTExist()
        {
            NavigateTo("/widgets/-1");
        }

        [Then(@"the widget details are displayed")]
        [Then(@"the widget is displayed")]
        public void ThenTheWidgetIsDisplayed()
        {
            var widget = _widgets.Single();

            var expectedUrl = string.Format(@"widgets/{0}", widget.Id);
            RelativeUrl().ToString().Should().Be.EqualTo(expectedUrl);

            var form = new MvcFormHelper<WidgetDetails>(Browser);

            form.Get(m => m.Name).Should().Be.EqualTo(widget.Name);

            Math.Abs(form.Get(m => m.Size) - widget.Size)
                .Should().Be.LessThan(Epsilon);
        }

        [Then(@"the error message is ""(.*)""")]
        public void ThenTheErrorMessageIs(string expected)
        {
            var actual = Browser.FindElement(By.ClassName("error")).Text;
            actual.Should().Be.EqualTo(expected);
        }

        [Then(@"two widgets are listed")]
        public void ThenTwoWidgetsAreListed()
        {
            var expected = _widgets.ToDictionary(widget => widget.Id,
                                                 widget => widget.Name);

            var widgetLinks = Browser.FindElements(By.XPath(""));

            var getIdFromUrl = new Func<string, int>(url =>
                                                         {
                                                             var regex = new Regex(@"/widgets/(?<id>\d+)$");
                                                             return Convert.ToInt32(regex.Match(url).Groups["id"].Value);
                                                         });

            var actual = widgetLinks.ToDictionary(
                link => getIdFromUrl(link.GetAttribute("href")),
                link => link.Text);

            actual.Should().Have.SameValuesAs(expected);
        }

        private readonly List<WidgetDetails> _widgets = new List<WidgetDetails>();
        private const float Epsilon = 0.0001F;

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
            form.Set(m => m.Name, widget.Name);
            form.Set(m => m.Size, widget.Size);
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
