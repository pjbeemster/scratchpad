using System;
using System.Web.Mvc;

using Coats.Crafts.Extensions;
using Coats.Crafts.Models;
using Coats.Crafts.Configuration;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.CustomOutputCache;
using Coats.Crafts.Filters;

using DD4T.ContentModel.Factories;
using DD4T.ContentModel;

namespace Coats.Crafts.Controllers
{
    public class ProductExplorerController : ProductBrandBaseController
    {
        protected override string DefaultLocation
        {
            get
            {
                //e.g. //catalog01/en_US/publicationid=tcm_0_70_1/schematitle={crafts2eproduct}
                return base.DefaultLocation + "/schematitle=crafts2eproduct";
            }
        }

        public ProductExplorerController(IAppSettings settings, IComponentFactory componentFactory, IKeywordRepository keywordrepository)
            : base(settings, componentFactory, keywordrepository)
        {
            _viewNameOverride = "~/Views/FacetedContent/Product-Explorer.cshtml";
            _listParentTcmId = string.Format(base._settings.ProductGroups, PublicationId);
            //_category = "product_groups";
            _category = string.Format(settings.ProductGroupFormat, settings.PublicationId);
        }

        //protected override Query PreCallFredHopper<T>(Query query, ref T productExplorer)
        //{
        //    // The base PreCallFredHopper adds extra criteria which we don't need in this view.
        //    // The extra criteria in this instance is in the DefaultLocation property.
        //    return query;
        //}

        //
        // GET: /Product-Explorer/
        
        //public ActionResult Index(IComponentPresentation componentPresentation)
        //{
        //    try { ViewBag.Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value; }
        //    catch (Exception) { }
        //    ViewBag.PodGenericSpan = "span3";

        //    ProductExplorer productExplorer = new ProductExplorer();
        //    com.fredhopper.lang.query.Query query = FacetedContentHelper.BuildQuery(ref productExplorer,
        //                                                                Request.QueryString["fh_params"],
        //                                                                componentPresentation.ComponentTemplate.MetadataFields["view"].Value,
        //                                                                ItemsPerPage,
        //                                                                DefaultLocation);


        //    if (productExplorer.ExtComponentSection == null)
        //    {
        //        productExplorer.ExtComponentSection = new ExtComponentSearchSection();
        //    }

        //    productExplorer = CallFredHopperAndPopulate(query, productExplorer);

        //    SetChooseCategoryList(productExplorer, query);

        //    var componentLoc = com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(productExplorer.ComponentSection.ComponentTypes.ToLocationString());
        //    var brandsLoc = query.getLocation();
        //    brandsLoc.removeCriteria("schematitle");
        //    brandsLoc.addCriterion(componentLoc);
        //    query.setLocation(brandsLoc);

        //    productExplorer.ExtComponentSection.LinkUrl = "?fh_params=" + HttpUtility.UrlEncode(query.toString());
        //    productExplorer.ExtComponentSection.LinkText = Helper.GetResource("OurBrands"); ;

        //    var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];

        //    // ASH: Removed the intro component as per jira issue CC-557
        //    //productExplorer = FacetedContentHelper.InjectIntroComponent(productExplorer, introMarkup);

        //    if (!string.IsNullOrEmpty(_viewNameOverride))
        //    {
        //        return View(_viewNameOverride, productExplorer);
        //    }
        //    else
        //    {
        //        return View(productExplorer);
        //    }
            
        //}

        [Level1BrandFilter]
        public ActionResult Index(IComponentPresentation componentPresentation, bool clear = false)
        {
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
            //if ((Session["FACETED_CONTENT"] != null) && !(Session["FACETED_CONTENT"] is ProductExplorer))
            //{
            //    Session["FACETED_CONTENT"] = null;
            //}

            //if (Session["FACETED_CONTENT"] != null && Request.QueryString["search"] != null)
            //{
            //    // Final check: is the session object of the right type?
            //    if (Session["FACETED_CONTENT"] is ProductExplorer)
            //    {
            //        ProductExplorer productExplorer = (ProductExplorer)Session["FACETED_CONTENT"];
            //        return HandleIndexPost(productExplorer, componentPresentation, "get");
            //    }
            //}

            ViewBag.ExpandForProductExplorer = true;

            return HandleIndexGet(componentPresentation);
        }

