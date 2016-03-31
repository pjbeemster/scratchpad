namespace Coats.Crafts.Redirector
{
    using Coats.Crafts;
    using GeoIP;
    using log4net;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    public class CoatsGeoIPRedirection : IHttpHandler
    {
        private ILog log = LogManager.GetLogger(typeof(CoatsGeoIPRedirection));

        private void HandleKnownFile(HttpContext context)
        {
            IHttpHandler handler = ((PageHandlerFactory) FormatterServices.GetUninitializedObject(typeof(PageHandlerFactory))).GetHandler(context, "GET", context.Request.Path, context.Request.MapPath(context.Request.Path));
            context.Handler = handler;
            handler.ProcessRequest(context);
        }

        private void HandleRedirection(HttpContext context)
        {
            TimeSpan span;
            if (HttpContext.Current.User.Identity.IsAuthenticated && (context.Request.QueryString["ip"] == null))
            {
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) HttpContext.Current.User;
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("GeoIPRedirection Principal username: {0}", user.UserName ?? "");
                    this.log.DebugFormat("GeoIPRedirection Principal country: {0}", user.COUNTRY ?? "");
                    this.log.DebugFormat("GeoIPRedirection Principal displayname: {0}", user.DISPLAYNAME ?? "");
                }
                string cOUNTRY = user.COUNTRY;
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("user country selected ? {0}", cOUNTRY == null);
                }
                if (!string.IsNullOrEmpty(cOUNTRY))
                {
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.DebugFormat("user country selected redirected ? {0}", cOUNTRY == null);
                    }
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    span = new TimeSpan(0, 0, 0);
                    context.Response.Cache.SetMaxAge(span);
                    context.Response.RedirectPermanent(cOUNTRY);
                }
            }
            else
            {
                this.log.Debug("Redirection not authenticated");
                if (context.Request.Cookies["country"] != null)
                {
                    this.log.DebugFormat("user country cookie redirected: {0}", context.Request.Cookies["country"].Value);
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    span = new TimeSpan(0, 0, 0);
                    context.Response.Cache.SetMaxAge(span);
                    context.Response.RedirectPermanent(context.Request.Cookies["country"].Value);
                }
            }
            string str2 = ConfigurationManager.AppSettings["GeoData"];
            if (string.IsNullOrEmpty(str2))
            {
                throw new NullReferenceException("Need a data file defined in web.config!!");
            }
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("data {0}", str2);
            }
            string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            string str4 = Path.Combine(data, str2);
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("dataFile {0}", str4);
            }
            bool flag = File.Exists(str4);
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("file exists? {0}", flag);
            }
            string str5 = ConfigurationManager.AppSettings["Default"];
            if (string.IsNullOrEmpty(str5))
            {
                throw new NullReferenceException("Need a default country defined in web.config!!");
            }
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("defaultCode {0}", str5);
            }
            Hashtable section = ConfigurationManager.GetSection("CountryUrls") as Hashtable;
            if (section == null)
            {
                throw new NullReferenceException("Need a CountryUrls section in web.config that defines redirections!!");
            }
            string str6 = section[str5].ToString();
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("url {0}", str6);
            }
            string clientIP = context.Request.GetClientIP();
            if (context.Request.QueryString["ip"] != null)
            {
                clientIP = context.Request.QueryString["ip"];
            }
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("client {0}", clientIP);
            }
            if (!string.IsNullOrEmpty(clientIP) && flag)
            {
                Country country = new LookupService(str4, LookupService.GEOIP_MEMORY_CACHE).getCountry(clientIP);
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("country? {0}", country == null);
                }
                if (country != null)
                {
                    string str8 = country.getCode();
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.DebugFormat("code? {0}", str8);
                    }
                    if (section.ContainsKey(str8))
                    {
                        str6 = section[str8].ToString();
                    }
                }
            }
            if (string.IsNullOrEmpty(str6))
            {
                throw new NullReferenceException("No url to redirect to!!");
            }
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("url {0}", str6);
            }
            if (context.Request.Url.AbsolutePath == "/")
            {
                context.Response.Redirect(str6);
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            bool flag = !string.IsNullOrEmpty(Path.GetExtension(context.Request.Path));
            this.log.DebugFormat("context.Request.Path: {0}", context.Request.Path);
            this.log.DebugFormat("isFile: {0}", Path.GetExtension(context.Request.Path));
            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("IsFile? {0}", flag);
            }
            bool flag2 = false;
            if (flag)
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("FilePath: {0}", context.Request.FilePath);
                }
                NameValueCollection section = ConfigurationManager.GetSection("FileChecks") as NameValueCollection;
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("Patterns?: {0}", section.Keys.Count);
                }
                foreach (string str in section)
                {
                    string str2 = section[str];
                    if (this.log.IsDebugEnabled)
                    {
                        this.log.DebugFormat("Pattern: {0}", str2);
                    }
                    if (Regex.IsMatch(context.Request.FilePath, str2, RegexOptions.IgnoreCase))
                    {
                        if (this.log.IsDebugEnabled)
                        {
                            this.log.DebugFormat("Checking {0} against {1} = MATCH!", context.Request.FilePath, str2);
                        }
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2)
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
    }
}

