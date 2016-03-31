namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class RenderPartialHelper
    {
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model, string htmlFieldPrefix)
        {
            string str = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            htmlHelper.RenderPartial(partialViewName, model);
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = str;
        }
    }
}

