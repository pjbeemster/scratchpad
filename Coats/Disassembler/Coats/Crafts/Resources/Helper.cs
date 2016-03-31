namespace Coats.Crafts.Resources
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Web;

    public static class Helper
    {
        public static string GetResource(string key)
        {
            return (HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, key) as string);
        }

        public static string GetResource(string bundle, string key)
        {
            return (HttpContext.GetGlobalResourceObject(bundle, key) as string);
        }
    }
}

