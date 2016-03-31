namespace Coats.Crafts.HtmlHelpers
{
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public static class ThemeHelper
    {
        public static string GetThemeClass(this HtmlHelper helper, IPage page)
        {
            if (page.MetadataFields.ContainsKey("theme"))
            {
                return page.MetadataFields["theme"].Value.ToLower();
            }
            IPage page2 = ((dynamic) helper.ViewContext.Controller.ViewBag).Page as IPage;
            if ((page2 != null) && page2.MetadataFields.ContainsKey("theme"))
            {
                return page2.MetadataFields["theme"].Value.ToLower();
            }
            return string.Empty;
        }
    }
}

