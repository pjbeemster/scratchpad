namespace Coats.Crafts.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;

    public static class HttpRequestBaseExtensions
    {
        public static string GetClientIP(this HttpRequestBase request)
        {
            string str = request.ServerVariables["True-Client-IP"];
            if (string.IsNullOrEmpty(str))
            {
                str = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (string.IsNullOrEmpty(str))
            {
                str = request.ServerVariables["REMOTE_ADDR"];
            }
            return str;
        }
    }
}

