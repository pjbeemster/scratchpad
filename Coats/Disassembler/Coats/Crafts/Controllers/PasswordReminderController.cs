namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Models;
    using Coats.Crafts.Resources;
    using Coats.IndustrialPortal.Providers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Security;

    public class PasswordReminderController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return base.View();
        }

        [HttpPost]
        public ActionResult Index(PasswordReminder passwordreminder)
        {
            bool flag = base.Request.IsAjaxRequest();
            bool flag2 = false;
            bool flag3 = false;
            string resource = string.Empty;
            string str2 = string.Empty;
            this.Logger.Debug("PasswordReminderController > Index ");
            bool isValid = base.ModelState.IsValid;
            this.Logger.DebugFormat("PasswordReminderController > isValid {0}", new object[] { isValid });
            if (isValid)
            {
                if (passwordreminder.ReminderEmailAddress != null)
                {
                    try
                    {
                        MembershipUser user = Membership.GetUser(passwordreminder.ReminderEmailAddress, false);
                        this.Logger.DebugFormat("PasswordReminderController > user {0}", new object[] { user == null });
                        if (user != null)
                        {
                            CoatsUserProfile profile = CoatsUserProfile.GetProfile(user.Email);
                            this.Logger.DebugFormat("PasswordReminderController > coatsUserProfile {0}", new object[] { profile == null });
                            string fromEmailAddress = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                            string emailTemplate = ConfigurationManager.AppSettings["PasswordReminderEmailTemplate"];
                            EmailUtility utility = new EmailUtility();
                            if (profile != null)
                            {
                                string str5;
                                flag3 = true;
                                resource = Helper.GetResource("PasswordReminderSuccess");
                                if (base.Request.Url != null)
                                {
                                    str5 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + base.Url.Content(WebConfiguration.Current.Registration.AddApplicationRoot());
                                }
                                else
                                {
                                    str5 = ConfigurationManager.AppSettings["SiteUrl"] + base.Url.Content(WebConfiguration.Current.Registration.AddApplicationRoot());
                                }
                                ResetPassword model = new ResetPassword {
                                    Password = profile.PASSWORD,
                                    SiteUrl = str5
                                };
                                string str6 = utility.SendEmail(model, emailTemplate, fromEmailAddress, user.Email);
                                this.Logger.DebugFormat("PasswordReminderController > result {0}", new object[] { str6 });
                                if (!(!string.IsNullOrEmpty(str6) && str6.Equals("true")))
                                {
                                    flag3 = false;
                                    resource = Helper.GetResource("ResetPasswordFailure");
                                    base.ModelState.AddModelError("EmailAddress", "Errors.SendEmailError");
                                }
                            }
                        }
                        flag3 = true;
                        resource = Helper.GetResource("PasswordReminderSuccess");
                        ((dynamic) base.ViewBag).MessageSent = "true";
                    }
                    catch (Exception)
                    {
                        flag3 = false;
                        resource = Helper.GetResource("ResetPasswordFailure");
                    }
                }
                else
                {
                    flag3 = false;
                    resource = Helper.GetResource("ValidEmail");
                }
            }
            if (!isValid)
            {
                this.Logger.Debug("PasswordReminderController > isValid was false");
                var typeArray = (from x in base.ModelState
                    where x.Value.Errors.Count > 0
                    select new { 
                        Key = x.Key,
                        Errors = x.Value.Errors
                    }).ToArray();
                flag3 = false;
                resource = Helper.GetResource("ValidEmail");
            }
            if (flag)
            {
                return base.Json(new { 
                    success = flag3,
                    allowRedirect = flag2,
                    redirect = str2,
                    message = resource
                });
            }
            return base.View();
        }

        public ILogger Logger { get; set; }
    }
}

