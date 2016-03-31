using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Coats.Crafts.Configuration;

namespace Coats.Crafts.HtmlHelpers
{
    public static class ResourceHelper
    {
        public static string GetResource(this HtmlHelper helper, string bundle, string key)
        {
            return HttpContext.GetGlobalResourceObject(bundle, key) as string;
        }

        public static string GetResource(this HtmlHelper helper, string key)
        {
            return HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, key) as string;
        }
    }
}