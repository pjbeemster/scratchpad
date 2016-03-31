using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;

using GeoIP;
using log4net;

namespace Redirector
{
    /// <summary>
    /// Summary description for GeoIPRedirection
    /// </summary>
    public class GeoIPRedirection : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            ILog log = LogManager.GetLogger(this.GetType());

            HttpModuleCollection modules = HttpContext.Current.ApplicationInstance.Modules;
            foreach (string moduleKey in modules.Keys)
            {
                IHttpModule module = modules[moduleKey];
                log.DebugFormat("Module: {0}", module.ToString());
                // Do your check here
            }

            // Gate check for data
            string data = ConfigurationManager.AppSettings["GeoData"];
            if (String.IsNullOrEmpty(data))
                throw new NullReferenceException("Need a data file defined in web.config!!");

            if (log.IsDebugEnabled)
                log.DebugFormat("data {0}", data);

            string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            string dataFile = Path.Combine(dataDirectory, data);

            if (log.IsDebugEnabled)
                log.DebugFormat("dataFile {0}", dataFile);

            bool exists = File.Exists(dataFile);
            if (log.IsDebugEnabled)
                log.DebugFormat("file exists? {0}", exists);

            // Gate check for default
            string defaultCode = ConfigurationManager.AppSettings["Default"];
            if (String.IsNullOrEmpty(defaultCode))
                throw new NullReferenceException("Need a default country defined in web.config!!");

            if (log.IsDebugEnabled)
                log.DebugFormat("defaultCode {0}", defaultCode);

            // Gate check for config section
            var settings = ConfigurationManager.GetSection("CountryUrls") as Hashtable;
            if (settings == null)
                throw new NullReferenceException("Need a CountryUrls section in web.config that defines redirections!!");

            // Start with the default url
            string url = settings[defaultCode].ToString();

            if (log.IsDebugEnabled)
                log.DebugFormat("url {0}", url);

            // Check IP
#if DEBUG
            string client = "24.24.24.24";
#else
            string client = context.Request.GetClientIP();
#endif

            if (log.IsDebugEnabled)
                log.DebugFormat("client {0}", client);

            if (!String.IsNullOrEmpty(client) && exists)
            {
                LookupService lookup = new LookupService(dataFile, LookupService.GEOIP_MEMORY_CACHE);
                Country country = lookup.getCountry(client);

                if (log.IsDebugEnabled)
                    log.DebugFormat("country? {0}", (country == null));

                if (country != null)
                {
                    string code = country.getCode();

                    if (log.IsDebugEnabled)
                        log.DebugFormat("code? {0}", code);

                    if (settings.ContainsKey(code))
                        url = settings[code].ToString();
                }
            }

            if (String.IsNullOrEmpty(url))
                throw new NullReferenceException("No url to redirect to!!");

            if (log.IsDebugEnabled)
                log.DebugFormat("url {0}", url);

            //context.Response.RedirectPermanent(url);
            context.Response.Redirect(url);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Basically a "getter" extension method for HttpResponseBase, similar to a property, but this is a method (you can't add extension properties)
        /// Firstly checks ServerVariables["HTTP_X_FORWARDED_FOR"], then, if null, checks ServerVariables["REMOTE_ADDR"].
        /// This probaby does exactly the same as UserHostAddress!
        /// </summary>
        /// <param name="context">The object that this extension method is bound to (HttpResponseBase)</param>
        /// <returns>The value of the item if present in the IDictionary, or false if there was a problem</returns>
        public static string GetClientIP(this HttpRequest request)
        {
            //string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

    }
}