namespace Coats.Crafts.HtmlHelpers
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;

    public static class ResourceHelper
    {
        public static string GetResource(this HtmlHelper helper, string key)
        {
            return (HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, key) as string);
        }

        public static string GetResource(this HtmlHelper helper, string bundle, string key)
        {
            return (HttpContext.GetGlobalResourceObject(bundle, key) as string);
        }
    }
}

