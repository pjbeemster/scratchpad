using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Text;

using DD4T.ContentModel;

namespace Coats.Crafts.HtmlHelpers
{
    public static class CategoryListHelper
    {
        public static String CategoryList(this HtmlHelper html, IField CategoryField)
        {
            StringBuilder categoryString = new StringBuilder();
            CategoryField.Keywords.ToList().ForEach(k =>
            {
                if (!string.IsNullOrEmpty(k.Description))
                {
                    categoryString.Append(k.Description);
                }
                else
                {
                    categoryString.Append(k.Title);
                }
                categoryString.Append(", ");
            });          

            return categoryString.ToString().Trim(',',' ');
        }
    }
}