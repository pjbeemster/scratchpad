using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Models;
using Coats.IndustrialPortal.Providers;
using System.Configuration;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Resources;
using Coats.Crafts.Extensions;
using Coats.Crafts.Configuration;

namespace Coats.Crafts.Controllers
{
    public class PasswordReminderController : Controller
    {
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PasswordReminder passwordreminder)
        {
            Boolean ajaxRequest = Request.IsAjaxRequest();
            Boolean allowRedirect = false;
            Boolean success = false;

            String message = String.Empty;
            String fbRedirect = String.Empty;

            Logger.Debug("PasswordReminderController > Index ");

            var isValid = ModelState.IsValid;

            Logger.DebugFormat("PasswordReminderController > isValid {0}", isValid);

            if (isValid)
            {
                if (passwordreminder.ReminderEmailAddress != null)
                {
                    try
                    {
                        var user = Membership.GetUser(passwordreminder.ReminderEmailAddress, false);

                        Logger.DebugFormat("PasswordReminderController > user {0}", (user == null));

                        if (user != null)
                        {
                            var coatsUserProfile = CoatsUserProfile.GetProfile(user.Email);
                            Logger.DebugFormat("PasswordReminderController > coatsUserProfile {0}", (coatsUserProfile == null));

                            var adminEmail = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                            var passwordReminderEmailTemplate = ConfigurationManager.AppSettings["PasswordReminderEmailTemplate"];

                            var util = new EmailUtility();

                            if (coatsUserProfile != null)
                            {
                                success = true;
                                message = Helper.GetResource("PasswordReminderSuccess");

                                string redirect;

                                if (Request.Url != null)
                                {
                                    redirect = Request.Url.Scheme + "://" + Request.Url.Host + (Url.Content(WebConfiguration.Current.Registration.AddApplicationRoot()));
                                } else {
                                    redirect = ConfigurationManager.AppSettings["SiteUrl"] + Url.Content(WebConfiguration.Current.Registration.AddApplicationRoot());
                                }

                                ResetPassword resetPw = new ResetPassword()
                                                            {
                                                                Password = coatsUserProfile.PASSWORD,
                                                                SiteUrl = redirect
                                                            };

                                var result = util.SendEmail(resetPw, passwordReminderEmailTemplate, adminEmail, user.Email);

                                Logger.DebugFormat("PasswordReminderController > result {0}", result);

                                if (string.IsNullOrEmpty(result) || !result.Equals("true"))
                                {
                                    success = false;
                                    message = Helper.GetResource("ResetPasswordFailure"); //"Sadly we couldn't reset your password, please try again.";

                                    ModelState.AddModelError("EmailAddress", "Errors.SendEmailError");
                                }
                            }
                        }

                        //Although the user is invalid we are displaying the same message to avoid user account sniffing.
                        success = true;
                        message = Helper.GetResource("PasswordReminderSuccess");

                        ViewBag.MessageSent = "true";
                    }

                    catch (Exception)
                    {
                        //ModelState.AddModelError("ModelStateKeys.RESETPASSWORDERROR", "new Exception(Resources.Errors.GetUserException, ex)");
                        //ModelState.AddModelError(ModelStateKeys.RESETPASSWORDERROR, new Exception(Resources.Errors.GetUserException, ex));

                        success = false;
                        message = Helper.GetResource("ResetPasswordFailure"); //"Sadly we couldn't reset your password, please try again.";
                    }
                }
                else
                {
                    success = false;
                    message = Helper.GetResource("ValidEmail"); //"Please enter a valid email address.";
                }
            }

            if (!isValid)
            {
                Logger.Debug("PasswordReminderController > isValid was false");

                var errors = ModelState
                  .Where(x => x.Value.Errors.Count > 0)
                  .Select(x => new { x.Key, x.Value.Errors })
                  .ToArray();

                success = false;
                message = Helper.GetResource("ValidEmail"); //"Please enter a valid email address.";
            }

            if (ajaxRequest)
            {
                return Json(
                    new
                    {
                        success = success,
                        allowRedirect = allowRedirect,
                        redirect = fbRedirect,
                        message = message
                    }
                );
            }

            return View();
        }
    }
}
