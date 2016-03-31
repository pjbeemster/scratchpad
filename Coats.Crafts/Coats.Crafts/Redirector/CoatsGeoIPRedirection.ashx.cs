using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Security;
using System.Text.RegularExpressions;

using GeoIP;
using log4net;
using System.Web.UI;

namespace Coats.Crafts.Redirector
{
    /// <summary>
    /// Summary description for GeoIPRedirection
    /// </summary>
    public class CoatsGeoIPRedirection : IHttpHandler
    {
        //private string google = @"google(.*)\.htm(l)?";
        //private string akamai = @"akamai-(.*)\.htm(l)?";
        //private string robots = @"robots.txt";
        //private string favicon = @"favicon.ico";

        private ILog log = LogManager.GetLogger(typeof(CoatsGeoIPRedirection));

        public void ProcessRequest(HttpContext context)
        {
            bool isFile = !String.IsNullOrEmpty(Path.GetExtension(context.Request.Path));

            log.DebugFormat("context.Request.Path: {0}", context.Request.Path);

            log.DebugFormat("isFile: {0}", Path.GetExtension(context.Request.Path));

            // Check is Google or Akamai verification requests
            if (log.IsDebugEnabled)
                log.DebugFormat("IsFile? {0}", isFile);

            bool serve = false;
            if (isFile)
            {           
                if (log.IsDebugEnabled)
                    log.DebugFormat("FilePath: {0}", context.Request.FilePath);

                // Get file patterns we;re checking for
                var file_checks = ConfigurationManager.GetSection("FileChecks") as NameValueCollection;
                if (log.IsDebugEnabled)
                    log.DebugFormat("Patterns?: {0}", file_checks.Keys.Count);

                foreach(string pattern_name in file_checks)
                {
                    string pattern = file_checks[pattern_name];
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Pattern: {0}", pattern);

                    if (Regex.IsMatch(context.Request.FilePath, pattern, RegexOptions.IgnoreCase))
                    {
                        if (log.IsDebugEnabled)
                            log.DebugFormat("Checking {0} against {1} = MATCH!", context.Request.FilePath, pattern);

                        serve = true;
                        break;
                    }
                }

                /*
                if (Regex.IsMatch(context.Request.FilePath, google, RegexOptions.IgnoreCase))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Checking {0} against {1} = MATCH!", context.Request.FilePath, google);

                    serve = true;
                }

                if (Regex.IsMatch(context.Request.FilePath, akamai, RegexOptions.IgnoreCase))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Checking {0} against {1} = MATCH!", context.Request.FilePath, akamai);

                    serve = true;
                }

                if (Regex.IsMatch(context.Request.FilePath, robots, RegexOptions.IgnoreCase))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Checking {0} against {1} = MATCH!", context.Request.FilePath, robots);

                    serve = true;
                }
                */
            }

            if (serve)
            {
                this.HandleKnownFile(context);
            }
            else
            {
                this.HandleRedirection(context);
            }

           
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void HandleKnownFile(HttpContext context)
        {
            PageHandlerFactory factory =
                (PageHandlerFactory)System.Runtime.Serialization.FormatterServices
                    .GetUninitializedObject(typeof(System.Web.UI.PageHandlerFactory));

            var handler = factory.GetHandler(context, "GET", context.Request.Path, context.Request.MapPath(context.Request.Path));

            context.Handler = handler;
            handler.ProcessRequest(context);
        }

        private void HandleRedirection(HttpContext context)
        {
            //--------------------------------------------------------------------------------------------
            // If the user is authenticated and has selected a country from the country selector then
            // we redirect to the chosen country
            //--------------------------------------------------------------------------------------------

            if (HttpContext.Current.User.Identity.IsAuthenticated && context.Request.QueryString["ip"] == null)
            {
                var principal = (MvcApplication.CraftsPrincipal)HttpContext.Current.User;

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("GeoIPRedirection Principal username: {0}", principal.UserName ?? "");
                    log.DebugFormat("GeoIPRedirection Principal country: {0}", principal.COUNTRY ?? "");
                    log.DebugFormat("GeoIPRedirection Principal displayname: {0}", principal.DISPLAYNAME ?? "");
                }

                string selectedCountry = principal.COUNTRY;

                if (log.IsDebugEnabled)
                    log.DebugFormat("user country selected ? {0}", (selectedCountry == null));

                if (!string.IsNullOrEmpty(selectedCountry))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("user country selected redirected ? {0}", (selectedCountry == null));

                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    TimeSpan ts = new TimeSpan(0, 0, 0);
                    context.Response.Cache.SetMaxAge(ts);
                    //context.Response.Redirect(selectedCountry);
                    context.Response.RedirectPermanent(selectedCountry);
                }
            } else {

                log.Debug("Redirection not authenticated");
                if (context.Request.Cookies["country"] != null)
                {
                    log.DebugFormat("user country cookie redirected: {0}", context.Request.Cookies["country"].Value);
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    TimeSpan ts = new TimeSpan(0, 0, 0);
                    context.Response.Cache.SetMaxAge(ts);
                    //context.Response.Redirect(context.Request.Cookies["country"].Value);
                    context.Response.RedirectPermanent(context.Request.Cookies["country"].Value);
                }
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

            // Get the client IP
            string client = context.Request.GetClientIP();

#if DEBUG

#endif
            //Override the client IP with a querystring version for testing
            if (context.Request.QueryString["ip"] != null)
            {
                client = context.Request.QueryString["ip"];
            }

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

            
            //CCPLCR-4 - client asked could url not have lang=1 on the end after redirect, so detecting this is the homepage
            if (context.Request.Url.AbsolutePath == "/")
            {
                //Commented by Ajaya and added below line on 22-7-14
                //context.Response.RedirectPermanent(url);
                context.Response.Redirect(url);
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
        /// <returns>The value of the item if present in the IDictionary, or false if there was a problem</returns>
        public static string GetClientIP(this HttpRequest request)
        {
            ILog log = LogManager.GetLogger(typeof(CoatsGeoIPRedirection));

			log.Debug("CoatsGeoIPRedirection GetClientIP()");

            // Check for special Akamai HTTP Header first . ..
            string ip = request.ServerVariables["True-Client-IP"];

            if (!string.IsNullOrEmpty(ip))
            {
                log.DebugFormat("True-Client-IP: {0}", ip.ToString());
            }
            else
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];                
            }

            if (!string.IsNullOrEmpty(ip))
            {
                log.DebugFormat("HTTP_X_FORWARDED_FOR: {0}", ip.ToString());
            }
            else
            {
                ip = request.ServerVariables["REMOTE_ADDR"];                
            }

            if (!string.IsNullOrEmpty(ip))
            {
                log.DebugFormat("REMOTE_ADDR: {0}", ip.ToString());
            }

            // No IP in any of the headers ...
            if (string.IsNullOrEmpty(ip))
            {
                ip = String.Empty;
                log.Debug("IP EMPTY");
            }

            // If requests go via any transaparent proxies ip could contain several address
            // See http://en.wikipedia.org/wiki/X-Forwarded-For - X-Forwarded-For: client, proxy1, proxy2
            // Need to split on ", " to grab the first one. 
            string[] ips = ip.Split(", ".ToCharArray());

            if (ips.Length > 0)
            {
                // Splitting an multiple chars that are adjecent might result in an empty array element
                ip = ips.FirstOrDefault<string>(address => !String.IsNullOrEmpty(address));
            
                // The above vould be null if no IP can be determined
                if (!string.IsNullOrEmpty(ip))            
                    log.DebugFormat("Using: {0}", ip.ToString());
            }

            return ip;
        }

    }
}