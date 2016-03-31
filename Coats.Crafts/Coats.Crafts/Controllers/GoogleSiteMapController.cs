using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Castle.Windsor;
using Castle.Core.Logging;

using Coats.Crafts.Configuration;

namespace Coats.Crafts.Controllers
{
    public class GoogleSiteMapController : Controller
    {
        public ILogger Logger { get; set; }


        private string AdjustUrlToContext(string url)
        {
            string appPath = HttpContext.Request.ApplicationPath;

            if (string.IsNullOrEmpty(appPath) || appPath.Equals("/"))
                return url;

            return url + appPath;
        }

        //
        // GET: /GoogleSiteMap/

        public ActionResult Index()
        {
            var server = HttpContext.Server;
            var response = HttpContext.Response;
            var request = HttpContext.Request;
            var accessor = HttpContext.ApplicationInstance as IContainerAccessor;

            Logger.Debug("GoogleSitemap handler >>>>>>>>>>>>>>>>>>");

            string domain = String.Format("http://{0}", request.Url.Host.ToLower());
            domain += (request.Url.Port != 80) ? ":" + request.Url.Port : "";

            domain = AdjustUrlToContext(domain);

            // xslt arguments
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("domain", "", domain);

            try
            {
                // get paths to the xml/xsl files
                string configSiteMap = WebConfiguration.Current.SiteMapFile;
                Logger.DebugFormat("GoogleSitemap config sitemap file {0}", configSiteMap);

                string sitemappath = server.MapPath(configSiteMap);
                Logger.DebugFormat("GoogleSitemap mapped sitemap file {0}", sitemappath);

                XPathDocument xpathDoc = new XPathDocument(sitemappath);
                XslCompiledTransform transform = new XslCompiledTransform();
                EmbeddedResourceResolver resolver = new EmbeddedResourceResolver();
                transform.Load("Coats.Crafts.google-sitemap.xsl", XsltSettings.TrustedXslt, resolver);

                // Create required writer for output  
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter transformedXml = new XmlTextWriter(stringWriter);

                transform.Transform(xpathDoc, args, transformedXml);

                return Content(stringWriter.ToString(), "text/xml", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.Error("Error rendering Google Sitemap XSLT", ex);
            }

            return Content("Error creating Google Sitemap");
        }

    }

    public class EmbeddedResourceResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(Path.GetFileName(absoluteUri.AbsolutePath));
        }
    }
}
