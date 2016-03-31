using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Utils
{
    public class TokenAdapter
    {

        /// <summary>
        /// Used for email template
        /// </summary>
        /// <param name="tridionToken"></param>
        /// <returns></returns>
        public static string ReplaceToken(string tridionToken)
        {
            string templateToken = tridionToken.Replace("[*", "<#");
            templateToken = templateToken.Replace("*]", "#>");
            return templateToken;
        }
    }
}