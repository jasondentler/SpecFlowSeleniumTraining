using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace Specs
{
    public class WidgetSteps
    {


        private int GetIdFromDetailsLink(IWebElement anchorTag)
        {
            var url = anchorTag.GetAttribute("href");
            var regex = new Regex(@"/widgets/(?<id>\d+)$");
            var sid = regex.Match(url).Groups["id"].Value;
            return Convert.ToInt32(sid);
        }

    }
}
