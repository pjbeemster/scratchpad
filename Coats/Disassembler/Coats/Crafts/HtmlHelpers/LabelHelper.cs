namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class LabelHelper
    {
        public static IHtmlString DisplayBoldLabel(bool isBold, string label, string value, string seperator)
        {
            if (isBold)
            {
                return MvcHtmlString.Create(string.Format("<strong>{0}{2}</strong> {1}", label, value, seperator));
            }
            return MvcHtmlString.Create(string.Format("{0}{2} {1}", label, value, seperator));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            string key = ExpressionHelper.GetExpressionText(expression).Split(new char[] { '.' }).Last<string>();
            string resource = html.GetResource(key);
            if (string.IsNullOrEmpty(resource))
            {
                try
                {
                    ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
                    resource = metadata.DisplayName ?? (metadata.PropertyName ?? key.Split(new char[] { '.' }).Last<string>());
                    if (string.IsNullOrEmpty(resource))
                    {
                        return MvcHtmlString.Empty;
                    }
                }
                catch (Exception)
                {
                    return MvcHtmlString.Create("No resource with name " + resource);
                }
            }
            TagBuilder builder = new TagBuilder("label");
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(key));
            builder.SetInnerText(resource);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return html.LabelFor<TModel, TValue>(expression, ((IDictionary<string, object>) new RouteValueDictionary(htmlAttributes)));
        }
    }
}

