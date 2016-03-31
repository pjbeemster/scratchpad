using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Castle.Core.Logging;

using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;

using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Exceptions;


namespace Coats.Crafts.Filters
{
    public class Level1BrandFilter : ActionFilterAttribute
    {
        private const string tcm_component_format = "tcm:{0}-{1}";

        public IAppSettings Settings { get; set; }       
        public ILogger Logger { get; set; }
        public IComponentFactory ComponentFactory { get; set; } 

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RouteData.Values.Add("Level1BrandActivated", false);
            filterContext.RouteData.Values.Add("BrandComponent", new Field());
            filterContext.RouteData.Values.Add("BrandFilter", String.Empty);
            filterContext.RouteData.Values.Add("BrandFacet", String.Empty);
            filterContext.RouteData.Values.Add("BrandFacetValue", String.Empty);
            filterContext.RouteData.Values.Add("BrandValueForSearch", String.Empty);

            if (Logger.IsInfoEnabled)
                Logger.Info("Executing brand check filter");

            var session = filterContext.HttpContext.Session;

            if (!String.IsNullOrEmpty(session.GetLevel1BrandFilter()))
            {
                string filter_string = session.GetLevel1BrandFilter();

                // We know the format of this string ...
                // facet name|facet value|Tridoin component id (without publication)
                string[] facetParts = filter_string.Split('|');
                if (facetParts.Count() < 4)
                    throw new ArgumentException("Brand filter is incorrect: + ", filter_string);

                string tcm = String.Format(tcm_component_format, Settings.PublicationId, facetParts[2]);
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Session detected, brand component id {0}", tcm);


                // get the brand component if published
                try
                {
                    IComponent brand = ComponentFactory.GetComponent(tcm);
                    if (brand != null)
                    {
                        // The title field of the brand component is a field that contains info about the brand logo
                        filterContext.RouteData.Values["Level1BrandActivated"] = true;
                        filterContext.RouteData.Values["BrandComponent"] = brand.Fields["title"];
                        filterContext.RouteData.Values["BrandFilter"] = filter_string;
                        filterContext.RouteData.Values["BrandFacet"] = facetParts[0];
                        filterContext.RouteData.Values["BrandFacetValue"] = facetParts[1];
                        filterContext.RouteData.Values["BrandValueForSearch"] = facetParts[3];

                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("BrandComponent set {0}", tcm);
                        

                    }
                }
                catch (ComponentNotFoundException)
                {
                    // no published, not exist
                    Logger.DebugFormat("Component {0} not found", tcm);                    
                }
            }
            else
            {
                if (Logger.IsInfoEnabled)
                    Logger.Info("No brand session");
            }
        }
    }

    public static class RouteDataExtensions
    {
        public static bool IsLevel1BrandFilterActivated(this RouteValueDictionary rvd)
        {           
            return (bool)rvd["Level1BrandActivated"];
        }

        public static Field GetLevel1BrandComponent(this RouteValueDictionary rvd)
        {
            return (Field)rvd["BrandComponent"];
        }

        public static string GetLevel1BrandFilter(this RouteValueDictionary rvd)
        {
            return (string)rvd["BrandFilter"];
        }

        public static string GetLevel1BrandFacet(this RouteValueDictionary rvd)
        {
            return (string)rvd["BrandFacet"];
        }

        public static string GetLevel1BrandFacetValue(this RouteValueDictionary rvd)
        {
            return (string)rvd["BrandFacetValue"];
        }

        public static string GetLevel1BrandFacetValueDelimeted(this RouteValueDictionary rvd)
        {
            return String.Format("{0}{1}{2}", (string)rvd["BrandFacet"], WebConfiguration.Current.NestedLevelDelimiter, (string)rvd["BrandFacetValue"]);            
        }

        public static string GetLevel1BrandSearchValue(this RouteValueDictionary rvd)
        {
            return (string)rvd["BrandValueForSearch"];
        }
    }
}