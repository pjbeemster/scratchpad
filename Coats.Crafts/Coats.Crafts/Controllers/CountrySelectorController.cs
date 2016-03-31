using System.Web.Mvc;
using Coats.Crafts.ControllerHelpers;
using log4net;
using System;

namespace Coats.Crafts.Controllers
{
    public class CountrySelectorController : Controller
    {
        public ActionResult RedirectCountry(string countryUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                ILog log = LogManager.GetLogger(GetType());

                var principal = (MvcApplication.CraftsPrincipal)HttpContext.User;
                CookieHelper.WriteFormsCookie(principal.UserName, principal.DISPLAYNAME, principal.Firstname,
                                              principal.Lastname, principal.LONG, principal.LAT, countryUrl);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("CountrySelector Principal username: {0}", principal.UserName ?? "");
                    log.DebugFormat("CountrySelector Principal country: {0}", countryUrl ?? "");
                    log.DebugFormat("CountrySelector Principal displayname: {0}", principal.DISPLAYNAME ?? "");
                }

            }
            else
            {
                Response.Cookies["country"].Value = countryUrl;
                Response.Cookies["country"].Expires = DateTime.Now.AddDays(365);
            }

            return RedirectPermanent(countryUrl);
        }

    }
}
