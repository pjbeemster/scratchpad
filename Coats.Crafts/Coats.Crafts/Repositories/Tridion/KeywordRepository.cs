using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Models;
using Coats.IndustrialPortal.Gateway;
using Castle.Core.Logging;
using log4net;

namespace Coats.Crafts.Repositories.Tridion
{
    public class KeywordRepository : IKeywordRepository
    {
        public ILogger Logger { get; set; }

        public IList<Keyword> GetKeywordsList(string identifier)
        {
           
            IList<Keyword> KeywordList = GetFromCache(identifier) as List<Keyword>;

            if (KeywordList == null)
            {
                try
                {
                    var kw = UtilityGateway.Instance.GetKeyword(identifier);

                    KeywordList = new List<Keyword>();

                    if (kw != null)
                    {
                        foreach (var tridionKeyword in kw.Children)
                        {
                            var keyword = new Keyword
                                              {
                                                  Name = tridionKeyword.Name,
                                                  Description = tridionKeyword.Description,
                                                  Id = tridionKeyword.KeywordUri,
                                                  Uri = tridionKeyword.TaxonomyUri
                                              };
                            KeywordList.Add(keyword);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return KeywordList; 
                }
            }

            return KeywordList;

        }


        private object GetFromCache(string cacheKey)
        {
            Logger.DebugFormat("GetFromCache {0}", cacheKey);
            return HttpRuntime.Cache[cacheKey];
        }

    }
}
