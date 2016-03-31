namespace Coats.Crafts.CustomOutputCache
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    public class FredHopperOutputCache : OutputCacheAttribute
    {
        private bool? _enabled;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (this.Enabled)
            {
                base.OnActionExecuted(filterContext);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(base.CacheProfile))
            {
                lock (base.GetType())
                {
                    try
                    {
                        OutputCacheSettingsSection section = (OutputCacheSettingsSection) WebConfigurationManager.GetSection("system.web/caching/outputCacheSettings");
                        OutputCacheProfile profile = section.OutputCacheProfiles[base.CacheProfile];
                        this.Enabled = profile.Enabled;
                        if (this.Enabled)
                        {
                            base.Duration = (profile.Duration > 0) ? profile.Duration : base.Duration;
                            base.VaryByCustom = string.IsNullOrWhiteSpace(profile.VaryByCustom) ? base.VaryByCustom : profile.VaryByCustom;
                            base.VaryByParam = string.IsNullOrWhiteSpace(profile.VaryByParam) ? base.VaryByParam : "*";
                        }
                    }
                    catch (Exception)
                    {
                        this.Enabled = false;
                    }
                    base.CacheProfile = null;
                }
            }
            if (this.Enabled)
            {
                base.OnActionExecuting(filterContext);
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (this.Enabled)
            {
                base.OnResultExecuted(filterContext);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (this.Enabled)
            {
                base.OnResultExecuting(filterContext);
            }
        }

        public bool Enabled
        {
            get
            {
                return (this._enabled.HasValue ? this._enabled.Value : true);
            }
            set
            {
                this._enabled = new bool?(value);
            }
        }
    }
}

