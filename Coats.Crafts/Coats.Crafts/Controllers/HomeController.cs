using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Configuration;
using DD4T.ContentModel;
using com.fredhopper.lang.query;
using Coats.Crafts.Models;
using Coats.Crafts.Extensions;
using Coats.Crafts.FredHopper;
using Coats.Crafts.ControllerHelpers;

using Castle.Core.Logging;
using System;
using Coats.Crafts.CustomOutputCache;
using System.Diagnostics;

namespace Coats.Crafts.Controllers
{
    [FredHopperOutputCache(CacheProfile = "Homepage")]
    public class HomeController : Controller
    {
        public ILogger Logger { get; set; }

        protected IAppSettings _settings;

        protected string BaseLocation
        {
            get
            {
                //e.g. //catalog01/en_US/publicationid=tcm_0_70_1
                return FredHopperInterface.GetPublicationPath(_settings.PublicationId);
            }
        }

		//protected string BaseQueryEncoded
		//{
		//    get
		//    {
		//        //e.g. //catalog01/en_US/publicationid=tcm_0_70_1
		//        return "?fh_params=" + HttpUtility.UrlEncode("fh_location=" + BaseLocation + "/");
		//    }
		//}

        protected string DefaultLocation
        {
            get
            {
                //e.g. //catalog01/en_US/publicationid=tcm_0_70_1/70_techniques>{70_techniques__dir__knitting}
                //return BaseLocation + "/70_promote_on_homepage>{70_promote_on_homepage__yes}/";
                //return BaseLocation + "/promote_on_homepage>{promote_on_homepage" + FacetedContentHelper.NestedLevelDelimiter + "yes}/";
                string promoteFacet = string.Format("{0}/{1}_promote_on_homepage>{{{1}_promote_on_homepage{2}yes}}/",
                    BaseLocation,
                    _settings.PublicationId,
                    FacetedContentHelper.NestedLevelDelimiter);
                
                return promoteFacet;
            }
        }

        public HomeController(IAppSettings settings)
        {
            _settings = settings;
        }

        //
        // GET: /Product-Explorer/
        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            Stopwatch sw = new Stopwatch();

            if (Logger.IsDebugEnabled)
            {
                sw.Start();
            }

            ViewBag.PodGenericSpan = "span4";
            string fh_params = Request.QueryString["fh_params"];
            Query query = null;

            HomePageBannerContent content = new HomePageBannerContent();
            content.Banner = componentPresentation;

            // Create query...

            // Check if the call includes query string params or not
            if (!string.IsNullOrEmpty(fh_params))
            {
                // Parse the query string params into the facetedContent object
                query = new com.fredhopper.lang.query.Query();
                query.ParseQuery(fh_params);
            }
            else
            {
                // No query string params, so use default location
                com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(DefaultLocation);
                var crit = HomePageTabsConfig.ToCriteria(_settings.PublicationId);
                loc.addCriterion(crit);                
                query = new com.fredhopper.lang.query.Query(loc);
            }

            // Set the view type to lister
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);

            // Set number of items per page
			query.setListViewSize(Convert.ToInt32(WebConfiguration.Current.HomepageItemsPerPage));

            // Set order by
            //query.setSortingBy("rating", SortDirection.DESC);
            //query.setSortingBy("commentcount", SortDirection.DESC);
			query.setSortingBy(WebConfiguration.Current.HomepageSort);

            // Fire off to FredHopper...
            DD4TComponents dd4t = new DD4TComponents(Logger);

            if (Logger.IsDebugEnabled)
            {
                sw.Stop();
                string timer = sw.Elapsed.ToString();
                Logger.DebugFormat("HomeController Index time elapsed 1:" + timer);
            }

            // ASH: Make sure we get the additional fields such as commentcount, rating, etc, by passing in "true"
            //      for the "useWebConfigExtendedPropertiesList" param.
            //content.FredHopperComponents = dd4t.GetComponents(query.toString(), false);
            content.FredHopperComponents = dd4t.GetComponents(query.toString(), true);

            // Create the top tab list
            //string prefix = "?fh_params=" + HttpUtility.UrlEncode("fh_location=" + BaseLocation + "/");
            //content.Tabs = HomePageTabsConfig.ToSelectList(query.toString());
            content.Tabs = HomePageTabsConfig.ToSelectList(query.toString());

            // Inject the additional component presentations (should be "Get Involved" and "Find a Store")
            IList<IComponentPresentation> ctaList = componentPresentation.Page.ComponentPresentations.Where(m => m.ComponentTemplate.Id != componentPresentation.ComponentTemplate.Id).ToList();
            content.FredHopperComponents = FacetedContentHelper.InjectCTAComponents(content.FredHopperComponents, ctaList, false, false);

            if (Logger.IsDebugEnabled)
            {
                sw.Stop();
                string timer = sw.Elapsed.ToString();
                Logger.DebugFormat("HomeController Index time elapsed 2:" + timer);
            }

            return View(content);
        }

    }
}
