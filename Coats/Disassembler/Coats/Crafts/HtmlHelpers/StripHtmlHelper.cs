namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public static class StripHtmlHelper
    {
        public static string StripHtml(this HtmlHelper helper, string html)
        {
            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty);
        }
    }
}

