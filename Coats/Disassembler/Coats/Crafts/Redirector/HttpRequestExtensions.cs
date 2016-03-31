namespace Coats.Crafts.Redirector
{
    using log4net;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;

    public static class HttpRequestExtensions
    {
        public static string GetClientIP(this HttpRequest request)
        {
            ILog logger = LogManager.GetLogger(typeof(CoatsGeoIPRedirection));
            logger.Debug("CoatsGeoIPRedirection GetClientIP()");
            string str = request.ServerVariables["True-Client-IP"];
            if (!string.IsNullOrEmpty(str))
            {
                logger.DebugFormat("True-Client-IP: {0}", str.ToString());
            }
            else
            {
                str = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (!string.IsNullOrEmpty(str))
            {
                logger.DebugFormat("HTTP_X_FORWARDED_FOR: {0}", str.ToString());
            }
            else
            {
                str = request.ServerVariables["REMOTE_ADDR"];
            }
            if (!string.IsNullOrEmpty(str))
            {
                logger.DebugFormat("REMOTE_ADDR: {0}", str.ToString());
            }
            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
                logger.Debug("IP EMPTY");
            }
            string[] source = str.Split(", ".ToCharArray());
            if (source.Length > 0)
            {
                str = source.FirstOrDefault<string>(address => !string.IsNullOrEmpty(address));
                if (!string.IsNullOrEmpty(str))
                {
                    logger.DebugFormat("Using: {0}", str.ToString());
                }
            }
            return str;
        }
    }
}

