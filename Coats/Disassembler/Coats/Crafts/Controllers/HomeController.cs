namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.CustomOutputCache;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.FredHopper;
    using Coats.Crafts.Models;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    [FredHopperOutputCache(CacheProfile="Homepage")]
    public class HomeController : Controller
    {
        protected IAppSettings _settings;

        public HomeController(IAppSettings settings)
        {
            this._settings = settings;
        }

        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            string str2;
            Stopwatch stopwatch = new Stopwatch();
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Start();
            }
            ((dynamic) base.ViewBag).PodGenericSpan = "span4";
            string str = base.Request.QueryString["fh_params"];
            Query query = null;
            HomePageBannerContent model = new HomePageBannerContent {
                Banner = componentPresentation
            };
            if (!string.IsNullOrEmpty(str))
            {
                query = new Query();
                query.ParseQuery(str);
            }
            else
            {
                Location location = new Location(this.DefaultLocation);
                Criterion criterion = HomePageTabsConfig.ToCriteria(this._settings.PublicationId);
                location.addCriterion(criterion);
                query = new Query(location);
            }
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);
            int viewsize = Convert.ToInt32(WebConfiguration.Current.HomepageItemsPerPage);
            viewsize = FacetedContentHelper.AssertCappedViewSize(viewsize);
            query.setListViewSize(viewsize);
            query.setSortingBy(WebConfiguration.Current.HomepageSort);
            DD4TComponents components = new DD4TComponents(this.Logger);
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Stop();
                str2 = stopwatch.Elapsed.ToString();
                this.Logger.DebugFormat("HomeController Index time elapsed 1:" + str2, new object[0]);
            }
            model.FredHopperComponents = components.GetComponents(query.toString(), true);
            model.Tabs = HomePageTabsConfig.ToSelectList(query.toString());
            IList<IComponentPresentation> ctaList = (from m in componentPresentation.Page.ComponentPresentations
                where m.ComponentTemplate.Id != componentPresentation.ComponentTemplate.Id
                select m).ToList<IComponentPresentation>();
            model.FredHopperComponents = FacetedContentHelper.InjectCTAComponents(model.FredHopperComponents, ctaList, false, false);
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Stop();
                str2 = stopwatch.Elapsed.ToString();
                this.Logger.DebugFormat("HomeController Index time elapsed 2:" + str2, new object[0]);
            }
            return base.View(model);
        }

        protected string BaseLocation
        {
            get
            {
                return FredHopperInterface.GetPublicationPath(this._settings.PublicationId);
            }
        }

        protected string DefaultLocation
        {
            get
            {
                return string.Format("{0}/{1}_promote_on_homepage>{{{1}_promote_on_homepage{2}yes}}/", this.BaseLocation, this._settings.PublicationId, FacetedContentHelper.NestedLevelDelimiter);
            }
        }

        public ILogger Logger { get; set; }
    }
}

