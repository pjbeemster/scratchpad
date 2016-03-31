using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Web;
using System.Xml;

using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location.criteria;
using com.fredhopper.lang.query.util;

using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Exceptions;

using Castle.Core.Logging;

using Coats.Crafts.Models;
using Coats.Crafts.FredHopper;
using Coats.Crafts.FASWebService;
using Coats.Crafts.Extensions;
using Coats.Crafts.Configuration;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.CustomOutputCache;
using Coats.Crafts.Filters;

namespace Coats.Crafts.Controllers
{
    public class FacetedContentController : FacetedContentBaseController
    {
        public FacetedContentController(IAppSettings settings, IComponentFactory componentFactory) : base(settings, componentFactory)
        {
        }

        private void SetViewBag(IComponentPresentation componentPresentation)
        {
            ViewBag.ShowActiveDiscussionFilter = true;

            try { ViewBag.Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value; }
            catch (Exception) { }

            ViewBag.PodGenericSpan = "span4";
        }

        //
        // GET: /FacetedContent/
        [Level1BrandFilter]
        public ActionResult Index(FacetedContent facetedContent, IComponentPresentation componentPresentation, bool clear = false)
        {
            Stopwatch sw = new Stopwatch();

            if (Logger.IsDebugEnabled)
            {
                sw.Start();
            }

            if (clear)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("Clearing session");

                // Not nice but effective
                RouteData.Values["Level1BrandActivated"] = false;
                RouteData.Values["BrandComponent"] = new Field();
                RouteData.Values["BrandFilter"] = String.Empty;
                RouteData.Values["BrandFacet"] = String.Empty;
                RouteData.Values["BrandFacetValue"] = String.Empty;
                RouteData.Values["BrandValueForSearch"] = String.Empty;

                Session.ClearLevel1BrandFilter();
            }

            SetViewBag(componentPresentation);

            com.fredhopper.lang.query.Query query = new Query();
            string facetViewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;


            string nonSelected = string.Empty;
            //little bit of a fudge.  We need a way of only displaying events (from the "display all events" link)
            //so we look for the events querystring and de-select the blogs option
            if (Request.QueryString["events"] != null)
            {
                nonSelected = "crafts2eblog";
            }
 
            // Only doing this because BuildQuery differs if GET or POST
            if (Request.HttpMethod != "POST")
            {
                if (RouteData.Values.IsLevel1BrandFilterActivated())
                {
                    facetedContent.Brand = RouteData.Values.GetLevel1BrandComponent();
                    // Check if the level 1 brand is NOT in this current query, if so, clear the session
                    if (!String.IsNullOrEmpty(Request.QueryString["fh_params"]))
                    {
                        base.CheckQueryContainsBrand(Request.QueryString["fh_params"]);
                    }
                }

                query = FacetedContentHelper.BuildQuery(ref facetedContent,
                                                        Request.QueryString["fh_params"],
                                                        facetViewType,
                                                        ItemsPerPage,
                                                        DefaultLocation,
                                                        _settings.PublicationId,
                                                        RouteData.Values.GetLevel1BrandFilter(), // This will be empty if not set anyway
                                                        nonSelected);
            }
            else if (Request.HttpMethod == "POST")
            {
                query = FacetedContentHelper.BuildQuery(ref facetedContent, ItemsPerPage, facetViewType, _settings.PublicationId);
            }

            Logger.Debug(string.Format("FacetedContentController: Session[\"BRAND_FILTER\"] = {0}", RouteData.Values.GetLevel1BrandFilter()));
            Logger.Debug(string.Format("FacetedContentController: Contructed query = {0}", query.toString()));


            #region Removed
            //if (Session["BRAND_FILTER"] != null)
            //{
            //    #region "Handle Brand Overload Code"

            //    string[] facetParts = Session["BRAND_FILTER"].ToString().Split('|');
            //    string tcm = facetParts[2].Replace("##", _settings.PublicationId.ToString());

            //    // get the brand component if published
            //    try
            //    {
            //        IComponent brand = _componentFactory.GetComponent(tcm);
            //        if (brand != null)
            //            facetedContent.Brand = brand.Fields["title"];
            //    }
            //    catch (ComponentNotFoundException)
            //    {
            //        // no published, not exist
            //        Logger.DebugFormat("Component {0} not found", tcm);
            //        facetedContent.Brand = null;
            //    }

