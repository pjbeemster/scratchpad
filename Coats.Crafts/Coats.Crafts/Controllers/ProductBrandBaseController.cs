using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Coats.Crafts.Filters;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Configuration;
using Coats.Crafts.Models;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Resources;
using Coats.Crafts.Extensions;

using DD4T.ContentModel.Factories;
using DD4T.ContentModel;

using com.fredhopper.lang.query;
using com.fredhopper.lang.query.util;
using com.fredhopper.lang.query.location;
using com.fredhopper.lang.query.location.criteria;

namespace Coats.Crafts.Controllers
{
    public abstract class ProductBrandBaseController : FacetedContentBaseController
    {
        protected readonly IKeywordRepository _keywordrepository;
        //protected string _linkUrl = string.Empty;
        //protected string _linkTextKey = string.Empty;
        protected string _listParentTcmId = string.Empty;
        protected string _category = string.Empty;

        public int PublicationId
        {
            get { return _settings.PublicationId; }
        }

        public ProductBrandBaseController(IAppSettings settings, IComponentFactory componentFactory, IKeywordRepository keywordrepository)
            : base(settings, componentFactory)
        {
            _keywordrepository = keywordrepository;
        }

        protected override int ItemsPerPage
        {
            get
            {
                try { return int.Parse(ConfigurationManager.AppSettings["ProductExplorerItemsPerPage"]); }
                catch { return DefaultItemsPerPage; }
            }
        }

        protected override Query PreCallFredHopper<T>(Query query, ref T facetedContent)
        {
            // The base PreCallFredHopper adds extra criteria which we don't need in this view.
            // The extra criteria in this instance is in the DefaultLocation property.
            return query;
        }
         
        protected ActionResult HandleIndexGet(IComponentPresentation componentPresentation)
        {
            try { ViewBag.Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value; }
            catch (Exception) { }
            ViewBag.PodGenericSpan = "span3";

            ProductExplorer productExplorer = new ProductExplorer();
            if (RouteData.Values.IsLevel1BrandFilterActivated())
            {
                productExplorer.Brand = RouteData.Values.GetLevel1BrandComponent();
                // Check if the level 1 brand is NOT in this current query, if so, clear the session
                if (!String.IsNullOrEmpty(Request.QueryString["fh_params"]))
                {
                    base.CheckQueryContainsBrand(Request.QueryString["fh_params"]);
                }
            }
            
            com.fredhopper.lang.query.Query query = FacetedContentHelper.BuildQuery(ref productExplorer,
                                                                        Request.QueryString["fh_params"],
                                                                        componentPresentation.ComponentTemplate.MetadataFields["view"].Value,
                                                                        ItemsPerPage,
                                                                        DefaultLocation,
                                                                        _settings.PublicationId,
                                                                        RouteData.Values.GetLevel1BrandFilter());

            if (productExplorer.ExtComponentSection == null)
            {
                productExplorer.ExtComponentSection = new ExtComponentSearchSection();
            }

            // Is this request from a history back link?
            bool historyBack = false;
            if(!bool.TryParse(Request.QueryString["history"], out historyBack))
            {
                historyBack = false;
            }

            productExplorer = CallFredHopperAndPopulate(query, productExplorer, historyBack);

            // 13/09/13 - JC - Wrapping some logic here to remove category list on brands page
            if(Request.Url.LocalPath != Url.Content(ConfigurationManager.AppSettings["Brands"].AddApplicationRoot()))
            {
                SetChooseCategoryList(productExplorer, query);
            }

            //var componentLoc = com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(productExplorer.ComponentSection.ComponentTypes.ToLocationString());

            //var brandsLoc = query.getLocation();
            //brandsLoc.removeCriteria("schematitle");
            //brandsLoc.addCriterion(componentLoc);
            //query.setLocation(brandsLoc);

            //productExplorer.ExtComponentSection.LinkUrl = "?fh_params=" + HttpUtility.UrlEncode(query.toString());
            //productExplorer.ExtComponentSection.LinkUrl = _linkUrl;
            //productExplorer.ExtComponentSection.LinkText = Helper.GetResource("OurBrands");
            //productExplorer.ExtComponentSection.LinkText = Helper.GetResource(_linkTextKey);

            // ASH: Removed the intro component as per jira issue CC-557
            //var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];
            //productExplorer = FacetedContentHelper.InjectIntroComponent(productExplorer, introMarkup);

            // Create the "Sort by" select list
            string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
            if (productExplorer.Sort == null) { productExplorer.Sort = new FacetSort(); }
            productExplorer.Sort.SortBy = FacetedContentHelper.GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, _settings.PublicationId);
            productExplorer.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, viewType, _settings.PublicationId);

