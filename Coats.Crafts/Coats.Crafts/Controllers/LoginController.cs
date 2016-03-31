using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Models;
using Coats.Crafts.Extensions;

namespace Coats.Crafts.Controllers
{
    public class LoginController : Controller
    {
        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            Logger.Info("LoginController.Index() GET ");

            Login model = new Login();
            ViewBag.RememberMe = false;

            HttpCookie authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    ViewBag.RememberMe = true;
                    Response.Redirect(FormsAuthentication.DefaultUrl.AddApplicationRoot());
                }

            }
            return View(model);            
        }


        //[ChildActionOnly, HttpPost]
        [HttpPost]
        public ActionResult Index(Login model)
        {
            Logger.Info(string.Format("LoginController.Index() POST  is Valid={0} ", ModelState.IsValid));

            if (ModelState.IsValid)
            {
                try
                {
                    var isValid = IsValidUser(model);

                    if (isValid)
                    {
                        var rememberMe = Convert.ToBoolean(Request["rememberMe"]);

                        FormsAuthentication.SetAuthCookie(model.EmailAddress, rememberMe);

                        Logger.Info("LoginController.Index() POST  Success Pre-returning ");

                        Logger.InfoFormat("LoginController.Index() Request.IsAjaxRequest() {0}", Request.IsAjaxRequest());

                        if (Request.IsAjaxRequest())
                        {
                            //return Content(UtilityHelper.WrapUrl(WebConfiguration.Current.LoginSuccess, "success"));
                        }

                        //non js
                        //var returnUrl = Request["ReturnUrl"];
                        Logger.DebugFormat("Return url: {0}", model.ReturnUrl);

                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            if (!model.ReturnUrl.Contains(model.ReturnUrl))
                            {
                                // NG - The Response.Redirect was commented out, not sure why?
                                // We want to go back to the return url?
                                Response.Redirect(model.ReturnUrl, true);
                                //return View(returnUrl, model);
                            }
                        }
                    }

                    //ModelState.AddModelError(string.Empty, Errors.LoginTryAgain);

                }
                catch (Exception ex)
                {
                    Logger.Info(string.Format("LoginController.Index() POST  Failed error={0} ", ex.Message));
                    //ModelState.AddModelError(ModelStateKeys.LOGINERROR, ex);
                }
            }

            return View(model);
        }


        public static bool IsValidUser(Login model)
        {
            var isValid = Membership.ValidateUser(model.EmailAddress, model.Password);
            return isValid;
        }

    }
}
