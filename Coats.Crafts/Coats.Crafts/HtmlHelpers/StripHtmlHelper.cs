using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Coats.Crafts.HtmlHelpers
{
    public static class StripHtmlHelper
    {
        public static String StripHtml(this HtmlHelper helper, string html)
        {
            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty);
        }
    }
}