        //[HttpPost]
        //public ActionResult Index(ProductExplorer productExplorer, IComponentPresentation componentPresentation)
        //{
        //    try { ViewBag.Title = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["title"].Value; }
        //    catch (Exception) { }
        //    ViewBag.PodGenericSpan = "span3";

        //    com.fredhopper.lang.query.Query query = FacetedContentHelper.BuildQuery(ref productExplorer, ItemsPerPage);

        //    // However, we do need to set any potential drop down list selection
        //    string chosenCategory = productExplorer.ExtComponentSection.ChosenCategory;
        //    if (!string.IsNullOrEmpty(chosenCategory))
        //    {
        //        var loc = query.getLocation();
        //        loc.removeCriteria("techniques");
        //        loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(chosenCategory));
        //    }

        //    productExplorer = CallFredHopperAndPopulate(query, productExplorer);

        //    var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];
        //    productExplorer = FacetedContentHelper.InjectIntroComponent(productExplorer, introMarkup);

        //    SetChooseCategoryList(productExplorer, query);

        //    if (!string.IsNullOrEmpty(_viewNameOverride))
        //    {
        //        return View(_viewNameOverride, productExplorer);
        //    }
        //    else
        //    {
        //        return View(productExplorer);
        //    }

        //}

        [HttpPost, Level1BrandFilter]
        public ActionResult Index(ProductExplorer productExplorer, IComponentPresentation componentPresentation)
        {
            ViewBag.ExpandForProductExplorer = true;

            return HandleIndexPost(productExplorer, componentPresentation, "post");
        }

        //protected void SetChooseCategoryList(ProductExplorer productExplorer, Query query)
        //{
        //    // Populate category list
        //    // Category for drop down list
        //    //tcm:70-4086-512

        //    //product_groups>{product_groups__dir__canvas}
        //    //product_groups>{product_groups__dir__cotton_thread}
        //    //product_groups>{product_groups__dir__crochet}
        //    //product_groups>{product_groups__dir__embroidery_thread}
        //    //etc...

        //    string category = "product_groups";
        //    string listProductGroups = string.Format(base._settings.ProductGroups, PublicationId);
        //    var keywords = _keywordrepository.GetKeywordsList(listProductGroups);
        //    if (productExplorer.ExtComponentSection.ChooseCategoryList == null)
        //    {
        //        productExplorer.ExtComponentSection.ChooseCategoryList = new List<FacetItem>();
        //    }

        //    productExplorer.ExtComponentSection.ChooseCategoryList.Clear();

        //    productExplorer.ExtComponentSection.ChooseCategoryList.Add(new FacetItem { Text = Helper.GetResource("ChooseACategory"), Value = string.Empty });

        //    foreach (var keyword in keywords)
        //    {
        //        FacetItem catListItem = new FacetItem();
        //        catListItem.Text = keyword.Description;
        //        string keywordId = keyword.Name.ToLower().Replace(' ', '_');

        //        //string critPath = string.Format("{0}>{{{0}__{1}}}", category, keywordId);
        //        string critPath = string.Format("{0}>{{{0}{1}{2}}}", category, FacetedContentHelper.NestedLevelDelimiter, keywordId);

        //        // Get new copy of location everytime, otherwise we just keep over writing the attributes.
        //        var loc = new com.fredhopper.lang.query.location.Location(query.getLocation());
        //        catListItem.Selected = loc.getCriteria(category).contains(keywordId);

        //        loc.removeCriteria(category);
        //        loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(critPath));

        //        catListItem.Value = loc.toString();

        //        productExplorer.ExtComponentSection.ChooseCategoryList.Add(catListItem);
        //    }

        //}

    }
}
