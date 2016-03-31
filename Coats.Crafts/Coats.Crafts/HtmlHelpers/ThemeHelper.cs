using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DD4T.ContentModel;

namespace Coats.Crafts.HtmlHelpers
{
    public static class ThemeHelper
    {
        /// <summary>
        /// Themes use metadata inheritance featue of DD4t - but ot avoid having to publish every page theres a Website Theme page that we check if
        /// a page doesn't have anything set.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetThemeClass(this HtmlHelper helper, IPage page)
        {
            // Check page's metadata first ..
            if (page.MetadataFields.ContainsKey("theme"))
                return page.MetadataFields["theme"].Value.ToLower();

            // Grab the "theme" page
            IPage themepage = helper.ViewContext.Controller.ViewBag.Page as IPage;
            if (themepage != null)
            {
                if (themepage.MetadataFields.ContainsKey("theme"))
                    return themepage.MetadataFields["theme"].Value.ToLower();
            }

            // If we here then no page setting, no theme page setting so nuffin!
            return String.Empty;
        }
    }
}