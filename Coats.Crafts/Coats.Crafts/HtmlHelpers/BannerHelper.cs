using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Xml;
using System.Text;
using System.Web.Mvc.Html;
using DD4T.ContentModel;

namespace Coats.Crafts.HtmlHelpers
{
    public static class BannerHelper
    {
        /// <summary>
        /// Adds the banner.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static MvcHtmlString Banner(this HtmlHelper helper, IField bannerField, string bannerType)
        {
            //--------------------------------------------------
            //
            // This needs looking at again as banners have changed since this was first done.
            // Homepage carousel now uses second method
            //--------------------------------------------------

            string schema = string.Empty;
            
            MvcHtmlString banner = MvcHtmlString.Empty;

            if (bannerField != null)
            {

                if (bannerField.LinkedComponentValues.Count > 0)
                {
                    schema = bannerField.LinkedComponentValues[0].Schema.Title;
                }

                switch (schema)
                {
                    case "Generic.ImageText.Multi":
                        if (bannerType == "inline")
                        {
                            banner = helper.Partial("~/Views/Partials/InlineCarousel.cshtml", helper.ViewData.Model);
                        }
                        else
                        {
                            banner = helper.Partial("~/Views/Partials/FullWidthCarousel.cshtml", helper.ViewData.Model);
                        }
                        break;
                    case "Generic.Image":
                        banner = helper.Partial("~/Views/Partials/FullWidthBanner.cshtml", helper.ViewData.Model);
                        break;
                    case "Generic.ComponentGroup":
                        banner = helper.Partial("~/Views/Partials/FullWidthCarousel.cshtml", helper.ViewData.Model);
                        break;
                    case "Generic.Youtube":
                        banner = helper.Partial("~/Views/Partials/Video.cshtml", helper.ViewData.Model);
                        break;
                }
            }

            return banner;
        }


        public static MvcHtmlString Banner(this HtmlHelper helper)
        {
            MvcHtmlString banner = MvcHtmlString.Empty;

            banner = helper.Partial("~/Views/Partials/HomepageCarousel.cshtml", helper.ViewData.Model);

            return banner;
        }

    }
}