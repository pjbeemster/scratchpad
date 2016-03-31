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
    public class OurBrandsController : ProductBrandBaseController
    {
        protected override string DefaultLocation
        {
            get
            {
                //e.g. //catalog01/en_US/publicationid=tcm_0_70_1/schematitle=crafts2ebrand
                return base.DefaultLocation + "/schematitle=crafts2ebrand";
            }
        }

        public OurBrandsController(IAppSettings settings, IComponentFactory componentFactory, IKeywordRepository keywordrepository)
            : base(settings, componentFactory, keywordrepository)
        {
            _viewNameOverride = "~/Views/FacetedContent/Our-Brands.cshtml";
            _listParentTcmId = string.Format(base._settings.CraftListCategory, PublicationId);
            //_category = "techniques";
            _category = string.Format("{0}_techniques", settings.PublicationId);
        }

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

            return HandleIndexGet(componentPresentation);
        }

        [HttpPost, Level1BrandFilter]
        public ActionResult Index(ProductExplorer productExplorer, IComponentPresentation componentPresentation)
        {
            return HandleIndexPost(productExplorer, componentPresentation, "post");
        }
    }
}
