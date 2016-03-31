using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Basically a "getter" extension method for HttpResponseBase, similar to a property, but this is a method (you can't add extension properties)
        /// Firstly checks ServerVariables["HTTP_X_FORWARDED_FOR"], then, if null, checks ServerVariables["REMOTE_ADDR"].
        /// This probaby does exactly the same as UserHostAddress!
        /// </summary>
        /// <param name="context">The object that this extension method is bound to (HttpResponseBase)</param>
        /// <returns>The value of the item if present in the IDictionary, or false if there was a problem</returns>
        public static string GetClientIP(this HttpRequestBase request)
        {
            // Check for special Akamai HTTP Header first . ..
            string ip = request.ServerVariables["True-Client-IP"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }

            if (string.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

    }
}