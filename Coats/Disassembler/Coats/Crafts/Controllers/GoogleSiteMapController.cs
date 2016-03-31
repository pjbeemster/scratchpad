namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    public class GoogleSiteMapController : Controller
    {
        private string AdjustUrlToContext(string url)
        {
            string applicationPath = base.HttpContext.Request.ApplicationPath;
            if (string.IsNullOrEmpty(applicationPath) || applicationPath.Equals("/"))
            {
                return url;
            }
            return (url + applicationPath);
        }

        public ActionResult Index()
        {
            HttpServerUtilityBase server = base.HttpContext.Server;
            HttpResponseBase response = base.HttpContext.Response;
            HttpRequestBase request = base.HttpContext.Request;
            IContainerAccessor applicationInstance = base.HttpContext.ApplicationInstance as IContainerAccessor;
            this.Logger.Debug("GoogleSitemap handler >>>>>>>>>>>>>>>>>>");
            string url = string.Format("http://{0}", request.Url.Host.ToLower()) + ((request.Url.Port != 80) ? (":" + request.Url.Port) : "");
            url = this.AdjustUrlToContext(url);
            XsltArgumentList arguments = new XsltArgumentList();
            arguments.AddParam("domain", "", url);
            try
            {
                string siteMapFile = WebConfiguration.Current.SiteMapFile;
                this.Logger.DebugFormat("GoogleSitemap config sitemap file {0}", new object[] { siteMapFile });
                string uri = server.MapPath(siteMapFile);
                this.Logger.DebugFormat("GoogleSitemap mapped sitemap file {0}", new object[] { uri });
                XPathDocument document = new XPathDocument(uri);
                XslCompiledTransform transform = new XslCompiledTransform();
                EmbeddedResourceResolver stylesheetResolver = new EmbeddedResourceResolver();
                transform.Load("Coats.Crafts.google-sitemap.xsl", XsltSettings.TrustedXslt, stylesheetResolver);
                StringWriter w = new StringWriter();
                XmlTextWriter writer2 = new XmlTextWriter(w);
                transform.Transform((IXPathNavigable) document, arguments, (XmlWriter) writer2);
                return this.Content(w.ToString(), "text/xml", Encoding.UTF8);
            }
            catch (Exception exception)
            {
                this.Logger.Error("Error rendering Google Sitemap XSLT", exception);
            }
            return base.Content("Error creating Google Sitemap");
        }

        public ILogger Logger { get; set; }
    }
}

