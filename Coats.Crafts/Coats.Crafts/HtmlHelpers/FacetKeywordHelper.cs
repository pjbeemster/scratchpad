using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Xml;
using System.Text;
using System.Web.Mvc.Html;

using Coats.Crafts.Extensions;
using Coats.Crafts.Filters;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Controllers;

using DD4T.ContentModel;

using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location;
using com.fredhopper.lang.query.location.criteria;

namespace Coats.Crafts.HtmlHelpers
{
    public static class FacetKeywordHelper
    {
        /// <summary>
        /// The close link on the brand banner has to work by simply remocing the specific brand facet form the query
        /// If no brand facet is found - ie first load - then just create a link back to original page
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString CloseLink(this HtmlHelper helper)
        {
            var builder = new TagBuilder("a");
            string href = helper.ViewContext.HttpContext.Request.Url.AbsolutePath;

            // Add attributes
            builder.MergeAttribute("class","icon close");
            builder.SetInnerText(helper.GetResource("RemoveFilter"));

            string facet = helper.ViewContext.RouteData.Values.GetLevel1BrandFacet();
            string facet_value = helper.ViewContext.RouteData.Values.GetLevel1BrandFacetValueDelimeted();

            if (!String.IsNullOrEmpty(
                helper.ViewContext.HttpContext.Request["fh_params"]))
            {
                var query = new Query();
                query.ParseQuery(helper.ViewContext.HttpContext.Request["fh_params"]);

                var fh_location = query.getLocation().toString();
                var link = FredHopperExtensions.RemoveFacetValues(
                                    fh_location,
                                    facet,
                                    facet_value);

                Location criterionLoc = new Location(link);
                query.setLocation(criterionLoc);

                href += query.ToFhParams(); //"?fh_params=" + HttpUtility.UrlEncode(query.toString());
            }
            else
            {
                href += "?clear=true";
            }

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            builder.MergeAttribute("href", urlHelper.Content(href.AddApplicationRoot()));

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// Adds the banner.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static string FacetKeyword(this HtmlHelper helper, IComponent comp, string facetKeyword)
        {
            if (comp.MetadataFields.Count > 0)
            {
                if (comp.MetadataFields["facets"].LinkedComponentValues[0].Fields.ContainsKey(facetKeyword))
                {
                    var keywords = comp.MetadataFields["facets"].LinkedComponentValues[0].Fields[facetKeyword].Keywords;
                    string[] facets = new string[keywords.Count];
                    int i = 0;

                    foreach (Keyword key in keywords)
                    {
                        if (!string.IsNullOrEmpty(key.Description))
                        {
                            facets[i] = key.Description;
                            i++;
                        }
                        else
                        {
                            //Should only get here if a description has been missed off!
                            facets[i] = "[" + key.Title + "]";
                            i++;
                        }
                    }
                    return string.Join(", ", facets);
                }
            }
            return "";
        }
    }
}