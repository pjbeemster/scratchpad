namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class CATSController : TridionControllerBase
    {
        private readonly ICatsRepository _catsrepository;
        private readonly IAppSettings _settings;

        public CATSController(ICatsRepository catsrepository, IAppSettings settings)
        {
            this._catsrepository = catsrepository;
            this._settings = settings;
        }

        [HttpGet]
        public ActionResult Index()
        {
            CatsContactForm model = new CatsContactForm();
            if (base.TempData["contactform"] != null)
            {
                base.ViewData = (ViewDataDictionary) base.TempData["ViewData"];
                model = (CatsContactForm) base.TempData["contactform"];
                model.cp = base.GetComponentPresentation();
            }
            else
            {
                model.cp = base.GetComponentPresentation();
            }
            return base.View(model);
        }

        [HttpPost, ContactActionFilter]
        public ActionResult Index(CatsContactForm form)
        {
            if (base.ModelState.IsValid)
            {
                string catsThankYou = this._settings.CatsThankYou;
                form.dateSubmitted = DateTime.Now;
                this._catsrepository.SaveCatsFormData(form);
                return this.Redirect(catsThankYou.AddApplicationRoot());
            }
            base.TempData["contactform"] = form;
            return this.Redirect("/contactus");
        }

        public ILogger Logger { get; set; }
    }
}

