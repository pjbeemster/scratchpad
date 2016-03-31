namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public class OurBrandsController : ProductBrandBaseController
    {
        public OurBrandsController(IAppSettings settings, IComponentFactory componentFactory, IKeywordRepository keywordrepository) : base(settings, componentFactory, keywordrepository)
        {
            base._viewNameOverride = "~/Views/FacetedContent/Our-Brands.cshtml";
            base._listParentTcmId = string.Format(base._settings.CraftListCategory, base.PublicationId);
            base._category = string.Format("{0}_techniques", settings.PublicationId);
        }

        [Level1BrandFilter, HttpPost]
        public ActionResult Index(ProductExplorer productExplorer, IComponentPresentation componentPresentation)
        {
            return base.HandleIndexPost(productExplorer, componentPresentation, "post");
        }

        [Level1BrandFilter]
        public ActionResult Index(IComponentPresentation componentPresentation, bool clear = false)
        {
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
            return base.HandleIndexGet(componentPresentation);
        }

        protected override string DefaultLocation
        {
            get
            {
                return (base.DefaultLocation + "/schematitle=crafts2ebrand");
            }
        }
    }
}

