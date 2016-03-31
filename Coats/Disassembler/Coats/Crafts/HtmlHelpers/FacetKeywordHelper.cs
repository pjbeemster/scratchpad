namespace Coats.Crafts.HtmlHelpers
{
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public static class FacetKeywordHelper
    {
        public static MvcHtmlString CloseLink(this HtmlHelper helper)
        {
            TagBuilder builder = new TagBuilder("a");
            string absolutePath = helper.ViewContext.HttpContext.Request.Url.AbsolutePath;
            builder.MergeAttribute("class", "icon close");
            builder.SetInnerText(helper.GetResource("RemoveFilter"));
            string facet = helper.ViewContext.RouteData.Values.GetLevel1BrandFacet();
            string facetValue = helper.ViewContext.RouteData.Values.GetLevel1BrandFacetValueDelimeted();
            if (!string.IsNullOrEmpty(helper.ViewContext.HttpContext.Request["fh_params"]))
            {
                Query query = new Query();
                query.ParseQuery(helper.ViewContext.HttpContext.Request["fh_params"]);
                Location location = new Location(FredHopperExtensions.RemoveFacetValues(query.getLocation().toString(), facet, facetValue));
                query.setLocation(location);
                absolutePath = absolutePath + query.ToFhParams();
            }
            else
            {
                absolutePath = absolutePath + "?clear=true";
            }
            UrlHelper helper2 = new UrlHelper(helper.ViewContext.RequestContext);
            builder.MergeAttribute("href", helper2.Content(absolutePath.AddApplicationRoot()));
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string FacetKeyword(this HtmlHelper helper, IComponent comp, string facetKeyword)
        {
            if ((comp.MetadataFields.Count > 0) && comp.MetadataFields["facets"].LinkedComponentValues[0].Fields.ContainsKey(facetKeyword))
            {
                IList<IKeyword> keywords = comp.MetadataFields["facets"].LinkedComponentValues[0].Fields[facetKeyword].Keywords;
                string[] strArray = new string[keywords.Count];
                int index = 0;
                foreach (Keyword keyword in keywords)
                {
                    if (!string.IsNullOrEmpty(keyword.Description))
                    {
                        strArray[index] = keyword.Description;
                        index++;
                    }
                    else
                    {
                        strArray[index] = "[" + keyword.Title + "]";
                        index++;
                    }
                }
                return string.Join(", ", strArray);
            }
            return "";
        }
    }
}

