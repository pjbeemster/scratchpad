namespace Coats.Crafts.HtmlHelpers
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ProductCareHelper
    {
        public static IList<IKeyword> ProductCareList(IList<IKeyword> ProductCareKeywordList)
        {
            if (ProductCareKeywordList != null)
            {
                ProductCareKeywordList = (from x in ProductCareKeywordList
                    orderby x.Title
                    select x).ToList<IKeyword>();
            }
            return ProductCareKeywordList;
        }
    }
}

