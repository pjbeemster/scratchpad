namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.Models;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public class FacetedContentController : FacetedContentBaseController
    {
        public FacetedContentController(IAppSettings settings, IComponentFactory componentFactory) : base(settings, componentFactory)
        {
        }

        [Level1BrandFilter]
        public ActionResult Index(FacetedContent facetedContent, IComponentPresentation componentPresentation, bool clear = false)
        {
            Func<IComponentPresentation, bool> predicate = null;
            Stopwatch stopwatch = new Stopwatch();
            if (base.Logger.IsDebugEnabled)
            {
                stopwatch.Start();
            }
            if (clear)
            {
                if (base.Logger.IsDebugEnabled)
                {
                    base.Logger.Debug("Clearing session");
                }
                base.RouteData.Values["Level1BrandActivated"] = false;
                base.RouteData.Values["BrandComponent"] = new Field();
                base.RouteData.Values["BrandFilter"] = string.Empty;
                base.RouteData.Values["BrandFacet"] = string.Empty;
                base.RouteData.Values["BrandFacetValue"] = string.Empty;
                base.RouteData.Values["BrandValueForSearch"] = string.Empty;
                base.Session.ClearLevel1BrandFilter();
            }
            this.SetViewBag(componentPresentation);
            Query query = new Query();
            string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
            string nonSelected = string.Empty;
            if (base.Request.QueryString["events"] != null)
            {
                nonSelected = "crafts2eblog";
            }
            if (base.Request.HttpMethod != "POST")
            {
                if (base.RouteData.Values.IsLevel1BrandFilterActivated())
                {
                    facetedContent.Brand = base.RouteData.Values.GetLevel1BrandComponent();
                    if (!string.IsNullOrEmpty(base.Request.QueryString["fh_params"]))
                    {
                        base.CheckQueryContainsBrand(base.Request.QueryString["fh_params"]);
                    }
                }
                query = FacetedContentHelper.BuildQuery<FacetedContent>(ref facetedContent, base.Request.QueryString["fh_params"], viewType, this.ItemsPerPage, this.DefaultLocation, base._settings.PublicationId, base.RouteData.Values.GetLevel1BrandFilter(), nonSelected);
            }
            else if (base.Request.HttpMethod == "POST")
            {
                query = FacetedContentHelper.BuildQuery<FacetedContent>(ref facetedContent, this.ItemsPerPage, viewType, base._settings.PublicationId);
            }
            base.Logger.Debug(string.Format("FacetedContentController: Session[\"BRAND_FILTER\"] = {0}", base.RouteData.Values.GetLevel1BrandFilter()));
            base.Logger.Debug(string.Format("FacetedContentController: Contructed query = {0}", query.toString()));
            bool result = false;
            if (!bool.TryParse(base.Request.QueryString["history"], out result))
            {
                result = false;
            }
            facetedContent = this.CallFredHopperAndPopulate<FacetedContent>(query, facetedContent, result);
            facetedContent.ComponentSection.ComponentTypes = FacetedContentHelper.GetComponentSchemaTypes(viewType, true, "", facetedContent);
            if (facetedContent.Sort == null)
            {
                facetedContent.Sort = new FacetSort();
            }
            facetedContent.Sort.SortBy = FacetedContentHelper.GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, base._settings.PublicationId);
            facetedContent.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, viewType, base._settings.PublicationId);
            facetedContent.Sort.ActiveDiscussionsUrl = FacetedContentHelper.GetActiveDiscussionUrl(query);
            IField introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];
            if (facetedContent.ComponentList.Components.Count > 0)
            {
                facetedContent = FacetedContentHelper.InjectIntroComponent<FacetedContent>(facetedContent, introMarkup, result);
                if (predicate == null)
                {
                    predicate = m => m.ComponentTemplate.Id != componentPresentation.ComponentTemplate.Id;
                }
                IList<IComponentPresentation> ctaList = componentPresentation.Page.ComponentPresentations.Where<IComponentPresentation>(predicate).ToList<IComponentPresentation>();
                bool randomInsert = true;
                bool historyBack = result;
                facetedContent = FacetedContentHelper.InjectCTAComponents<FacetedContent>(facetedContent, ctaList, randomInsert, historyBack);
            }
            facetedContent.ComponentSection.ComponentTypes.SetHrefLinks(query);
            if (base.Logger.IsDebugEnabled)
            {
                stopwatch.Stop();
                string str3 = stopwatch.Elapsed.ToString();
                base.Logger.DebugFormat("FacetedContentController.Index time elapsed:" + str3, new object[0]);
            }
            if (!string.IsNullOrEmpty(base._viewNameOverride))
            {
                return base.View(base._viewNameOverride, facetedContent);
            }
            return base.View(facetedContent);
        }

        protected override Query PreCallFredHopper<T>(Query query, ref T facetedContent) //where T: FacetedContentBase
        {
            Location location = query.getLocation();
            MultiValuedCriterion criterion = CriterionFactory.parse(facetedContent.ComponentSection.ComponentTypes.ToLocationString()) as MultiValuedCriterion;
            if ((criterion != null) && !criterion.getGreaterThan().isEmpty())
            {
                location.removeCriteria("schematitle");
                location.addCriterion(criterion);
                query.setLocation(location);
            }
            return query;
        }

        private void SetViewBag(IComponentPresentation componentPresentation)
        {
            ((dynamic)base.ViewBag).ShowActiveDiscussionFilter = true;
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value;
            }
            catch (Exception)
            {
            }
            ((dynamic) base.ViewBag).PodGenericSpan = "span4";
        }
    }
}

