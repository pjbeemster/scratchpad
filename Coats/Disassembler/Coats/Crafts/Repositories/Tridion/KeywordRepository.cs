namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.IndustrialPortal.Gateway;
    using Coats.IndustrialPortal.Gateway.CoatsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web;

    public class KeywordRepository : IKeywordRepository
    {
        private object GetFromCache(string cacheKey)
        {
            this.Logger.DebugFormat("GetFromCache {0}", new object[] { cacheKey });
            return HttpRuntime.Cache[cacheKey];
        }

        public IList<Keyword> GetKeywordsList(string identifier)
        {
            IList<Keyword> fromCache = this.GetFromCache(identifier) as List<Keyword>;
            if (fromCache == null)
            {
                try
                {
                    TridionKeyword keyword = UtilityGateway.Instance.GetKeyword(identifier);
                    fromCache = new List<Keyword>();
                    if (keyword != null)
                    {
                        foreach (TridionKeyword keyword2 in keyword.Children)
                        {
                            Keyword item = new Keyword {
                                Name = keyword2.Name,
                                Description = keyword2.Description,
                                Id = keyword2.KeywordUri,
                                Uri = keyword2.TaxonomyUri
                            };
                            fromCache.Add(item);
                        }
                    }
                }
                catch (Exception)
                {
                    return fromCache;
                }
            }
            return fromCache;
        }

        public ILogger Logger { get; set; }
    }
}

