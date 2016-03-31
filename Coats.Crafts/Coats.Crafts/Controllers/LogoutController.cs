using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Extensions;

namespace Coats.Crafts.Controllers
{
    public class LogoutController : Controller
    {

        public ILogger Logger { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            Logger.Info("LogoutController.Index() GET ");

            FormsAuthentication.SignOut();
            Session.Abandon();
            //if (!Request.IsAjaxRequest())
            //{
                //Response.Redirect(FormsAuthentication.DefaultUrl.AddApplicationRoot());
            //}

            //return View();
            string url = FormsAuthentication.DefaultUrl.AddApplicationRoot();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Redirecting to url: {0}", url);

            return Redirect(url);
        }

    }
}
