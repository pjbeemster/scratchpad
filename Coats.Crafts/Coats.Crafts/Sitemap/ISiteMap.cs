using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Coats.Crafts.Sitemap
{
    public interface ISiteMap
    {
        string GetSiteMapXml(string path);
        SiteMapNode GetSiteMapFromCache();
        void StoreSiteMapinCache(SiteMapNode sitemap);
        int PublicationId { get; }
    }
}