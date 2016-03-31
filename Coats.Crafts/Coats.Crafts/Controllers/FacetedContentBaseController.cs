using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

using Coats.Crafts.Configuration;
using Coats.Crafts.Filters;
using Coats.Crafts.FredHopper;
using Coats.Crafts.Models;
using Coats.Crafts.Extensions;
using Coats.Crafts.CustomOutputCache;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Attributes;

using DD4T.ContentModel.Factories;
using DD4T.ContentModel;

using Castle.Core.Logging;

using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location;
using com.fredhopper.lang.query.location.criteria;

namespace Coats.Crafts.Controllers
{
    [NoCache]
    [FredHopperOutputCache(CacheProfile = "FacetedContent")]
    public abstract class FacetedContentBaseController : Controller
    {
        protected const int DefaultItemsPerPage = 10;
        protected IAppSettings _settings;
        protected IComponentFactory _componentFactory;
        protected string _viewNameOverride = null;
        public ILogger Logger { get; set; }

        /// <summary>
        /// Check a query for the brand facet - if not there then the brand session is ended.
        /// The brand facet is removed when clicking the "close" link on the brand banner
        /// </summary>
        /// <param name="query"></param>
        protected void CheckQueryContainsBrand(string fh_params)
        {
            string facet = RouteData.Values.GetLevel1BrandFacet();
            string facet_value = RouteData.Values.GetLevel1BrandFacetValueDelimeted();
            bool clear = true;

            Query query = new Query();
            query.ParseQuery(fh_params);

            MultiValuedCriterion mvc = query.getLocation().getCriterion(facet) as MultiValuedCriterion;
            if (mvc != null)
            {
                string[] values = mvc.getGreaterThan().values();
                // Does the brand facet value still exist?
                if (values.Any(v => v == facet_value))
                {
                    Logger.DebugFormat("Session maintained as {0} in query {1}", facet_value, query.toString());
                    clear = false;
                }
            }

            // The brand filter could have been activated, then some use a vanity Url (another 301 redirect)
            // If this redirection didnt carry the brand facet in the QS then the session would be cleared.
            // Instead, these vanity urls add their own QS param which we check for
            if (clear)
            {
                if (Request["vanity_url"] != null)
                {
                    Logger.DebugFormat("Session maintained as {0} in query {1} as vanity_url present", facet_value, query.toString());
                    clear = false;
                }
            }

            if (clear)
            {
                // Clear route data down
                RouteData.Values["Level1BrandActivated"] = false;
                RouteData.Values["BrandComponent"] = new Field();
                RouteData.Values["BrandFilter"] = String.Empty;
                RouteData.Values["BrandFacet"] = String.Empty;
                RouteData.Values["BrandFacetValue"] = String.Empty;
                RouteData.Values["BrandValueForSearch"] = String.Empty;

                Session.ClearLevel1BrandFilter();
            }
        }


        protected virtual int ItemsPerPage
        {
            get
            {
                try { return int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"]); }
                catch { return DefaultItemsPerPage; }
            }
        }

        protected virtual string DefaultLocation
        {
            get
            {
                //e.g. //catalog01/en_US/publicationid=tcm_0_70_1
                return FredHopperInterface.GetPublicationPath(_settings.PublicationId);
            }
        }

        public FacetedContentBaseController(IAppSettings settings, IComponentFactory componentFactory)
        {
            _settings = settings;
            _componentFactory = componentFactory;
        }

        protected abstract Query PreCallFredHopper<T>(Query query, ref T facetedContent) where T : FacetedContentBase;

        //protected virtual T CallFredHopperAndPopulate<T>(Query query, T facetedContent) where T : FacetedContentBase
        //{
        //    return CallFredHopperAndPopulate(query, facetedContent, null);
        //}

        /// <summary>
        /// Does what it says on the tin.
        /// After calling FredHopper, the facetedContent model is populated.
        /// </summary>
        /// <typeparam name="T">Must derive from FacetedContentBase</typeparam>
        /// <param name="query">The FredHopper query</param>
        /// <param name="facetedContent">The model to populate</param>
        /// <param name="historyBack">
        /// A boolean flag which denotes if this query has been fired from a history
        /// back button click, or similar (return to search results, etc). It will 
        /// determine if the query should play "catch-up" and get
        /// all content from index zero to the current page. For example, if the query is
        /// set to get the items for page 2 (i.e. 10 through to 19), we actually want to
        /// get all the items from the beginning (i.e. 0 through to 19)
        /// </param>
        /// <returns>The populated model, which must derive from FacetedContentBase</returns>
        protected virtual T CallFredHopperAndPopulate<T>(Query query, T facetedContent, bool historyBack) where T : FacetedContentBase
        {
            query = PreCallFredHopper(query, ref facetedContent);

            // Store the current query location in the model
            facetedContent.FredHopperLocation = query.getLocation().toString();

            //// I would have preferred to be encapsulated, but it all relies on the final query string.
            //facetedContent.ComponentSection.ComponentTypes.SetHrefLinks(query);

            #region Call FredHopper

            //// Enough faffing already; let's query FredHopper!
            DD4TComponents dd4t = new DD4TComponents(Logger);
            if (facetedContent.ComponentList == null) { facetedContent.ComponentList = new PaginatedComponentList(); }
            if (facetedContent.Sort == null) { facetedContent.Sort = new FacetSort(); }
            //facetedContent.ComponentList.Components = dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions, "rts=true");

            // Original...
            //facetedContent.ComponentList.Components = dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);

            // This is all to do with the history back button (or similar).
            // If historyBack is true, then we want to get all content from 
            // zero to the current page.
            
            // Store the original values (we'll need these later)
            int orgStartIndex = query.getListStartIndex();
            int orgViewSize = query.getListViewSize();

            // Do we need to get all previous pages and merge into one?
            if (historyBack && orgStartIndex > 0)
            {
                // Work out the new view size (the number of items to retrieve).
                int viewSize = orgStartIndex + orgViewSize;
                // Set the new view size
                query.setListViewSize(viewSize);
                // Reset the start index to zero
                query.setListStartIndex(0);

                // Get the components
                facetedContent.ComponentList.Components = dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);

                // TODO - this really needs changing to avoid the second call to FH
                // Reset the original values, and call FH again. 
                // This is purely to re-sync the pagination
                query.setListStartIndex(orgStartIndex);
                query.setListViewSize(orgViewSize);
                dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);
            }
            else
            {
                // Simple, original call.
                facetedContent.ComponentList.Components = dd4t.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);
            }

