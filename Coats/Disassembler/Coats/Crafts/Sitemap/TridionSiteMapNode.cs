namespace Coats.Crafts.Sitemap
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Runtime.CompilerServices;
    using System.Web;

    public class TridionSiteMapNode : SiteMapNode
    {
        public static string NoUrlFoundPrefix = "/NoUrlInSitemap#";

        public TridionSiteMapNode(SiteMapProvider provider, string key, string uri, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey) : base(provider, key, url, title, description, roles, attributes, explicitResourceKeys, implicitResourceKey)
        {
            if (url.StartsWith("tcm:"))
            {
                this.Url = this.MakeDummyUrl(url);
            }
            this.Uri = uri;
        }

        private string MakeDummyUrl(string inputUrl)
        {
            return (NoUrlFoundPrefix + HttpUtility.HtmlEncode(inputUrl));
        }

        public NameValueCollection Attributes
        {
            get
            {
                return base.Attributes;
            }
        }

        public override SiteMapNodeCollection ChildNodes
        {
            get
            {
                return base.ChildNodes;
            }
            set
            {
                base.ChildNodes = value;
            }
        }

        public override bool HasChildNodes
        {
            get
            {
                return (this.ChildNodes.Count > 0);
            }
        }

        public int Level { get; set; }

        public string Uri { get; set; }

        public override string Url
        {
            get
            {
                if (base.Url.StartsWith(NoUrlFoundPrefix))
                {
                    return string.Empty;
                }
                return base.Url;
            }
            set
            {
                base.Url = value;
            }
        }
    }
}

