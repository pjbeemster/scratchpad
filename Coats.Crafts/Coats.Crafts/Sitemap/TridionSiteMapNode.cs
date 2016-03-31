using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using DD4T.ContentModel.Factories;
using DD4T.Factories;
using DD4T.ContentModel;

namespace Coats.Crafts.Sitemap
{
    public class TridionSiteMapNode : SiteMapNode
    {
        public static string NoUrlFoundPrefix = "/NoUrlInSitemap#";

        public TridionSiteMapNode(SiteMapProvider provider, string key, string uri, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey):
            base(provider, key, url, title, description, roles, attributes, explicitResourceKeys, implicitResourceKey)
        {
            if (url.StartsWith("tcm:"))
            {
                Url = MakeDummyUrl(url);
            }
            Uri = uri;
        }

        public override bool HasChildNodes
        {
            get
            {
                return ChildNodes.Count > 0;
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

        public new NameValueCollection Attributes
        {
            get
            {
                return base.Attributes;
            }
        }

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

        public string Uri { get; set; }

        public int Level { get; set; }

        private string MakeDummyUrl(string inputUrl)
        {
            return NoUrlFoundPrefix + HttpUtility.HtmlEncode(inputUrl);
        }
    }
}
