using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DD4T.ContentModel;

namespace Coats.Crafts.HtmlHelpers
{
    public static class ProductCareHelper
    {

        /// <summary>
        /// Returns the product care list ordered by title 
        /// </summary>
        /// <param name="lst">The ProductCareKeywordList.</param>
        /// <returns></returns>
        public static IList<IKeyword> ProductCareList(IList<IKeyword> ProductCareKeywordList)
        {
            if (ProductCareKeywordList != null)
            {
               ProductCareKeywordList = ProductCareKeywordList.OrderBy(x => x.Title).ToList();
            }

            return ProductCareKeywordList;
        }
    }
}