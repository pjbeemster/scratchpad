namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Extensions;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Security;

    public class LogoutController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            this.Logger.Info("LogoutController.Index() GET ");
            FormsAuthentication.SignOut();
            base.Session.Abandon();
            string url = FormsAuthentication.DefaultUrl.AddApplicationRoot();
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("Redirecting to url: {0}", new object[] { url });
            }
            return this.Redirect(url);
        }

        public ILogger Logger { get; set; }
    }
}

