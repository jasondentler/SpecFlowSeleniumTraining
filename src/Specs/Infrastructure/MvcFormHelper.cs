using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using OpenQA.Selenium;

namespace Specs.Infrastructure
{
    public class MvcFormHelper<TModel> 
    {
        private readonly IWebDriver _driver;

        public MvcFormHelper(IWebDriver driver)
        {
            _driver = driver;
        }
        
        public void Set<TProperty>(Expression<Func<TModel, TProperty>> expression, TProperty value)
        {
            var element = FindElement(expression);
            element.Clear();

            if (Equals(value, null))
                return;

            element.SendKeys(value.ToString());
        }

        public void Submit<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var element = FindElement(expression);
            element.Submit();
        }

        public string FindElementName<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return MvcContrib.UI.InputBuilder.Helpers.ReflectionHelper.BuildNameFrom(expression);
        }

        public string FindElementId<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return HtmlHelper.GenerateIdFromName(FindElementName(expression));
        }

        public IWebElement FindElement<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return _driver.FindElement(By.Id(FindElementId(expression)));
        }

    }
}
