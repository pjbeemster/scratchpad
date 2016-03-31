namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Exceptions;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class FooterController : Controller
    {
        private readonly IComponentFactory _componentFactory;
        private readonly IKeywordRepository _keywordrepository;
        private readonly IAppSettings _settings;

        public FooterController(IComponentFactory componentFactory, IAppSettings settings, IKeywordRepository keywordrepository)
        {
            this._componentFactory = componentFactory;
            this._settings = settings;
            this._keywordrepository = keywordrepository;
        }

        public ActionResult Index()
        {
            string componentUri = string.Format(this._settings.FooterLinksComponents, this.PublicationId);
            IComponent model = null;
            try
            {
                model = this._componentFactory.GetComponent(componentUri);
            }
            catch (ComponentNotFoundException exception)
            {
                this.Logger.ErrorFormat("Footer not found? {0} - for TCM {1}", new object[] { exception, this._settings.FooterLinksComponents });
            }
            if (model != null)
            {
                return this.PartialView("Footer", model);
            }
            return base.Content("No footer");
        }

        public ILogger Logger { get; set; }

        public int PublicationId
        {
            get
            {
                return this._settings.PublicationId;
            }
        }
    }
}

