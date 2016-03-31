namespace Coats.Crafts.Sitemap
{
    using System;
    using System.Web;

    public interface ISiteMap
    {
        SiteMapNode GetSiteMapFromCache();
        string GetSiteMapXml(string path);
        void StoreSiteMapinCache(SiteMapNode sitemap);

        int PublicationId { get; }
    }
}

