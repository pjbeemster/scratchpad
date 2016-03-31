using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

using DD4T.Utils;
using DD4T.ContentModel.Logging;

using Castle.Windsor;
using Castle.MicroKernel;
using Castle.Core.Logging;

namespace Coats.Crafts.Sitemap
{       
    /// <summary>
    /// This is the provider as far as the .Net Dramework is concerned. 
    /// The actual class that gets the XML is injected in via DI.
    /// </summary>
    public class TridionSiteMapProvider : StaticSiteMapProvider
    {
        private int debugCounter = 0;

        public const string DefaultSiteMapPath = "/system/sitemap/sitemap.xml";

        private ISiteMap _sitemap;

        public virtual string SiteMapPath
        {
            get
            {
                string path = ConfigurationHelper.SiteMapPath;
                if (string.IsNullOrEmpty(path))
                {
                    return DefaultSiteMapPath;
                }
                return path;
            }
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return BuildSiteMap();
        }

        private SiteMapNode ReadSitemapFromXml(string sitemapUrl)
        {
            LoggerService.Debug(">>ReadSitemapFromXml", LoggingCategory.Performance);
            SiteMapNode rootNode = null;

            string sitemap = _sitemap.GetSiteMapXml(sitemapUrl);
            LoggerService.Debug(string.Format("loaded sitemap with url {0}, length {1}", sitemapUrl, sitemap.Length), LoggingCategory.Performance);

            XDocument xDoc = XDocument.Parse(sitemap);

            LoggerService.Debug("parsed sitemap into XDocument", LoggingCategory.Performance);

            //XElement siteMapRoot = xDoc.Element("siteMap");
            XElement siteMapRoot = xDoc.Root;

            try
            {
                rootNode = new TridionSiteMapNode(this, String.Empty, "root_" + _sitemap.PublicationId, String.Empty, String.Empty, String.Empty, new ArrayList(), new NameValueCollection(), new NameValueCollection(), String.Empty);                
                LoggerService.Debug("created root node", LoggingCategory.Performance);
                AddNode(rootNode);
                LoggerService.Debug("added root node", LoggingCategory.Performance);

                //Fill down the hierarchy.
                AddChildren(rootNode, siteMapRoot.Elements(), 1);
            }
            catch (Exception e)
            {
                Exception e2 = e;
            }
            LoggerService.Debug("<<ReadSitemapFromXml", LoggingCategory.Performance);
            return rootNode;
        }

        private void AddChildren(SiteMapNode rootNode, IEnumerable<XElement> siteMapNodes, int currentLevel)
        {
            //LoggerService.Debug(">>AddChildren for root node {0} at level {1}", LoggingCategory.Performance, rootNode.Title, currentLevel);

            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var logger = accessor.Container.Resolve<ILogger>();

            foreach (var element in siteMapNodes)
            {
              //  LoggerService.Debug(">>>for loop iteration: {0}", LoggingCategory.Performance, element.ToString());
                SiteMapNode childNode = CreateNodeFromElement(element, currentLevel);

               // LoggerService.Debug("finished creating TridionSiteMapNode", LoggingCategory.Performance);

                //LoggerService.Debug("about to add TridionSiteMapNode to node dictionary", LoggingCategory.Performance);
                //LoggerService.Debug("finished adding TridionSiteMapNode to node dictionary", LoggingCategory.Performance);

                //Use the SiteMapNode AddNode method to add the SiteMapNode to the ChildNodes collection
                //LoggerService.Debug("about to add node to SiteMap", LoggingCategory.Performance);

                logger.DebugFormat("TridionSiteMapProvider AddChildren - childNode.Url = {0}, ", childNode.Url);

                AddNode(childNode, rootNode);

                //LoggerService.Debug(string.Format("finished adding node to sitemap (title={0}, parent title={1})", childNode.Title, rootNode.Title), LoggingCategory.Performance);

                // Check for children in this node.
                AddChildren(childNode, element.Elements(), currentLevel + 1);
                //LoggerService.Debug("<<<for loop iteration: {0}", LoggingCategory.Performance, element.ToString());
            }

            //LoggerService.Debug("<<AddChildren for root node {0} at level {1}", LoggingCategory.Performance, rootNode.Title, currentLevel);
        }

        protected virtual SiteMapNode CreateNodeFromElement(XElement element, int currentLevel)
        {
            var attributes = new NameValueCollection();
            foreach (var a in element.Attributes())
            {
                attributes.Add(a.Name.ToString(), a.Value);
            }

            string uri;
            try
            {
                if (element.Attribute("uri") != null)
                    uri = element.Attribute("uri").Value;
                else if (element.Attribute("pageId") != null)
                    uri = element.Attribute("pageId").Value;
                else if (element.Attribute("id") != null)
                    uri = element.Attribute("id").Value;
                else
                    uri = "";
            }
            catch
            {
                LoggerService.Debug("exception while retrieving uri", LoggingCategory.General);
                uri = "";
            }
            
            SiteMapNode childNode = new TridionSiteMapNode(this,
                element.Attribute("id").Value, //key
                uri,
                element.Attribute("url").Value, //url
                element.Attribute("title").Value, //title
                "", //description
                null, //roles
                attributes, //attributes
                null, //explicitresourceKeys
                null) { Level = currentLevel }; // implicitresourceKey

            return childNode;
        }

        private object lock1 = new object();
        public override SiteMapNode BuildSiteMap()
        {
            debugCounter++;
            SiteMapNode rootNode = _sitemap.GetSiteMapFromCache();
            if (rootNode == null)
            {
                lock (lock1)
                {
                    base.Clear(); 
                    rootNode = ReadSitemapFromXml(SiteMapPath);
                    _sitemap.StoreSiteMapinCache(rootNode);
                }
            }

            return rootNode;
        }

        public virtual bool IsInitialized
        {
            get;
            private set;
        }

        public override void Initialize(string name, NameValueCollection attributes) 
        {           
            if (!IsInitialized)
            {
                IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                _sitemap = accessor.Container.Resolve<ISiteMap>();

                base.Initialize(name, attributes);
                IsInitialized = true;
            }
        }


        //FindSiteMapNodeByComponentId moved to DD4TComponents - TridionSiteMapProvider not used any more but method is still needed

        //public SiteMapNode FindSiteMapNodeByComponentId(string componentTcm)
        //{
        //    IEnumerable<SiteMapNode> nodes = RootNode
        //                                        .GetAllNodes()
        //                                        .Cast<SiteMapNode>();

        //    SiteMapNode node = null;
        //    foreach (SiteMapNode n in nodes)
        //    {
        //        string components = n["components"];
        //        if (!String.IsNullOrEmpty(components))
        //            if (components.Contains(componentTcm))
        //                node = n;
        //    }

        //    //SiteMapNode node = nodes.FirstOrDefault(n => n["components"].Contains(componentTcm));

        //    return node;
        //}

        protected override void Clear()
        {
            lock (this)
            {
                // CacheAgent.Remove("rootNode"); // currently, CacheAgents do not support 'Remove'
                base.Clear();
            }
        }

    }
}
