using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Repositories.Tridion;
using Coats.Crafts.Models;
using Coats.Crafts.Repositories.Interfaces;
using DD4T.ContentModel;
using DD4T.Mvc.Controllers;
using Coats.Crafts.Extensions;
using Castle.Core.Logging;
using Coats.Crafts.Configuration;
using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Exceptions;
using Coats.Crafts.Filters;

namespace Coats.Crafts.Controllers
{
    public class CATSController : TridionControllerBase
    {
        public ILogger Logger { get; set; }
        private readonly ICatsRepository _catsrepository;
        private readonly IAppSettings _settings;

        public CATSController(ICatsRepository catsrepository, IAppSettings settings)
        {
            _catsrepository = catsrepository;
            _settings = settings;
        }

        //
        // GET: /CATS/
        [HttpGet]
        public ActionResult Index()
        {
            CatsContactForm form = new CatsContactForm();
            
            //if the validation failed then we'll have the contactform in TempData
            if (TempData["contactform"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                form = (CatsContactForm)TempData["contactform"];
                form.cp = base.GetComponentPresentation();
            }
            else
            {
                form.cp = base.GetComponentPresentation();
            }

            return View(form);
        }

        
        
        [HttpPost]
        [ContactActionFilter]
        public ActionResult Index(CatsContactForm form)
        {
            if (ModelState.IsValid)
            {
                string thanksUrl = _settings.CatsThankYou;
                form.dateSubmitted = DateTime.Now;
                _catsrepository.SaveCatsFormData(form);
                return Redirect(thanksUrl.AddApplicationRoot());
            }

            //validation failed so lets redirect back to contactus
            //contactactionfilter is responsible for returning the form and viewdata to the page

            TempData["contactform"] = form;
            return Redirect("/contactus");
        }

    }
}
