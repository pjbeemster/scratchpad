namespace Coats.Crafts.Sitemap
{
    using DD4T.ContentModel.Contracts.Caching;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Text;
    using System.Web;

    public class TridionSiteMap : ISiteMap
    {
        private ICacheAgent _cache;
        private IPageFactory _pageFactory;
        public const string CacheNullValue = "NULL";
        public const string CacheRegion = "System";
        public const string DefaultCacheKey = "SiteMapRootNode";

        public TridionSiteMap(IPageFactory pageFactory, ICacheAgent cache)
        {
            this._pageFactory = pageFactory;
            this._cache = cache;
        }

        private string emptySiteMapString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<siteMap publicationid=\"tcm:0-70-1\">");
            builder.Append("<siteMapNode title=\"website\" url=\"/\">");
            builder.Append("</siteMapNode>");
            builder.Append("</siteMap>");
            return builder.ToString();
        }

        public SiteMapNode GetSiteMapFromCache()
        {
            object obj2 = this._cache.Load(this.CacheKey);
            if (obj2 != null)
            {
                if ((obj2 is string) && ((string) obj2).Equals("NULL"))
                {
                    return null;
                }
                return (obj2 as SiteMapNode);
            }
            return null;
        }

        public string GetSiteMapXml(string path)
        {
            string str;
            if (!this._pageFactory.TryFindPageContent(path, out str))
            {
                str = this.emptySiteMapString();
            }
            return str;
        }

        public void StoreSiteMapinCache(SiteMapNode sitemap)
        {
            if (sitemap == null)
            {
                this._cache.Store(this.CacheKey, "System", "NULL");
            }
            else
            {
                this._cache.Store(this.CacheKey, "System", sitemap);
            }
        }

        public virtual string CacheKey
        {
            get
            {
                return "SiteMapRootNode";
            }
        }

        public int PublicationId
        {
            get
            {
                return this._pageFactory.PageProvider.PublicationId;
            }
        }
    }
}

