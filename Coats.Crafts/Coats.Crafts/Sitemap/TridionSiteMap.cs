using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

using DD4T.ContentModel.Factories;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.Utils;
using DD4T.ContentModel.Logging;

namespace Coats.Crafts.Sitemap
{
    /// <summary>
    /// This is the injected class responsible for retrieiving the sitemap from Broker.
    /// </summary>
    public class TridionSiteMap : ISiteMap
    {
        public const string CacheRegion = "System";
        public const string CacheNullValue = "NULL";
        public const string DefaultCacheKey = "SiteMapRootNode";

        private IPageFactory _pageFactory;
        private ICacheAgent _cache;

        public int PublicationId
        {
            get
            {
                return _pageFactory.PageProvider.PublicationId;
            }
        }



        public virtual string CacheKey
        {
            get
            {
                return DefaultCacheKey;
            }
        }


        public TridionSiteMap(IPageFactory pageFactory, ICacheAgent cache)
        {
            _pageFactory = pageFactory;
            _cache = cache;
        }

        public string GetSiteMapXml(string path)
        {
            string sitemap;
            if (!_pageFactory.TryFindPageContent(path, out sitemap))
            {
                sitemap = emptySiteMapString();
            }

            return sitemap;
        }

        public void StoreSiteMapinCache(SiteMapNode sitemap)
        {
            if (sitemap == null)
            {
                // cache special 'null value' (so we do not try to load the sitemap from an invalid or non-existing XML every time!)
                _cache.Store(CacheKey, CacheRegion, CacheNullValue);
            }
            else
            {
                // Store the root node in the cache.
                _cache.Store(CacheKey, CacheRegion, sitemap);
            }
        }

        public SiteMapNode GetSiteMapFromCache()
        {
            object result = _cache.Load(CacheKey);
            if (result != null)
            {
                if (result is string && ((string)result).Equals(CacheNullValue))
                    return null;
                return result as SiteMapNode;
            }

            return null;
        }

        private string emptySiteMapString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<siteMap publicationid=\"tcm:0-70-1\">");
            sb.Append("<siteMapNode title=\"website\" url=\"/\">");
            sb.Append("</siteMapNode>");
            sb.Append("</siteMap>");

            return sb.ToString();
        }
    }
}