namespace Coats.Crafts.HtmlHelpers
{
    using DD4T.ContentModel;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web.Mvc;

    public static class CategoryListHelper
    {
        public static string CategoryList(this HtmlHelper html, IField CategoryField)
        {
            StringBuilder categoryString = new StringBuilder();
            CategoryField.Keywords.ToList<IKeyword>().ForEach(delegate (IKeyword k) {
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
            return categoryString.ToString().Trim(new char[] { ',', ' ' });
        }
    }
}

