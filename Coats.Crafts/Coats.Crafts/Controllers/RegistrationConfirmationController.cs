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
using Coats.Crafts.NewsletterAPI;

namespace Coats.Crafts.Controllers
{
    public class RegistrationConfirmationController : TridionControllerBase
    {
        //
        // GET: /RegistrationConfirmation/

        public ILogger Logger { get; set; }
        private readonly IRegistrationRepository _registrationrepository;
        private readonly IAppSettings _settings;

        public RegistrationConfirmationController(IRegistrationRepository registrationrepository, IAppSettings settings)
        {
            _registrationrepository = registrationrepository;
            _settings = settings;
        }

        //
        // GET: /CATS/
        [HttpGet]
        public ActionResult Index()
        {
            RegistrationConfirmation RC = new RegistrationConfirmation();

            
            //Decrypt query string of Email address
            string ConfirmEmailAddress =string.Empty;
            // ConfirmEmailAddress = Request.QueryString["UserEmail"];
            ConfirmEmailAddress = Utils.General.Decrypt(HttpUtility.UrlDecode(Request.QueryString["UserEmail"]));

            RC.cp = base.GetComponentPresentation();

            RC.IsEmailExist = _registrationrepository.checkEmailAddressExists(ConfirmEmailAddress);
            if (RC.IsEmailExist)
            {
                PublicasterServiceRequest oPublicasterServiceRequest = new PublicasterServiceRequest();
                oPublicasterServiceRequest.ConfirmPublicaster(ConfirmEmailAddress);
                _registrationrepository.SaveRegisterData(ConfirmEmailAddress, "", GetClientIP());
            }

            return View(RC);
        }

        private string GetClientIP()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
       
    }
}
