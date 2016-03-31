namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;

    public abstract class ProductBrandBaseController : FacetedContentBaseController
    {
        protected string _category;
        protected readonly IKeywordRepository _keywordrepository;
        protected string _listParentTcmId;

        public ProductBrandBaseController(IAppSettings settings, IComponentFactory componentFactory, IKeywordRepository keywordrepository) : base(settings, componentFactory)
        {
            this._listParentTcmId = string.Empty;
            this._category = string.Empty;
            this._keywordrepository = keywordrepository;
        }

        protected ActionResult HandleIndexGet(IComponentPresentation componentPresentation)
        {
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value;
            }
            catch (Exception)
            {
            }
            ((dynamic) base.ViewBag).PodGenericSpan = "span3";
            ProductExplorer facetedContent = new ProductExplorer();
            if (base.RouteData.Values.IsLevel1BrandFilterActivated())
            {
                facetedContent.Brand = base.RouteData.Values.GetLevel1BrandComponent();
                if (!string.IsNullOrEmpty(base.Request.QueryString["fh_params"]))
                {
                    base.CheckQueryContainsBrand(base.Request.QueryString["fh_params"]);
                }
            }
            Query query = FacetedContentHelper.BuildQuery<ProductExplorer>(ref facetedContent, base.Request.QueryString["fh_params"], componentPresentation.ComponentTemplate.MetadataFields["view"].Value, this.ItemsPerPage, this.DefaultLocation, base._settings.PublicationId, base.RouteData.Values.GetLevel1BrandFilter(), "");
            if (facetedContent.ExtComponentSection == null)
            {
                facetedContent.ExtComponentSection = new ExtComponentSearchSection();
            }
            bool result = false;
            if (!bool.TryParse(base.Request.QueryString["history"], out result))
            {
                result = false;
            }
            facetedContent = this.CallFredHopperAndPopulate<ProductExplorer>(query, facetedContent, result);
            if (base.Request.Url.LocalPath != base.Url.Content(ConfigurationManager.AppSettings["Brands"].AddApplicationRoot()))
            {
                this.SetChooseCategoryList(facetedContent, query);
            }
            string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
            if (facetedContent.Sort == null)
            {
                facetedContent.Sort = new FacetSort();
            }
            facetedContent.Sort.SortBy = FacetedContentHelper.GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, base._settings.PublicationId);
            facetedContent.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, viewType, base._settings.PublicationId);
            facetedContent.Sort.ActiveDiscussionsEnabled = false;
            facetedContent.ComponentSection.ComponentTypes.SetHrefLinks(query);
            if (!string.IsNullOrEmpty(base._viewNameOverride))
            {
                return base.View(base._viewNameOverride, facetedContent);
            }
            return base.View(facetedContent);
        }

        protected ActionResult HandleIndexPost(ProductExplorer productExplorer, IComponentPresentation componentPresentation, string FormMethod)
        {
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value;
            }
            catch (Exception)
            {
            }
            if (base.RouteData.Values.IsLevel1BrandFilterActivated())
            {
                productExplorer.Brand = base.RouteData.Values.GetLevel1BrandComponent();
                if (!string.IsNullOrEmpty(base.Request.QueryString["fh_params"]))
                {
                    base.CheckQueryContainsBrand(base.Request.QueryString["fh_params"]);
                }
            }
            ((dynamic) base.ViewBag).PodGenericSpan = "span3";
            string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
            Query query = FacetedContentHelper.BuildQuery<ProductExplorer>(ref productExplorer, this.ItemsPerPage, viewType, base._settings.PublicationId);
            string chosenCategory = productExplorer.ExtComponentSection.ChosenCategory;
            if (!string.IsNullOrEmpty(chosenCategory))
            {
                Location location = query.getLocation();
                location.removeCriteria(this._category);
                location.addCriterion(CriterionFactory.parse(chosenCategory));
            }
            bool result = false;
            if (!bool.TryParse(base.Request.QueryString["history"], out result))
            {
                result = false;
            }
            productExplorer = this.CallFredHopperAndPopulate<ProductExplorer>(query, productExplorer, result);
            this.SetChooseCategoryList(productExplorer, query);
            if (productExplorer.Sort == null)
            {
                productExplorer.Sort = new FacetSort();
            }
            productExplorer.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, viewType, base._settings.PublicationId);
            productExplorer.ComponentSection.ComponentTypes.SetHrefLinks(query);
            if (!string.IsNullOrEmpty(base._viewNameOverride))
            {
                return base.View(base._viewNameOverride, productExplorer);
            }
            return base.View(productExplorer);
        }

        protected override Query PreCallFredHopper<T>(Query query, ref T facetedContent) where T: FacetedContentBase
        {
            return query;
        }

        protected void SetChooseCategoryList(ProductExplorer productExplorer, Query query)
        {
            Func<FacetSection, bool> predicate = null;
            Action<FacetItem> action = null;
            if (productExplorer.ExtComponentSection.ChooseCategoryList == null)
            {
                productExplorer.ExtComponentSection.ChooseCategoryList = new List<FacetItem>();
            }
            productExplorer.ExtComponentSection.ChooseCategoryList.Clear();
            Location locOrg = new Location(query.getLocation());
            if (productExplorer.FacetMap.FacetSections != null)
            {
                if (predicate == null)
                {
                    predicate = f => f.on == this._category;
                }
                FacetSection section = productExplorer.FacetMap.FacetSections.SingleOrDefault<FacetSection>(predicate);
                if (section != null)
                {
                    if (action == null)
                    {
                        action = delegate (FacetItem f) {
                            f.Selected = locOrg.getCriteria(this._category).ToString().Contains(f.FHId);
                            if (!f.Selected)
                            {
                                Location location = new Location(query.getLocation());
                                Criterion criterion = location.getCriterion("publicationid");
                                Criterion criterion2 = location.getCriterion("schematitle");
                                Criterion criterion3 = null;
                                if (this.RouteData.Values.IsLevel1BrandFilterActivated())
                                {
                                    criterion3 = location.getCriterion(this.RouteData.Values.GetLevel1BrandFacet());
                                }
                                location.removeAllCriteria();
                                if (criterion != null)
                                {
                                    location.addCriterion(criterion);
                                }
                                if (criterion2 != null)
                                {
                                    location.addCriterion(criterion2);
                                }
                                if (criterion3 != null)
                                {
                                    location.addCriterion(criterion3);
                                }
                                ValueSet lowSet = new ValueSet(ValueSet.AggregationType.OR);
                                lowSet.add(f.FHId);
                                MultiValuedCriterion criterion4 = new MultiValuedCriterion(this._category, lowSet, null, false);
                                f.Value = new Query(location.addCriterion(criterion4)).ToFhParams();
                            }
                            productExplorer.ExtComponentSection.ChooseCategoryList.Add(f);
                        };
                    }
                    section.Facets.ToList<FacetItem>().ForEach(action);
                }
            }
            query.setLocation(locOrg);
        }

        protected override int ItemsPerPage
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["ProductExplorerItemsPerPage"]);
                }
                catch
                {
                    return 10;
                }
            }
        }

        public int PublicationId
        {
            get
            {
                return base._settings.PublicationId;
            }
        }
    }
}

