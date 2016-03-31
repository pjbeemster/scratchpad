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

namespace Coats.Crafts.HtmlHelpers
{
    public static class AddCommentCountHelper
    {

        /// <summary>
        /// Adds the banner.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static string addCommentCount(this HtmlHelper helper)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(helper.Partial("~/Views/Partials/FullWidthCarousel.cshtml", helper.ViewData.Model));
            return builder.ToString();

        }
    }
}