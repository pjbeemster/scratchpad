namespace Coats.Crafts.Filters
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using GeoIP;
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    public class CheckCountryFilter : ActionFilterAttribute
    {
        private Location GetLocation(string client)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CheckCountryFilter >> GetLocation {0}", new object[] { client });
            }
            Location location = HttpContext.Current.Cache[client] as Location;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CheckCountryFilter >> location in cache? {0}", new object[] { location != null });
            }
            if (location == null)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("CheckCountryFilter >> checking location ... ", new object[0]);
                }
                try
                {
                    location = this.LookupService.getLocation(client);
                }
                catch (Exception exception)
                {
                    this.Logger.Error("CheckCountryFilter >> LookupService getLocation: " + exception);
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.DebugFormat("CheckCountryFilter >> LookupService getLocation: " + exception, new object[0]);
                    }
                }
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("CheckCountryFilter >> after get location ... ", new object[0]);
                }
                if (location != null)
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.DebugFormat("CheckCountryFilter >> location not null ", new object[0]);
                    }
                    HttpContext.Current.Cache[client] = location;
                }
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CheckCountryFilter >> location null? {0}", new object[] { location == null });
            }
            return location;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items["CurrentRequestCountry"] = string.Empty;
            filterContext.HttpContext.Items["CurrentRequestLongitude"] = 0.0M;
            filterContext.HttpContext.Items["CurrentRequestLatitude"] = 0.0M;
            filterContext.HttpContext.Items["GeoInfoAvailable"] = false;
            if (!Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
            {
                string clientIP = filterContext.HttpContext.Request.GetClientIP();
                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["ip"]))
                {
                    clientIP = filterContext.HttpContext.Request.QueryString["ip"];
                }
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("CheckCountryFilter >> client {0}", new object[] { clientIP });
                }
                if (!string.IsNullOrEmpty(clientIP))
                {
                    Location location = this.GetLocation(clientIP);
                    if (location != null)
                    {
                        string countryCode = location.countryCode;
                        if (this.Logger.IsDebugEnabled)
                        {
                            this.Logger.DebugFormat("CheckCountryFilter >> countrycode {0}", new object[] { countryCode });
                        }
                        try
                        {
                            RegionInfo info = new RegionInfo(this.Settings.Culture);
                            string twoLetterISORegionName = info.TwoLetterISORegionName;
                            if (this.Logger.IsDebugEnabled)
                            {
                                this.Logger.DebugFormat("CheckCountryFilter >> region ({0}) == countrycode ({1}) = {2}", new object[] { twoLetterISORegionName, countryCode, twoLetterISORegionName == countryCode });
                            }
                            if (twoLetterISORegionName != countryCode)
                            {
                                ((dynamic) filterContext.Controller.ViewBag).CountryAlert = true;
                            }
                        }
                        catch (ArgumentException exception)
                        {
                            this.Logger.ErrorFormat(exception, "CheckCountryFilter >> Could not create Region from current LCID {0}. Check globaization setting.", new object[] { Thread.CurrentThread.CurrentCulture.LCID });
                        }
                        filterContext.HttpContext.Items["CurrentRequestCountry"] = countryCode;
                        filterContext.HttpContext.Items["CurrentRequestLongitude"] = (decimal) location.longitude;
                        filterContext.HttpContext.Items["CurrentRequestLatitude"] = (decimal) location.latitude;
                        filterContext.HttpContext.Items["GeoInfoAvailable"] = true;
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

        public ILogger Logger { get; set; }

        public ILookupService LookupService { get; set; }

        public IAppSettings Settings { get; set; }
    }
}

