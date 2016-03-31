using System.Web.Mvc;

using Coats.Crafts.Configuration;

using DD4T.ContentModel;
using DD4T.ContentModel.Factories;

using Castle.Core.Logging;
using Coats.Crafts.Repositories.Interfaces;
using DD4T.ContentModel.Exceptions;

namespace Coats.Crafts.Controllers
{
    public class FooterController : Controller
    {
        private readonly IComponentFactory _componentFactory;
        private readonly IAppSettings _settings;
        private readonly IKeywordRepository _keywordrepository;

        public ILogger Logger { get; set; }

        public FooterController(IComponentFactory componentFactory, IAppSettings settings, IKeywordRepository keywordrepository)
        {
            _componentFactory = componentFactory;
            _settings = settings;
            _keywordrepository = keywordrepository;
        }

        public int PublicationId
        {
            get
            {
                return _settings.PublicationId;
            }
        }

        //[ChildActionOnly]
        public ActionResult Index()
        {
            string footerLinksComponents = string.Format(_settings.FooterLinksComponents, PublicationId);

            // This would probably be _settings.FooterComponent or something
            IComponent footerComponent = null;
            try
            {
                footerComponent = _componentFactory.GetComponent(footerLinksComponents);
            }
            catch (ComponentNotFoundException cex)
            {
                Logger.ErrorFormat("Footer not found? {0} - for TCM {1}", cex, _settings.FooterLinksComponents);
            }


            if (footerComponent != null)
            {
                return PartialView("Footer", footerComponent);
            }

            // If no footer could be found.
            return Content("No footer");
        }

    }
}