            //    // check FH_PARAMS brand filter has been manually removed
            //    if (Request.QueryString["fh_params"] != null)
            //    {
            //        // check for brand filter absence in FH_PARAMS
            //        var queryTest = FacetedContentHelper.BuildQuery(ref facetedContent,
            //                                                        Request.QueryString["fh_params"],
            //                                                        componentPresentation.ComponentTemplate.MetadataFields["view"].Value,
            //                                                        ItemsPerPage,
            //                                                        DefaultLocation);
            //        if (!queryTest.getLocation().getCriteria(facetParts[0]).contains(facetParts[1]))
            //            Session.Remove("BRAND_FILTER");
            //    }

            //    // brand filter exists?
            //    if (Session["BRAND_FILTER"] != null)
            //    {
            //        string brandFilter = Session["BRAND_FILTER"].ToString();
            //        query = FacetedContentHelper.BuildQuery(ref facetedContent,
            //                                                Request.QueryString["fh_params"],
            //                                                componentPresentation.ComponentTemplate.MetadataFields["view"].Value,
            //                                                ItemsPerPage,
            //                                                DefaultLocation,
            //                                                optionalFacet: brandFilter);
            //        Logger.Debug(string.Format("FacetedContentController: Session[\"BRAND_FILTER\"] = {0}", brandFilter)); 
            //        Logger.Debug(string.Format("FacetedContentController: Contructed query = {0}", query.toString())); 
            //    }
            //    else
            //        query = FacetedContentHelper.BuildQuery(ref facetedContent,
            //                                                Request.QueryString["fh_params"],
            //                                                componentPresentation.ComponentTemplate.MetadataFields["view"].Value,
            //                                                ItemsPerPage,
            //                                                DefaultLocation);

            //    #endregion
            //}
            //else
            //{
            //    //string facetViewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;

            //    string nonSelected = string.Empty;

            //    //little bit of a fudge.  We need a way of only displaying events (from the "display all events" link)
            //    //so we look for the events querystring and de-select the blogs option
            //    if (Request.QueryString["events"] != null)
            //    {
            //        nonSelected = "crafts2eblog";
            //    }

            //    query = FacetedContentHelper.BuildQuery(ref facetedContent,
            //                                            Request.QueryString["fh_params"],
            //                                            facetViewType,
            //                                            ItemsPerPage,
            //                                            DefaultLocation, "", nonSelected);
            //}
#endregion

            //com.fredhopper.lang.query.Query query = BuildQuery(ref facetedContent, Request.QueryString["fh_params"], componentPresentation.ComponentTemplate.MetadataFields["view"].Value);

            // Only call FredHopper if there is at least one component section selected (e.g. Projects & patterns, Articles, Moodboards, Designers, etc)
            bool historyBack = false;
            //int selectedCount = facetedContent.ComponentSection.ComponentTypes.Facets.Count(m => m.Selected);
            //if (selectedCount > 0)
            //{
                // Is this request from a history back link?
                if (!bool.TryParse(Request.QueryString["history"], out historyBack))
                {
                    historyBack = false;
                }

                facetedContent = CallFredHopperAndPopulate(query, facetedContent, historyBack);
                
                //now that we have all the content we should update the schema type checkboxes, disabling ones with 0 results
                facetedContent.ComponentSection.ComponentTypes = FacetedContentHelper.GetComponentSchemaTypes(facetViewType, true, "", facetedContent);

                // Removed this as ComponentTypes now correctlt set from proceeding call
                //facetedContent.ComponentSection.ComponentTypes.Facets = query.ParseSelectedSchemaTypes(facetedContent.ComponentSection.ComponentTypes.Facets);
            //}
            //else
            //{
            //    #region Navigational Items

            //    // Ensure Pagingation (if no selected components)
            //    facetedContent.ComponentList = new PaginatedComponentList();
            //    facetedContent.ComponentList.Pagination = new List<FacetPagination>();
            //    facetedContent.ComponentList.Pagination.Add(new FacetPagination { Url = "#", LinkText = "1", Selected = true });

