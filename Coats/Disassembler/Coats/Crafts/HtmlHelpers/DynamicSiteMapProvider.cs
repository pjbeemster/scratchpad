namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Xml;

    public class DynamicSiteMapProvider : StaticSiteMapProvider
    {
        private SiteMapNode _rootNode = null;
        private string _siteMapFileName;
        private const string SiteMapNodeName = "siteMapNode";

        private static XmlElement AddDynamicChildElement(XmlElement parentElement, string url, string title, string description)
        {
            XmlElement newChild = parentElement.OwnerDocument.CreateElement("siteMapNode");
            newChild.SetAttribute("url", url);
            newChild.SetAttribute("title", title);
            newChild.SetAttribute("description", description);
            parentElement.AppendChild(newChild);
            return newChild;
        }

        private void AddDynamicNodes(XmlElement rootElement)
        {
        }

        public override SiteMapNode BuildSiteMap()
        {
            lock (this)
            {
                if (null == this._rootNode)
                {
                    this.Clear();
                    XmlElement rootElement = (XmlElement) this.LoadSiteMapXml().GetElementsByTagName("siteMapNode")[0];
                    this.GenerateSiteMapNodes(rootElement);
                }
            }
            return this._rootNode;
        }

        protected override void Clear()
        {
            lock (this)
            {
                this._rootNode = null;
                base.Clear();
            }
        }

        private void CreateChildNodes(XmlElement parentElement, SiteMapNode parentNode)
        {
            foreach (XmlNode node in parentElement.ChildNodes)
            {
                if (node.Name == "siteMapNode")
                {
                    SiteMapNode siteMapNodeFromElement = this.GetSiteMapNodeFromElement((XmlElement) node);
                    this.AddNode(siteMapNodeFromElement, parentNode);
                    this.CreateChildNodes((XmlElement) node, siteMapNodeFromElement);
                }
            }
        }

        private void GenerateSiteMapNodes(XmlElement rootElement)
        {
            this._rootNode = this.GetSiteMapNodeFromElement(rootElement);
            this.AddNode(this._rootNode);
            this.CreateChildNodes(rootElement, this._rootNode);
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return this.RootNode;
        }

        private SiteMapNode GetSiteMapNodeFromElement(XmlElement rootElement)
        {
            string attribute = rootElement.GetAttribute("url");
            string title = rootElement.GetAttribute("title");
            return new SiteMapNode(this, (attribute + title).GetHashCode().ToString(), attribute, title, rootElement.GetAttribute("description"));
        }

        public override void Initialize(string name, NameValueCollection attributes)
        {
            base.Initialize(name, attributes);
            this._siteMapFileName = attributes["siteMapFile"];
        }

        private XmlDocument LoadSiteMapXml()
        {
            XmlDocument document = new XmlDocument();
            document.Load(AppDomain.CurrentDomain.BaseDirectory + this._siteMapFileName);
            return document;
        }

        public override SiteMapNode RootNode
        {
            get
            {
                return this.BuildSiteMap();
            }
        }
    }
}

