namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Controllers;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web;

    public class NewsLetterRepository : INewsLetterRepository
    {
        private object GetFromCache(string cacheKey)
        {
            this.Logger.DebugFormat("GetFromCache {0}", new object[] { cacheKey });
            return HttpRuntime.Cache[cacheKey];
        }

        public IList<Newsletter> GetNewsLetterList(string identifier)
        {
            IList<Newsletter> fromCache = this.GetFromCache(identifier) as List<Newsletter>;
            if (fromCache == null)
            {
                try
                {
                    IFieldSet fields = this.GetPageInfo(identifier).ComponentPresentations[0].Component.Fields;
                    IList<IKeyword> list2 = fields.ContainsKey("NewsLetterBrands") ? fields["NewsLetterBrands"].Keywords : null;
                    fromCache = new List<Newsletter>();
                    Registration.NewsLetterHeader = fields.ContainsKey("header") ? fields["header"].Value : null;
                    foreach (DD4T.ContentModel.Keyword keyword in list2)
                    {
                        Newsletter item = new Newsletter {
                            id = keyword.Id,
                            Header = keyword.MetadataFields.ContainsKey("Header") ? keyword.MetadataFields["Header"].Value : string.Empty,
                            Description = keyword.MetadataFields.ContainsKey("Description") ? keyword.MetadataFields["Description"].Value : string.Empty,
                            logo = keyword.MetadataFields.ContainsKey("logo") ? keyword.MetadataFields["logo"] : null
                        };
                        fromCache.Add(item);
                    }
                }
                catch (Exception)
                {
                    return fromCache;
                }
            }
            return fromCache;
        }

        private IPage GetPageInfo(string tcm)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IPageFactory>().GetPage(tcm);
        }

        public ILogger Logger { get; set; }
    }
}