            // ASH: Adding a swich to make sure that Active Discussions isn't shown in responsive views.
            productExplorer.Sort.ActiveDiscussionsEnabled = false;

            // I would have preferred to be encapsulated, but it all relies on the final query string.
            productExplorer.ComponentSection.ComponentTypes.SetHrefLinks(query);

            if (!string.IsNullOrEmpty(_viewNameOverride))
            {
                return View(_viewNameOverride, productExplorer);
            }
            else
            {
                return View(productExplorer);
            }
            
        }

        protected ActionResult HandleIndexPost(ProductExplorer productExplorer, IComponentPresentation componentPresentation, string FormMethod)
        {
            try { ViewBag.Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value; }
            catch (Exception) { }

            if (RouteData.Values.IsLevel1BrandFilterActivated())
            {
                productExplorer.Brand = RouteData.Values.GetLevel1BrandComponent();
                // Check if the level 1 brand is NOT in this current query, if so, clear the session
                if (!String.IsNullOrEmpty(Request.QueryString["fh_params"]))
                {
                    base.CheckQueryContainsBrand(Request.QueryString["fh_params"]);
                }
            }

            //Session["FACETED_CONTENT"] = productExplorer;

            //if (FormMethod == "post" && Request.UrlReferrer != null)
            //{
            //    Session.Add("REFERRER", Request.UrlReferrer.ToString());
            //}
            
            ViewBag.PodGenericSpan = "span3";

            string viewType = componentPresentation.ComponentTemplate.MetadataFields["view"].Value;
            com.fredhopper.lang.query.Query query = FacetedContentHelper.BuildQuery(ref productExplorer, ItemsPerPage, viewType, _settings.PublicationId);

            // However, we do need to set any potential drop down list selection
            string chosenCategory = productExplorer.ExtComponentSection.ChosenCategory;
            if (!string.IsNullOrEmpty(chosenCategory))
            {
                var loc = query.getLocation();
                //loc.removeCriteria("techniques");
                loc.removeCriteria(_category);
                loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(chosenCategory));
            }

            // Is this request from a history back link?
            bool historyBack = false;
            if (!bool.TryParse(Request.QueryString["history"], out historyBack))
            {
                historyBack = false;
            }

            productExplorer = CallFredHopperAndPopulate(query, productExplorer, historyBack);

            // ASH: Removed the intro component as per jira issue CC-557
            //var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];
            //productExplorer = FacetedContentHelper.InjectIntroComponent(productExplorer, introMarkup);

            SetChooseCategoryList(productExplorer, query);

            // Create the "Sort by" select list
            if (productExplorer.Sort == null) { productExplorer.Sort = new FacetSort(); }
            productExplorer.Sort.SortByList = FacetedContentHelper.CreateSortByList(query, viewType, _settings.PublicationId);

            // I would have preferred to be encapsulated, but it all relies on the final query string.
            productExplorer.ComponentSection.ComponentTypes.SetHrefLinks(query);

            if (!string.IsNullOrEmpty(_viewNameOverride))
            {
                return View(_viewNameOverride, productExplorer);
            }
            else
            {
                return View(productExplorer);
            }

        }

        protected void SetChooseCategoryList(ProductExplorer productExplorer, Query query)
        {
            // Populate category list
            // Category for drop down list
            //tcm:70-4086-512

            //product_groups>{product_groups__dir__canvas}
            //product_groups>{product_groups__dir__cotton_thread}
            //product_groups>{product_groups__dir__crochet}
            //product_groups>{product_groups__dir__embroidery_thread}
            //etc...

            //string listProductGroups = string.Format(base._settings.ProductGroups, PublicationId);
            //var keywords = _keywordrepository.GetKeywordsList(listProductGroups);

            // Dont use keywords from Tridion - CC-1566 - needs to be based on xx_product_group facet values.
            //var keywords = _keywordrepository.GetKeywordsList(_listParentTcmId).OrderBy(x=>x.Name);
            if (productExplorer.ExtComponentSection.ChooseCategoryList == null)
            {
                productExplorer.ExtComponentSection.ChooseCategoryList = new List<FacetItem>();
            }

            productExplorer.ExtComponentSection.ChooseCategoryList.Clear();

            var locOrg = new com.fredhopper.lang.query.location.Location(query.getLocation());

            if (productExplorer.FacetMap.FacetSections != null)
            {
                // Grab categories from unused facet
                FacetSection product_categories = productExplorer.FacetMap.FacetSections.SingleOrDefault(f => f.on == _category);

                if (product_categories != null)
                {
                    // Check if this facet is selected, if not change its link so that it clears away any existing criteria
                    product_categories.Facets.ToList().ForEach(f =>
                    {
                        f.Selected = locOrg.getCriteria(_category).ToString().Contains(f.FHId);

                        if (!f.Selected)
                        {
                            var loc = new Location(query.getLocation());
                            var critPub = loc.getCriterion("publicationid");
                            var critSchema = loc.getCriterion("schematitle");

                            Criterion brand = null; 
                            if (RouteData.Values.IsLevel1BrandFilterActivated())
                            {
                                brand = loc.getCriterion(RouteData.Values.GetLevel1BrandFacet());
                            }                            

                            loc.removeAllCriteria();

                            if (critPub != null) { loc.addCriterion(critPub); }
                            if (critSchema != null) { loc.addCriterion(critSchema); }
                            if (brand != null) { loc.addCriterion(brand); }

                            // Recreate query without other criteria
                            var values = new ValueSet(ValueSet.AggregationType.OR);
                            values.add(f.FHId);
                            var criteria = new MultiValuedCriterion(_category, values, null, false);
                            var q = new Query(loc.addCriterion(criteria));
                            f.Value = q.ToFhParams();
                        }

                        productExplorer.ExtComponentSection.ChooseCategoryList.Add(f);
                    });

                }
            }

            /* REMOVED AS DROP-DOWN DRIVEN BY FACET NOT TRIDION
            foreach (var keyword in keywords)
            {
                FacetItem catListItem = new FacetItem();
                catListItem.Text = keyword.Description;
                string keywordId = keyword.Name.ToLower().Replace(' ', '_');

                //string critPath = string.Format("{0}>{{{0}__{1}}}", _category, keywordId);
                string critPath = string.Format("{0}>{{{0}{1}{2}}}", _category, FacetedContentHelper.NestedLevelDelimiter, keywordId);

                // Get new copy of location everytime, otherwise we just keep over writing the attributes.
                var loc = new com.fredhopper.lang.query.location.Location(query.getLocation());
                catListItem.Selected = locOrg.getCriteria(_category).ToString().Contains(keywordId);

                if (!catListItem.Selected)
                {
                    //loc.removeCriteria(_category);

                    //CC-1475 - Filter not resetting when product category changed
                    //need to reset the filter before loading a different product group

                    // Store the publication id and schema title selections
                    var critPub = loc.getCriterion("publicationid");
                    var critSchema = loc.getCriterion("schematitle");
                    
                    // Remove the whole sha-boodle!
                    loc.removeAllCriteria();
                    // Return the publication id, schema title, and product group selections
                    if (critPub != null) { loc.addCriterion(critPub); }
                    if (critSchema != null) { loc.addCriterion(critSchema); }

                    loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(critPath));
                }

                query.setLocation(loc);

                catListItem.Value = query.ToFhParams();

                productExplorer.ExtComponentSection.ChooseCategoryList.Add(catListItem);
            }
            */
            query.setLocation(locOrg);
        }       
    }
}
