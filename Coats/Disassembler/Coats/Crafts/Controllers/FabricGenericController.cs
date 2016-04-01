namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class FabricGenericController : Controller
    {
        protected IComponentFactory _componentFactory;
        private readonly IKeywordRepository _keywordrepository;
        protected IAppSettings _settings;

        public FabricGenericController(IComponentFactory componentFactory, IAppSettings settings, IKeywordRepository keywordrepository)
        {
            this._settings = settings;
            this._componentFactory = componentFactory;
            this._keywordrepository = keywordrepository;
        }

        private IPage GetPageInfo(string tcm)
        {
            IContainerAccessor applicationInstance = System.Web.HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IPageFactory>().GetPage(tcm);
        }

        public ActionResult Index()
        {
            this.Logger.Info("FabricGenericController.Index() GET ");
            FabricComponentList model = new FabricComponentList
            {
                Components = new List<IComponent>()
            };
            string tcm = string.Format(this._settings.FabricPage, this._settings.PublicationId);
            IList<IComponentPresentation> componentPresentations = this.GetPageInfo(tcm).ComponentPresentations;
            foreach (IComponentPresentation presentation in componentPresentations)
            {
                model.Components.Add(presentation.Component);
            }
            try
            {
                string str2 = string.Empty;
                if (base.Request.QueryString["keyword_clicked"] != null)
                {
                    str2 = base.Request.QueryString["keyword_clicked"];
                    foreach (IComponent component in model.Components)
                    {
                        if ("color".Equals(((TridionItem)component).Title, StringComparison.CurrentCultureIgnoreCase) && (base.Request.QueryString["keyword_clicked"] == "4478-512"))
                        {
                            model.SelectedComponent = component;
                        }
                        else if ("designer".Equals(((TridionItem)component).Title, StringComparison.CurrentCultureIgnoreCase) && (base.Request.QueryString["keyword_clicked"] == "4465-512"))
                        {
                            model.SelectedComponent = component;
                        }
                        else if ("fabric type".Equals(((TridionItem)component).Title, StringComparison.CurrentCultureIgnoreCase) && (base.Request.QueryString["keyword_clicked"] == "4489-512"))
                        {
                            model.SelectedComponent = component;
                        }
                        else if ("theme".Equals(((TridionItem)component).Title, StringComparison.CurrentCultureIgnoreCase) && (base.Request.QueryString["keyword_clicked"] == "4469-512"))
                        {
                            model.SelectedComponent = component;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.Info($"FabricGenericController.Index() POST  Failed error={exception.Message} ");
            }
            return this.PartialView("FabricGeneric", model);
        }

        public static bool IsValidUser(Login model) =>
            Membership.ValidateUser(model.EmailAddress, model.Password);

        public ILogger Logger { get; set; }
    }
}
