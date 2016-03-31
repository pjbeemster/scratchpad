using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;

namespace Coats.Crafts.HtmlHelpers
{
    public static class LabelHelper
    {

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// Updated to avoid errors using Name metaData in Models when using App_GlobalResources
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            string htmlFieldName = (ExpressionHelper.GetExpressionText(expression).Split('.').Last());

            //get LabelText directly from Labels resource file

            string labelText = (String)ResourceHelper.GetResource(html, htmlFieldName);
            //string labelText = (String)ResourceHelper.GetResource(html, "resources.resx", htmlFieldName);
            //string labelText = (String)HttpContext.GetGlobalResourceObject("Labels", htmlFieldName);
            //if it does not exist in resource files then try to get in normal way
            if (string.IsNullOrEmpty(labelText))
            {
                try
                {
                    ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
                    labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
                    if (String.IsNullOrEmpty(labelText))
                    {
                        return MvcHtmlString.Empty;
                    }
                }
                catch (Exception e)
                {
                    return MvcHtmlString.Create("No resource with name " + labelText);
                }
            }

            TagBuilder tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.SetInnerText(labelText);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

		public static IHtmlString DisplayBoldLabel(bool isBold, string label, string value, string seperator)
		{


			if(isBold) {

				return MvcHtmlString.Create(String.Format("<strong>{0}{2}</strong> {1}", label, value, seperator));

			} else {
				 return MvcHtmlString.Create(String.Format("{0}{2} {1}", label, value,seperator));
			}

			 

		}
    }
}