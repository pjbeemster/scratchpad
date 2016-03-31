namespace Coats.Crafts.ControllerHelpers
{
    using System;
    using System.Web;
    using System.Web.Security;

    public static class CookieHelper
    {
        public static void WriteFormsCookie(string username, string displayname, string firstname, string surname, string longitude, string latitude, string country)
        {
            string userData = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", new object[] { displayname, firstname, surname, longitude, latitude, country });
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), false, userData, FormsAuthentication.FormsCookiePath);
            string str2 = FormsAuthentication.Encrypt(ticket);
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, str2));
        }
    }
}

