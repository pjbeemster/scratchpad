namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using Coats.Crafts.NewsletterAPI;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Utils;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;

    public class RegistrationConfirmationController : TridionControllerBase
    {
        private readonly IRegistrationRepository _registrationrepository;
        private readonly IAppSettings _settings;

        public RegistrationConfirmationController(IRegistrationRepository registrationrepository, IAppSettings settings)
        {
            this._registrationrepository = registrationrepository;
            this._settings = settings;
        }

        private string GetClientIP()
        {
            HttpContext current = HttpContext.Current;
            string str = current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(str))
            {
                str = current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return str;
        }

        [HttpGet]
        public ActionResult Index()
        {
            RegistrationConfirmation model = new RegistrationConfirmation();
            string registeredEmailAddress = string.Empty;
            registeredEmailAddress = General.Decrypt(HttpUtility.UrlDecode(base.Request.QueryString["UserEmail"]));
            model.cp = base.GetComponentPresentation();
            model.IsEmailExist = this._registrationrepository.checkEmailAddressExists(registeredEmailAddress);
            if (model.IsEmailExist)
            {
                new PublicasterServiceRequest().ConfirmPublicaster(registeredEmailAddress);
                this._registrationrepository.SaveRegisterData(registeredEmailAddress, "", this.GetClientIP());
            }
            return base.View(model);
        }

        public ILogger Logger { get; set; }
    }
}

