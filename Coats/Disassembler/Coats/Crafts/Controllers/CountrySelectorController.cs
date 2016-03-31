namespace Coats.Crafts.Controllers
{
    using Coats.Crafts;
    using Coats.Crafts.ControllerHelpers;
    using log4net;
    using System;
    using System.Web.Mvc;

    public class CountrySelectorController : Controller
    {
        public ActionResult RedirectCountry(string countryUrl)
        {
            if (base.User.Identity.IsAuthenticated)
            {
                ILog logger = LogManager.GetLogger(base.GetType());
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                CookieHelper.WriteFormsCookie(user.UserName, user.DISPLAYNAME, user.Firstname, user.Lastname, user.LONG, user.LAT, countryUrl);
                if (logger.IsDebugEnabled)
                {
                    logger.DebugFormat("CountrySelector Principal username: {0}", user.UserName ?? "");
                    logger.DebugFormat("CountrySelector Principal country: {0}", countryUrl ?? "");
                    logger.DebugFormat("CountrySelector Principal displayname: {0}", user.DISPLAYNAME ?? "");
                }
            }
            else
            {
                base.Response.Cookies["country"].Value = countryUrl;
                base.Response.Cookies["country"].Expires = DateTime.Now.AddDays(365.0);
            }
            return this.RedirectPermanent(countryUrl);
        }
    }
}

