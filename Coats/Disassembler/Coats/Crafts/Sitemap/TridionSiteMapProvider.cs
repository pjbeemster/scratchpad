namespace Coats.Crafts.Sitemap
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using DD4T.ContentModel.Logging;
    using DD4T.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Xml.Linq;

    public class TridionSiteMapProvider : StaticSiteMapProvider
    {
        private ISiteMap _sitemap;
        private int debugCounter = 0;
        public const string DefaultSiteMapPath = "/system/sitemap/sitemap.xml";
        private object lock1 = new object();

        private void AddChildren(SiteMapNode rootNode, IEnumerable<XElement> siteMapNodes, int currentLevel)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            foreach (XElement element in siteMapNodes)
            {
                SiteMapNode node = this.CreateNodeFromElement(element, currentLevel);
                logger.DebugFormat("TridionSiteMapProvider AddChildren - childNode.Url = {0}, ", new object[] { node.Url });
                this.AddNode(node, rootNode);
                this.AddChildren(node, element.Elements(), currentLevel + 1);
            }
        }

        public override SiteMapNode BuildSiteMap()
        {
            this.debugCounter++;
            SiteMapNode siteMapFromCache = this._sitemap.GetSiteMapFromCache();
            if (siteMapFromCache == null)
            {
                lock (this.lock1)
                {
                    base.Clear();
                    siteMapFromCache = this.ReadSitemapFromXml(this.SiteMapPath);
                    this._sitemap.StoreSiteMapinCache(siteMapFromCache);
                }
            }
            return siteMapFromCache;
        }

        protected override void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }

        protected virtual SiteMapNode CreateNodeFromElement(XElement element, int currentLevel)
        {
            string str;
            NameValueCollection attributes = new NameValueCollection();
            foreach (XAttribute attribute in element.Attributes())
            {
                attributes.Add(attribute.Name.ToString(), attribute.Value);
            }
            try
            {
                if (element.Attribute("uri") != null)
                {
                    str = element.Attribute("uri").Value;
                }
                else if (element.Attribute("pageId") != null)
                {
                    str = element.Attribute("pageId").Value;
                }
                else if (element.Attribute("id") != null)
                {
                    str = element.Attribute("id").Value;
                }
                else
                {
                    str = "";
                }
            }
            catch
            {
                LoggerService.Debug("exception while retrieving uri", LoggingCategory.General, new object[0]);
                str = "";
            }
            return new TridionSiteMapNode(this, element.Attribute("id").Value, str, element.Attribute("url").Value, element.Attribute("title").Value, "", null, attributes, null, null) { Level = currentLevel };
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return this.BuildSiteMap();
        }

        public override void Initialize(string name, NameValueCollection attributes)
        {
            if (!this.IsInitialized)
            {
                IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                this._sitemap = applicationInstance.Container.Resolve<ISiteMap>();
                base.Initialize(name, attributes);
                this.IsInitialized = true;
            }
        }

        private SiteMapNode ReadSitemapFromXml(string sitemapUrl)
        {
            LoggerService.Debug(">>ReadSitemapFromXml", LoggingCategory.Performance, new object[0]);
            SiteMapNode node = null;
            string siteMapXml = this._sitemap.GetSiteMapXml(sitemapUrl);
            LoggerService.Debug(string.Format("loaded sitemap with url {0}, length {1}", sitemapUrl, siteMapXml.Length), LoggingCategory.Performance, new object[0]);
            XDocument document = XDocument.Parse(siteMapXml);
            LoggerService.Debug("parsed sitemap into XDocument", LoggingCategory.Performance, new object[0]);
            XElement root = document.Root;
            try
            {
                node = new TridionSiteMapNode(this, string.Empty, "root_" + this._sitemap.PublicationId, string.Empty, string.Empty, string.Empty, new ArrayList(), new NameValueCollection(), new NameValueCollection(), string.Empty);
                LoggerService.Debug("created root node", LoggingCategory.Performance, new object[0]);
                this.AddNode(node);
                LoggerService.Debug("added root node", LoggingCategory.Performance, new object[0]);
                this.AddChildren(node, root.Elements(), 1);
            }
            catch (Exception)
            {
            }
            LoggerService.Debug("<<ReadSitemapFromXml", LoggingCategory.Performance, new object[0]);
            return node;
        }

        public virtual bool IsInitialized { get; private set; }

        public virtual string SiteMapPath
        {
            get
            {
                string siteMapPath = ConfigurationHelper.SiteMapPath;
                if (string.IsNullOrEmpty(siteMapPath))
                {
                    return "/system/sitemap/sitemap.xml";
                }
                return siteMapPath;
            }
        }
    }
}

