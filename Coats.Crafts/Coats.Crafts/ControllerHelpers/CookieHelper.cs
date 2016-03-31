using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Coats.Crafts.ControllerHelpers
{
    public static class CookieHelper
    {
        public static void WriteFormsCookie(string username, string displayname, string firstname, string surname, string longitude, string latitude, string country)
        {
            string userData = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", displayname, firstname, surname, longitude, latitude, country);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    username,
                    DateTime.Now,
                    DateTime.Now.Add(FormsAuthentication.Timeout),
                    false,
                    userData,
                    FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }
    }
}