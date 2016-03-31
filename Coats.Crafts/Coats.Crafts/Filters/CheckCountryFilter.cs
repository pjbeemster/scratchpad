using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;

using Castle.Core.Logging;

using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;

using GeoIP;

namespace Coats.Crafts.Filters
{
    public class CheckCountryFilter : ActionFilterAttribute
    {
        public IAppSettings Settings { get; set; }
        public ILookupService LookupService { get; set; }
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items["CurrentRequestCountry"] = String.Empty;
            filterContext.HttpContext.Items["CurrentRequestLongitude"] = 0.0M;
            filterContext.HttpContext.Items["CurrentRequestLatitude"] = 0.0M;
            filterContext.HttpContext.Items["GeoInfoAvailable"] = false;

            // We need a specific culture to get the region
            if (!Thread.CurrentThread.CurrentCulture.IsNeutralCulture)
            {

                // ASH - 12/06/2013:
                // Removing the following '#if DEBUG...' in favour of checking for an IP query string item.
//#if DEBUG
//                //string client = "24.24.24.24";
//                //Glasgow
//                //string client = "82.12.114.166";
//                //London
//                string client = "86.164.227.84";


//#else
//            string client = filterContext.HttpContext.Request.GetClientIP();
//#endif

                string client = filterContext.HttpContext.Request.GetClientIP();

                // IP address override for testing purposes.
                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["ip"]))
                {
                    client = filterContext.HttpContext.Request.QueryString["ip"];
                }

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("CheckCountryFilter >> client {0}", client);

                if (!String.IsNullOrEmpty(client))
                {
                    Location location = this.GetLocation(client);
                   // Don't think this is ever true as the Lookup service returns "N/A"
                    if (location != null)
                    {
                        string countrycode = location.countryCode;

                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("CheckCountryFilter >> countrycode {0}", countrycode);

                        RegionInfo info;
                        try
                        {
                            // Had to get culture from web config, not from globalization as 
                            // chnaing that setting broke DD4T!
                            info = new RegionInfo(Settings.Culture);
                            string region = info.TwoLetterISORegionName;

                            if (Logger.IsDebugEnabled)
                                Logger.DebugFormat("CheckCountryFilter >> region ({0}) == countrycode ({1}) = {2}", region, countrycode, (region== countrycode));

                            if (region != countrycode)
                                filterContext.Controller.ViewBag.CountryAlert = true;
                        }
                        catch (ArgumentException ae)
                        {
                            Logger.ErrorFormat(ae, "CheckCountryFilter >> Could not create Region from current LCID {0}. Check globaization setting.", Thread.CurrentThread.CurrentCulture.LCID);
                        }

                        // Add to context for anyone else to use
                        filterContext.HttpContext.Items["CurrentRequestCountry"] = countrycode;
                        filterContext.HttpContext.Items["CurrentRequestLongitude"] = (decimal) location.longitude;
                        filterContext.HttpContext.Items["CurrentRequestLatitude"] = (decimal) location.latitude;
                        filterContext.HttpContext.Items["GeoInfoAvailable"] = true;
                    }
                }

            }

            base.OnActionExecuting(filterContext);
        }

        private Location GetLocation(string client)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CheckCountryFilter >> GetLocation {0}", client);

            // Check cache first
            Location location = HttpContext.Current.Cache[client] as Location;
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CheckCountryFilter >> location in cache? {0}", (location != null));

            if (location == null)
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("CheckCountryFilter >> checking location ... ");

                //LookupService lookup = new LookupService(dataFile, LookupService.GEOIP_MEMORY_CACHE);

                try
                {
                    location = LookupService.getLocation(client); //lookup.getLocation(client);
                }
                catch (Exception ex)
                {

                    Logger.Error("CheckCountryFilter >> LookupService getLocation: " + ex);

                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("CheckCountryFilter >> LookupService getLocation: " + ex);
                }


                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("CheckCountryFilter >> after get location ... ");

                // Add to cache if not null (which it could be based on tests with 127.0.0.1
                if (location != null)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("CheckCountryFilter >> location not null ");
                    }   
                    HttpContext.Current.Cache[client] = location;
                }

            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CheckCountryFilter >> location null? {0}", (location == null));

            return location;
        }
    }
}