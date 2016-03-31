using System;
using System.Web.UI;
using System.Web.Mvc;
using System.Web.Security;
using Coats.Crafts.Resources;
using Coats.Crafts.Repositories.Interfaces;


namespace Coats.Crafts.Controllers
{
    public class RemoteValidationController : Controller
    {

        private readonly IRegistrationRepository _registrationrepository;

        public RemoteValidationController(IRegistrationRepository registrationrepository)
        {
            _registrationrepository = registrationrepository;
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult IsValidUser([Bind(Prefix = "PasswordReminder.ReminderEmailAddress")] String reminderEmailAddress)
        {
            var user = Membership.GetUser(reminderEmailAddress, false);

            if (user == null)
            {
                // Invalid user, return error string
                //return Json("Invalid email address, please try again.", JsonRequestBehavior.AllowGet);
                return Json(Helper.GetResource("UnableToCreateNewUser"), JsonRequestBehavior.AllowGet);
            }
            // Valid user, return true
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult IsEmailAvailable([Bind(Prefix = "RegistrationForm.CustomerDetails.EmailAddress")] String emailAddress)
        {
            if (emailAddress != null)
            {
                MembershipUser mUser = Membership.GetUser(emailAddress);

                if (mUser == null)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(Helper.GetResource("EmailAlreadyRegistered"), JsonRequestBehavior.AllowGet);
            }

            return Json(Helper.GetResource("ProblemRegisteredEmail"), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [ValidateInput(false)]
        public JsonResult IsDisplayNameAvailable([Bind(Prefix = "RegistrationForm.CustomerDetails.DisplayName")] String displayname)
        {

            if (displayname != null)
            {
                if (_registrationrepository.checkDisplayNameExists(displayname))
                {
                    return Json(Helper.GetResource("ProblemDisplayNameExists"), JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(Helper.GetResource("ProblemRegisteredEmail"), JsonRequestBehavior.AllowGet);

        }

    }
}
