using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;

namespace Coats.Crafts.CustomOutputCache
{
    /// <summary>
    /// Needed because MVC3 does not support cahce profiles on child actions.
    /// See http://prashantbrall.wordpress.com/2013/09/29/using-output-cache-profile-in-asp-net-mvc/.
    /// </summary>
    public class FredHopperOutputCache : OutputCacheAttribute
    {
        private bool? _enabled;
        public bool Enabled 
        {
            get { return _enabled.HasValue ? _enabled.Value : true; } // Default to true
            set { _enabled = value; }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Enabled)
            {
                // Only do something is caching is enabled
                base.OnActionExecuted(filterContext);
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (Enabled)
            {
                // Only do something is caching is enabled
                base.OnResultExecuted(filterContext);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(CacheProfile))
            {
                lock (this.GetType())
                {
                    try
                    {
                        // OutputCacheAttribute CacheProfile only supports
                        // Duration, VaryByCustom, and VaryByParam values.
                        OutputCacheSettingsSection settings = (OutputCacheSettingsSection)WebConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
                        OutputCacheProfile profile = settings.OutputCacheProfiles[CacheProfile];
                        Enabled = profile.Enabled;
                        if (Enabled)
                        {
                            Duration = profile.Duration > 0 ? profile.Duration : Duration;
                            VaryByCustom = string.IsNullOrWhiteSpace(profile.VaryByCustom) ? VaryByCustom : profile.VaryByCustom;
                            VaryByParam = string.IsNullOrWhiteSpace(profile.VaryByParam) ? VaryByParam : "*";

                            // See comment above^^^
                            //Location = profile.Location;
                            //NoStore = profile.NoStore;
                            ////this.Order
                            //VaryByContentEncoding = string.IsNullOrWhiteSpace(profile.VaryByContentEncoding) ? VaryByContentEncoding : profile.VaryByContentEncoding;
                            //VaryByHeader = string.IsNullOrWhiteSpace(profile.VaryByHeader) ? VaryByHeader : profile.VaryByHeader;
                        }
                    }
                    catch (Exception)
                    {
                        // Something went wrong, so just disable the whole thing.
                        Enabled = false;
                    }
                    // Finished with the CacheProfile string now, so reset it to null so the base class doesn't try to interrogate it.
                    CacheProfile = null;
                }
            }
            if (Enabled)
            {
                // Only do something is caching is enabled
                base.OnActionExecuting(filterContext);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Enabled)
            {
                // Only do something is caching is enabled
                base.OnResultExecuting(filterContext);
            }
        }

    }
}