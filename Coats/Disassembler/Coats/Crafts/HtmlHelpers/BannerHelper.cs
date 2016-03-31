namespace Coats.Crafts.HtmlHelpers
{
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class BannerHelper
    {
        public static MvcHtmlString Banner(this HtmlHelper helper)
        {
            return helper.Partial("~/Views/Partials/HomepageCarousel.cshtml", helper.ViewData.Model);
        }

        public static MvcHtmlString Banner(this HtmlHelper helper, IField bannerField, string bannerType)
        {
            string title = string.Empty;
            MvcHtmlString empty = MvcHtmlString.Empty;
            if (bannerField == null)
            {
                return empty;
            }
            if (bannerField.LinkedComponentValues.Count > 0)
            {
                title = bannerField.LinkedComponentValues[0].Schema.Title;
            }
            string str4 = title;
            if (str4 == null)
            {
                return empty;
            }
            if (str4 != "Generic.ImageText.Multi")
            {
                if (str4 != "Generic.Image")
                {
                    if (str4 == "Generic.ComponentGroup")
                    {
                        return helper.Partial("~/Views/Partials/FullWidthCarousel.cshtml", helper.ViewData.Model);
                    }
                    if (str4 != "Generic.Youtube")
                    {
                        return empty;
                    }
                    return helper.Partial("~/Views/Partials/Video.cshtml", helper.ViewData.Model);
                }
            }
            else
            {
                if (bannerType == "inline")
                {
                    return helper.Partial("~/Views/Partials/InlineCarousel.cshtml", helper.ViewData.Model);
                }
                return helper.Partial("~/Views/Partials/FullWidthCarousel.cshtml", helper.ViewData.Model);
            }
            return helper.Partial("~/Views/Partials/FullWidthBanner.cshtml", helper.ViewData.Model);
        }
    }
}