            #endregion

            #region Navigational Items

            // Pagingation
            try
            {
                facetedContent.ComponentList.Pagination = dd4t.Universe.itemssection.results.GetFacetPagination(query, ItemsPerPage);
            }
            catch (Exception) { facetedContent.ComponentList.Pagination = new List<FacetPagination>(); }

            // Get (create) facet map
            try
            {
                facetedContent.FacetMap = dd4t.Universe.facetmap[0].GetHierarchicalFacetCollection(query.getLocation().toString(), _settings.PublicationId);
                facetedContent.ReturnedSchemas = dd4t.Universe.facetmap[0].GetSchemaCollection(query.getLocation().toString());

                // If Level 1 Brand Filter is activated, check its on the return URL
                // Had to adjust return ResetFacetsUrl to be just the querystring to make life a littel easier 
                if (RouteData.Values.IsLevel1BrandFilterActivated())
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug("Level 1 Brand activated, adding brand filter into return url");

                    
                    Query q = new Query(
                        new Location(facetedContent.FacetMap.ResetFacetsUrl));

                    string fh_params;
                    fh_params = "fh_location=" + FacetedContentHelper.AppendOptionalFacet(q.getLocation().toString(), RouteData.Values.GetLevel1BrandFilter());
                    q.ParseQuery(fh_params);
                    facetedContent.FacetMap.ResetFacetsUrl = q.ToFhParams();
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug("Not active, so just create normal fh_params");

                    Query q = new Query(
                        new Location(facetedContent.FacetMap.ResetFacetsUrl));
                    facetedContent.FacetMap.ResetFacetsUrl = q.ToFhParams();
                }
            }
            catch (Exception ex) 
            {
                Logger.Error("Error in CallFredHopperAndPopulate when assigning FacetMap and ReturnedSchemas", ex);
                facetedContent.FacetMap = new FacetCollection(); 
            }

            #endregion

            #region Return To Search Session (Yuk!)

            //if (string.IsNullOrEmpty(facetedContent.ComponentSection.SearchTerm))
            //{
            //    Session.ClearReturnToSearchUrl();
            //}
            //else
            //{
            //    //Session.SetReturnToSearchUrl(query.ToFhParams());
            //    string absUri = HttpContext.Request.Url.AbsoluteUri;
            //    query.removeListStartIndex();
            //    string returnString = (absUri.Contains('?') ? absUri.Split('?')[0] : absUri) + query.ToFhParams();
            //    Session.SetReturnToSearchUrl(returnString);
            //}

            string absUri = HttpContext.Request.Url.AbsoluteUri;
            //query.removeListStartIndex();
            string returnString = (absUri.Contains('?') ? absUri.Split('?')[0] : absUri) + query.ToFhParams() + "&history=true";
            Session.SetReturnToSearchUrl(returnString, Request);

            #endregion

            return facetedContent;
        }
    }
}
