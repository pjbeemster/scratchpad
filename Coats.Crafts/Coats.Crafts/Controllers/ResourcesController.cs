
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Compilation;
using System.Xml;
using System.Xml.XPath;

using Coats.Crafts.Resources;
using Coats.Crafts.Configuration;

using Castle.Windsor;
using Castle.Core.Logging;

namespace Coats.Crafts.Controllers
{
    public class ResourcesController : Controller
    {
        public ILogger Logger { get; set; }
        public IDynamcResourceProviderFactory Factory { get; set; }

        private IAppSettings _settings;
        private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public ResourcesController(IAppSettings settings)
        {
            _settings = settings;
        }

        public ActionResult GetResourcesJavaScript(string resxFileName)
        {
            Logger.InfoFormat("Request for {0}", resxFileName);

            if (resxFileName.EndsWith(".js"))
                resxFileName = resxFileName.Replace(".js", ".resx");

            //IContainerAccessor accessor = HttpContext.ApplicationInstance as IContainerAccessor;
            //IDynamcResourceProviderFactory factory = accessor.Container.Resolve<IDynamcResourceProviderFactory>();
            DynamicResourceProvider provider = (DynamicResourceProvider)Factory.Create(resxFileName);

            //XmlDocument resourceXml = provider.ResourceDocument;
            XPathNavigator resourceXml = provider.ResourceDocument;
            StringReader xml = new StringReader(resourceXml.OuterXml);

            IContainerAccessor accessor = HttpContext.ApplicationInstance as IContainerAccessor;
            Logger.InfoFormat("GetResourcesJavaScript > Tracking factory? {0}", accessor.Container.Kernel.ReleasePolicy.HasTrack(Factory));
            Logger.InfoFormat("GetResourcesJavaScript > Tracking provider? {0}", accessor.Container.Kernel.ReleasePolicy.HasTrack(provider));


            var resourceDictionary = new ResXResourceReader(xml)
                                .Cast<DictionaryEntry>()
                                .ToDictionary(entry => entry.Key.ToString(), entry => entry.Value.ToString());
            var json = _serializer.Serialize(resourceDictionary);
            var javaScript = string.Format("window.Resources = window.Resources || {{}}; window.Resources = {0};", json);

            return JavaScript(javaScript);
        }
    }
}



