namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Models;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            this.Logger.Info("LoginController.Index() GET ");
            Login model = new Login();
            ((dynamic) base.ViewBag).RememberMe = false;
            HttpCookie cookie = base.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if ((cookie != null) && !string.IsNullOrEmpty(cookie.Value))
            {
                ((dynamic) base.ViewBag).RememberMe = true;
                base.Response.Redirect(FormsAuthentication.DefaultUrl.AddApplicationRoot());
            }
            return base.View(model);
        }

        [HttpPost]
        public ActionResult Index(Login model)
        {
            this.Logger.Info(string.Format("LoginController.Index() POST  is Valid={0} ", base.ModelState.IsValid));
            if (base.ModelState.IsValid)
            {
                try
                {
                    if (IsValidUser(model))
                    {
                        bool createPersistentCookie = Convert.ToBoolean(base.Request["rememberMe"]);
                        FormsAuthentication.SetAuthCookie(model.EmailAddress, createPersistentCookie);
                        this.Logger.Info("LoginController.Index() POST  Success Pre-returning ");
                        this.Logger.InfoFormat("LoginController.Index() Request.IsAjaxRequest() {0}", new object[] { base.Request.IsAjaxRequest() });
                        if (base.Request.IsAjaxRequest())
                        {
                        }
                        this.Logger.DebugFormat("Return url: {0}", new object[] { model.ReturnUrl });
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && !model.ReturnUrl.Contains(model.ReturnUrl))
                        {
                            base.Response.Redirect(model.ReturnUrl, true);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.Info(string.Format("LoginController.Index() POST  Failed error={0} ", exception.Message));
                }
            }
            return base.View(model);
        }

        public static bool IsValidUser(Login model)
        {
            return Membership.ValidateUser(model.EmailAddress, model.Password);
        }

        public ILogger Logger { get; set; }
    }
}

