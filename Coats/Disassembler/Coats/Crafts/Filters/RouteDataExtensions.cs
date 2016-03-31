namespace Coats.Crafts.Filters
{
    using Coats.Crafts.Configuration;
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Routing;

    public static class RouteDataExtensions
    {
        public static Field GetLevel1BrandComponent(this RouteValueDictionary rvd)
        {
            return (Field) rvd["BrandComponent"];
        }

        public static string GetLevel1BrandFacet(this RouteValueDictionary rvd)
        {
            return (string) rvd["BrandFacet"];
        }

        public static string GetLevel1BrandFacetValue(this RouteValueDictionary rvd)
        {
            return (string) rvd["BrandFacetValue"];
        }

        public static string GetLevel1BrandFacetValueDelimeted(this RouteValueDictionary rvd)
        {
            return string.Format("{0}{1}{2}", (string) rvd["BrandFacet"], WebConfiguration.Current.NestedLevelDelimiter, (string) rvd["BrandFacetValue"]);
        }

        public static string GetLevel1BrandFilter(this RouteValueDictionary rvd)
        {
            return (string) rvd["BrandFilter"];
        }

        public static string GetLevel1BrandSearchValue(this RouteValueDictionary rvd)
        {
            return (string) rvd["BrandValueForSearch"];
        }

        public static bool IsLevel1BrandFilterActivated(this RouteValueDictionary rvd)
        {
            return (bool) rvd["Level1BrandActivated"];
        }
    }
}

