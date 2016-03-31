namespace Coats.Crafts.Filters
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Exceptions;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;

    public class Level1BrandFilter : ActionFilterAttribute
    {
        private const string tcm_component_format = "tcm:{0}-{1}";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RouteData.Values.Add("Level1BrandActivated", false);
            filterContext.RouteData.Values.Add("BrandComponent", new Field());
            filterContext.RouteData.Values.Add("BrandFilter", string.Empty);
            filterContext.RouteData.Values.Add("BrandFacet", string.Empty);
            filterContext.RouteData.Values.Add("BrandFacetValue", string.Empty);
            filterContext.RouteData.Values.Add("BrandValueForSearch", string.Empty);
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.Info("Executing brand check filter");
            }
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (!string.IsNullOrEmpty(session.GetLevel1BrandFilter()))
            {
                string paramName = session.GetLevel1BrandFilter();
                string[] source = paramName.Split(new char[] { '|' });
                if (source.Count<string>() < 4)
                {
                    throw new ArgumentException("Brand filter is incorrect: + ", paramName);
                }
                string componentUri = string.Format("tcm:{0}-{1}", this.Settings.PublicationId, source[2]);
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("Session detected, brand component id {0}", new object[] { componentUri });
                }
                try
                {
                    IComponent component = this.ComponentFactory.GetComponent(componentUri);
                    if (component != null)
                    {
                        filterContext.RouteData.Values["Level1BrandActivated"] = true;
                        filterContext.RouteData.Values["BrandComponent"] = component.Fields["title"];
                        filterContext.RouteData.Values["BrandFilter"] = paramName;
                        filterContext.RouteData.Values["BrandFacet"] = source[0];
                        filterContext.RouteData.Values["BrandFacetValue"] = source[1];
                        filterContext.RouteData.Values["BrandValueForSearch"] = source[3];
                        if (this.Logger.IsDebugEnabled)
                        {
                            this.Logger.DebugFormat("BrandComponent set {0}", new object[] { componentUri });
                        }
                    }
                }
                catch (ComponentNotFoundException)
                {
                    this.Logger.DebugFormat("Component {0} not found", new object[] { componentUri });
                }
            }
            else if (this.Logger.IsInfoEnabled)
            {
                this.Logger.Info("No brand session");
            }
        }

        public IComponentFactory ComponentFactory { get; set; }

        public ILogger Logger { get; set; }

        public IAppSettings Settings { get; set; }
    }
}