            //    // Ensure facet map (if no selected components)
            //    facetedContent.FacetMap = new FacetCollection();

            //    #endregion

            //    if (facetedContent.ComponentList.Components == null)
            //    {
            //        facetedContent.ComponentList.Components = new List<Component>();
            //    }
            //}

            // Create the "Sort by" select list
            if (facetedContent.Sort == null) { facetedContent.Sort = new FacetSort(); }
            facetedContent.Sort.SortBy = FacetedContentHelper.GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), facetViewType, _settings.PublicationId);
            facetedContent.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, facetViewType, _settings.PublicationId);
            facetedContent.Sort.ActiveDiscussionsUrl = FacetedContentHelper.GetActiveDiscussionUrl(query);

            //// Create the "Sort by" select list
            //string sortingAttribute = query.getSortingAttribute(0);
            //if (facetedContent.Sort == null) { facetedContent.Sort = new FacetSort(); }
            //facetedContent.Sort.SortByList = FacetedContentHelper.CreateSortByList(sortingAttribute, query.getSortingDirection(0), query, out sortingAttribute);


            var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];

            //old functionality was to inject additional components even if we didn't have any results
            //we now only do that now if we have faceted content results 
            if (facetedContent.ComponentList.Components.Count > 0)
            {
                facetedContent = FacetedContentHelper.InjectIntroComponent(facetedContent, introMarkup, historyBack);

                IList<IComponentPresentation> ctaList = componentPresentation.Page.ComponentPresentations.Where(m => m.ComponentTemplate.Id != componentPresentation.ComponentTemplate.Id).ToList();
                facetedContent = FacetedContentHelper.InjectCTAComponents(facetedContent, ctaList, randomInsert: true, historyBack: historyBack);
            }

            //Session["FACETED_CONTENT"] = facetedContent;

            // I would have preferred to be encapsulated, but it all relies on the final query string.
            facetedContent.ComponentSection.ComponentTypes.SetHrefLinks(query);

            if (Logger.IsDebugEnabled)
            {
                sw.Stop();
                string timer = sw.Elapsed.ToString();
                Logger.DebugFormat("FacetedContentController.Index time elapsed:" + timer);
            }

            if (!string.IsNullOrEmpty(_viewNameOverride))
            {
                return View(_viewNameOverride, facetedContent);
            }
            else
            {
                return View(facetedContent);
            }
        }


        //[HttpPost, Level1BrandFilter]
        //public ActionResult Index(FacetedContent facetedContent, IComponentPresentation componentPresentation)
        //{
        //    // Active Discussion filter is always visible for Discover / Learn / Share.
        //    // Product Explorer and Brands uses a different controller, so we can just switch the filter on regardless...
        //    SetViewBag(componentPresentation);

        //    return GetFacetedContent(facetedContent, componentPresentation, "post");
        //}


        ///// <summary>
        ///// Gets the faceted content.
        ///// </summary>
        ///// <param name="facetedContent">Content of the faceted.</param>
        ///// <param name="componentPresentation">The component presentation.</param>
        ///// <returns></returns>
        //public ActionResult GetFacetedContent(FacetedContent facetedContent, IComponentPresentation componentPresentation, string FormMethod)
        //{
        //    string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
        //    com.fredhopper.lang.query.Query query = FacetedContentHelper.BuildQuery(ref facetedContent, ItemsPerPage, viewType);

        //    // Only call FredHopper if there is at least one component section selected (e.g. Projects & patterns, Articles, Moodboards, Designers, etc)
        //    bool historyBack = false;
        //    int selectedCount = facetedContent.ComponentSection.ComponentTypes.Facets.Count(m => m.Selected);
        //    if (selectedCount > 0)
        //    {
        //        // Is this request from a history back link?
        //        if (!bool.TryParse(Request.QueryString["history"], out historyBack))
        //        {
        //            historyBack = false;
        //        }
                
        //        facetedContent = CallFredHopperAndPopulate(query, facetedContent, historyBack);
        //    }
        //    else
        //    {
        //        #region Navigational Items

        //        // Ensure Pagingation (if no selected components)
        //        facetedContent.ComponentList = new PaginatedComponentList();
        //        facetedContent.ComponentList.Pagination = new List<FacetPagination>();
        //        facetedContent.ComponentList.Pagination.Add(new FacetPagination { Url = "#", LinkText = "1", Selected = true });

        //        // Ensure facet map (if no selected components)
        //        facetedContent.FacetMap = new FacetCollection();

        //        #endregion

        //        if (facetedContent.ComponentList.Components == null)
        //        {
        //            facetedContent.ComponentList.Components = new List<Component>();
        //        }
        //    }

        //    var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];

        //    if (facetedContent.ComponentList.Components.Count > 0)
        //    {
        //        facetedContent = FacetedContentHelper.InjectIntroComponent(facetedContent, introMarkup, historyBack);

        //        IList<IComponentPresentation> ctaList = componentPresentation.Page.ComponentPresentations.Where(m => m.ComponentTemplate.Id != componentPresentation.ComponentTemplate.Id).ToList();
        //        //facetedContent = FacetedContentHelper.InjectCTAComponents(facetedContent, ctaList, true);
        //        facetedContent = FacetedContentHelper.InjectCTAComponents(facetedContent, ctaList, randomInsert: true, historyBack: historyBack);
        //    }

        //    // Create the "Sort by" select list
        //    if (facetedContent.Sort == null) { facetedContent.Sort = new FacetSort(); }
        //    facetedContent.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, componentPresentation.ComponentTemplate.MetadataFields["view"].Value);
        //    facetedContent.Sort.ActiveDiscussionsUrl = FacetedContentHelper.GetActiveDiscussionUrl(query);

        //    // I would have preferred to be encapsulated, but it all relies on the final query string.
        //    facetedContent.ComponentSection.ComponentTypes.SetHrefLinks(query);

        //    if (!string.IsNullOrEmpty(_viewNameOverride))
        //    {
        //        return View(_viewNameOverride, facetedContent);
        //    }
        //    else
        //    {
        //        return View(facetedContent);
        //    }

        //}

        protected override Query PreCallFredHopper<T>(Query query, ref T facetedContent) //where T : FacetedContentBase
        {
            // Add the active component types from the checkbox list (e.g. projects, articles, moodboards, designers)
            var loc = query.getLocation();


            MultiValuedCriterion mvc = CriterionFactory.parse(facetedContent.ComponentSection.ComponentTypes.ToLocationString()) as MultiValuedCriterion;
            if (mvc != null)
            {
                if (!mvc.getGreaterThan().isEmpty())
                {
                    loc.removeCriteria("schematitle");
                    loc.addCriterion(mvc);
                    query.setLocation(loc);
                }
            }

            return query;
        }

        //protected virtual T CallFredHopperAndPopulate<T>(Query query, T facetedContent) where T : FacetedContentBase
        //{
        //    query = PreCallFredHopper(query, ref facetedContent);

        //    // Store the current query location in the model
        //    facetedContent.FredHopperLocation = query.getLocation().toString();

        //    #region Call FredHopper

        //    //// Enough faffing already; let's query FredHopper!
        //    DD4TComponents dd4t = new DD4TComponents();
        //    if (facetedContent.ComponentList == null) { facetedContent.ComponentList = new PaginatedComponentList(); }
        //    facetedContent.ComponentList.Components = dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions, "?rts=true");

        //    #endregion

        //    #region Navigational Items

        //    // Pagingation
        //    try
        //    {
        //        facetedContent.ComponentList.Pagination = dd4t.Universe.itemssection.results.GetFacetPagination(query, ItemsPerPage);
        //    }
        //    catch (Exception) { facetedContent.ComponentList.Pagination = new List<FacetPagination>(); }
            
        //    // Get (create) facet map
        //    try
        //    {
        //        facetedContent.FacetMap = dd4t.Universe.facetmap[0].GetFacetCollection(query.getLocation().toString());
        //    }
        //    catch (Exception) { facetedContent.FacetMap = new FacetCollection(); }

        //    #endregion

        //    return facetedContent;
        //}

    }
}
