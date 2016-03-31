namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.UI;

    public class RemoteValidationController : Controller
    {
        private readonly IRegistrationRepository _registrationrepository;

        public RemoteValidationController(IRegistrationRepository registrationrepository)
        {
            this._registrationrepository = registrationrepository;
        }

        [ValidateInput(false), HttpPost, OutputCache(Location=OutputCacheLocation.None, NoStore=true)]
        public JsonResult IsDisplayNameAvailable([Bind(Prefix="RegistrationForm.CustomerDetails.DisplayName")] string displayname)
        {
            if (displayname != null)
            {
                if (this._registrationrepository.checkDisplayNameExists(displayname))
                {
                    return base.Json(Helper.GetResource("ProblemDisplayNameExists"), JsonRequestBehavior.AllowGet);
                }
                return base.Json(true, JsonRequestBehavior.AllowGet);
            }
            return base.Json(Helper.GetResource("ProblemRegisteredEmail"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, OutputCache(Location=OutputCacheLocation.None, NoStore=true)]
        public JsonResult IsEmailAvailable([Bind(Prefix="RegistrationForm.CustomerDetails.EmailAddress")] string emailAddress)
        {
            if (emailAddress != null)
            {
                if (Membership.GetUser(emailAddress) == null)
                {
                    return base.Json(true, JsonRequestBehavior.AllowGet);
                }
                return base.Json(Helper.GetResource("EmailAlreadyRegistered"), JsonRequestBehavior.AllowGet);
            }
            return base.Json(Helper.GetResource("ProblemRegisteredEmail"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, OutputCache(Location=OutputCacheLocation.None, NoStore=true)]
        public JsonResult IsValidUser([Bind(Prefix="PasswordReminder.ReminderEmailAddress")] string reminderEmailAddress)
        {
            if (Membership.GetUser(reminderEmailAddress, false) == null)
            {
                return base.Json(Helper.GetResource("UnableToCreateNewUser"), JsonRequestBehavior.AllowGet);
            }
            return base.Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}

