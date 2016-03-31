namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Resources;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    public class ResourcesController : Controller
    {
        private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private IAppSettings _settings;

        public ResourcesController(IAppSettings settings)
        {
            this._settings = settings;
        }

        public ActionResult GetResourcesJavaScript(string resxFileName)
        {
            this.Logger.InfoFormat("Request for {0}", new object[] { resxFileName });
            if (resxFileName.EndsWith(".js"))
            {
                resxFileName = resxFileName.Replace(".js", ".resx");
            }
            DynamicResourceProvider instance = (DynamicResourceProvider) this.Factory.Create(resxFileName);
            StringReader reader = new StringReader(instance.ResourceDocument.OuterXml);
            IContainerAccessor applicationInstance = base.HttpContext.ApplicationInstance as IContainerAccessor;
            this.Logger.InfoFormat("GetResourcesJavaScript > Tracking factory? {0}", new object[] { applicationInstance.Container.Kernel.ReleasePolicy.HasTrack(this.Factory) });
            this.Logger.InfoFormat("GetResourcesJavaScript > Tracking provider? {0}", new object[] { applicationInstance.Container.Kernel.ReleasePolicy.HasTrack(instance) });
            Dictionary<string, string> dictionary = new ResXResourceReader(reader).Cast<DictionaryEntry>().ToDictionary<DictionaryEntry, string, string>(entry => entry.Key.ToString(), entry => entry.Value.ToString());
            string str = _serializer.Serialize(dictionary);
            string script = string.Format("window.Resources = window.Resources || {{}}; window.Resources = {0};", str);
            return this.JavaScript(script);
        }

        public IDynamcResourceProviderFactory Factory { get; set; }

        public ILogger Logger { get; set; }
    }
}

