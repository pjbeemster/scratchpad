namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Attributes;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.CustomOutputCache;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.FredHopper;
    using Coats.Crafts.Models;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    [FredHopperOutputCache(CacheProfile="FacetedContent"), NoCache]
    public abstract class FacetedContentBaseController : Controller
    {
        protected IComponentFactory _componentFactory;
        protected IAppSettings _settings;
        protected string _viewNameOverride = null;
        protected const int DefaultItemsPerPage = 10;

        public FacetedContentBaseController(IAppSettings settings, IComponentFactory componentFactory)
        {
            this._settings = settings;
            this._componentFactory = componentFactory;
        }

        protected virtual T CallFredHopperAndPopulate<T>(Query query, T facetedContent, bool historyBack) where T: FacetedContentBase
        {
            query = this.PreCallFredHopper<T>(query, ref facetedContent);
            facetedContent.FredHopperLocation = query.getLocation().toString();
            DD4TComponents components = new DD4TComponents(this.Logger);
            if (facetedContent.ComponentList == null)
            {
                facetedContent.ComponentList = new PaginatedComponentList();
            }
            if (facetedContent.Sort == null)
            {
                facetedContent.Sort = new FacetSort();
            }

            /* ================================================== */
            /* CHECK FABRIC SECTION TAKEN FROM STAGING ASSEMBLIES */
            /* ================================================== */
            facetedContent.ComponentList.CheckFabric = false;
            if (base.Request.QueryString["fh_params"] != null)
            {
                string[] strArray = base.Request.QueryString["fh_params"].Split(new char[] { '&' });
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (string str2 in strArray)
                {
                    int index = str2.IndexOf('=');
                    if ((index > -1) && (index < str2.Length))
                    {
                        dictionary.Add(str2.Substring(0, index), str2.Substring(index + 1));
                    }
                }
                if (base.Request.QueryString["fh_params"].Contains("dir__fabrics") && dictionary["fh_location"].ToString().EndsWith("dir__fabrics}"))
                {
                    facetedContent.ComponentList.CheckFabric = true;
                }
            }
            facetedContent.FabricComponents = new List<IComponent>();
            string tcm = string.Format(this._settings.FabricPage, this._settings.PublicationId);
            IList<IComponentPresentation> componentPresentations = this.GetPageInfo(tcm).ComponentPresentations;
            foreach (IComponentPresentation presentation in componentPresentations)
            {
                facetedContent.FabricComponents.Add(presentation.Component);
            }
            /* ================================================== */
            /* END FABRIC SECTION                                 */
            /* ================================================== */

            int offset = query.getListStartIndex();
            int size = query.getListViewSize();
            if (historyBack && (offset > 0))
            {
                int num3 = offset + size;
                query.setListViewSize(num3);
                query.setListStartIndex(0);
                facetedContent.ComponentList.Components = components.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);
                query.setListStartIndex(offset);
                query.setListViewSize(size);
                components.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);
            }
            else
            {
                facetedContent.ComponentList.Components = components.GetComponents(query.toString(), true, facetedContent.Sort.ActiveDiscussions);
            }
            try
            {
                facetedContent.ComponentList.Pagination = components.Universe.itemssection.results.GetFacetPagination(query, this.ItemsPerPage);
            }
            catch (Exception)
            {
                facetedContent.ComponentList.Pagination = new List<FacetPagination>();
            }
            try
            {
                facetedContent.FacetMap = components.Universe.facetmap[0].GetHierarchicalFacetCollection(query.getLocation().toString(), this._settings.PublicationId);
                facetedContent.ReturnedSchemas = components.Universe.facetmap[0].GetSchemaCollection(query.getLocation().toString());
                if (base.RouteData.Values.IsLevel1BrandFilterActivated())
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.Debug("Level 1 Brand activated, adding brand filter into return url");
                    }
                    Query query2 = new Query(new Location(facetedContent.FacetMap.ResetFacetsUrl));
                    string str = "fh_location=" + FacetedContentHelper.AppendOptionalFacet(query2.getLocation().toString(), base.RouteData.Values.GetLevel1BrandFilter());
                    query2.ParseQuery(str);
                    facetedContent.FacetMap.ResetFacetsUrl = query2.ToFhParams();
                }
                else
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.Debug("Not active, so just create normal fh_params");
                    }
                    facetedContent.FacetMap.ResetFacetsUrl = new Query(new Location(facetedContent.FacetMap.ResetFacetsUrl)).ToFhParams();
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error("Error in CallFredHopperAndPopulate when assigning FacetMap and ReturnedSchemas", exception);
                facetedContent.FacetMap = new FacetCollection();
            }
            string absoluteUri = base.HttpContext.Request.Url.AbsoluteUri;
            string returnUrl = (absoluteUri.Contains<char>('?') ? absoluteUri.Split(new char[] { '?' })[0] : absoluteUri) + query.ToFhParams() + "&history=true";
            base.Session.SetReturnToSearchUrl(returnUrl, base.Request);
            return facetedContent;
        }

        protected void CheckQueryContainsBrand(string fh_params)
        {
            Func<string, bool> predicate = null;
            string attribute = base.RouteData.Values.GetLevel1BrandFacet();
            string facet_value = base.RouteData.Values.GetLevel1BrandFacetValueDelimeted();
            bool flag = true;
            Query query = new Query();
            query.ParseQuery(fh_params);
            MultiValuedCriterion criterion = query.getLocation().getCriterion(attribute) as MultiValuedCriterion;
            if (criterion != null)
            {
                if (predicate == null)
                {
                    predicate = v => v == facet_value;
                }
                if (criterion.getGreaterThan().values().Any<string>(predicate))
                {
                    this.Logger.DebugFormat("Session maintained as {0} in query {1}", new object[] { facet_value, query.toString() });
                    flag = false;
                }
            }
            if (flag && (base.Request["vanity_url"] != null))
            {
                this.Logger.DebugFormat("Session maintained as {0} in query {1} as vanity_url present", new object[] { facet_value, query.toString() });
                flag = false;
            }
            if (flag)
            {
                base.RouteData.Values["Level1BrandActivated"] = false;
                base.RouteData.Values["BrandComponent"] = new Field();
                base.RouteData.Values["BrandFilter"] = string.Empty;
                base.RouteData.Values["BrandFacet"] = string.Empty;
                base.RouteData.Values["BrandFacetValue"] = string.Empty;
                base.RouteData.Values["BrandValueForSearch"] = string.Empty;
                base.Session.ClearLevel1BrandFilter();
            }
        }

        protected abstract Query PreCallFredHopper<T>(Query query, ref T facetedContent) where T: FacetedContentBase;

        protected virtual string DefaultLocation
        {
            get
            {
                return FredHopperInterface.GetPublicationPath(this._settings.PublicationId);
            }
        }

        protected virtual int ItemsPerPage
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"]);
                }
                catch
                {
                    return 10;
                }
            }
        }

        private IPage GetPageInfo(string tcm)
        {
            IContainerAccessor applicationInstance = System.Web.HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IPageFactory>().GetPage(tcm);
        }

        public ILogger Logger { get; set; }
    }
}

