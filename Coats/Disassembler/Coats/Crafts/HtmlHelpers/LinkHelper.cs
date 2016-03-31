namespace Coats.Crafts.HtmlHelpers
{
    using Castle.Windsor;
    using Coats.Crafts.Extensions;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using DD4T.Utils;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class LinkHelper
    {
        public static string BuildLink(string linkurl, string target, string title, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("a");
            builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes));
            builder.MergeAttribute("href", linkurl);
            builder.MergeAttribute("target", target);
            builder.MergeAttribute("title", title);
            builder.SetInnerText(title);
            return builder.ToString();
        }

        public static MvcHtmlString GetLinkTag(this IField link, IField title, object htmlAttributes, string statictitle)
        {
            IFieldSet linkSet = link.EmbeddedValues[0];
            return linkSet.GetNewLinkTag(title, htmlAttributes, statictitle);
        }

        public static MvcHtmlString GetNewLinkTag(this IFieldSet linkSet, IField title, object htmlAttributes, string statictitle)
        {
            string str4;
            UrlHelper helper = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext);
            string str2 = string.Empty;
            if (linkSet.ContainsKey("linkComponent"))
            {
                if (linkSet["linkComponent"].LinkedComponentValues[0].GetResolvedUrl() != "")
                {
                    str2 = helper.Content(linkSet["linkComponent"].LinkedComponentValues[0].GetResolvedUrl().AddApplicationRoot());
                }
            }
            else if (linkSet.ContainsKey("linkURL"))
            {
                str2 = linkSet["linkURL"].Value;
                if (!string.IsNullOrEmpty(str2) && !str2.StartsWith("http://"))
                {
                    str2 = helper.Content(str2.AddApplicationRoot());
                }
            }
            if (string.IsNullOrEmpty(str2))
            {
                str2 = "#";
            }
            string target = "_self";
            if (linkSet.ContainsKey("boolNewWindow"))
            {
                target = (linkSet["boolNewWindow"].Value == "True") ? "_blank" : "_self";
            }
            if (title == null)
            {
                str4 = statictitle ?? linkSet["linkTitle"].Value;
            }
            else
            {
                str4 = title.Value;
            }
            string str = BuildLink(str2, target, str4, htmlAttributes);
            return MvcHtmlString.Create(str);
        }

        public static string GetResolvedUrl(this IComponent component)
        {
            string url = "#nocomponenturi";
            if (!string.IsNullOrEmpty(component.Url))
            {
                url = component.Url.Replace(ConfigurationHelper.DefaultPage, string.Empty);
            }
            if (component.Multimedia != null)
            {
                url = component.Multimedia.Url;
            }
            return url.TrimEnd(new char[] { '/' });
        }

        public static string GetResolvedUrl(this IComponent component, string sourcePageUri, string excludeComponentTemplateUri)
        {
            string url = null;
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            if (applicationInstance != null)
            {
                ILinkFactory instance = applicationInstance.Container.Resolve<ILinkFactory>();
                try
                {
                    if (component.Multimedia != null)
                    {
                        url = component.Multimedia.Url;
                    }
                    else if (string.IsNullOrEmpty(sourcePageUri) && string.IsNullOrEmpty(excludeComponentTemplateUri))
                    {
                        url = instance.ResolveLink(component.Id);
                    }
                    else
                    {
                        url = instance.ResolveLink(sourcePageUri, component.Id, excludeComponentTemplateUri);
                    }
                }
                finally
                {
                    applicationInstance.Container.Release(instance);
                }
            }
            if (url == null)
            {
                url = string.Empty;
            }
            return url.Replace(ConfigurationHelper.DefaultPage, string.Empty);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IComponent comp)
        {
            string title = string.Empty;
            string str2 = string.Empty;
            if (comp != null)
            {
                if (comp.Fields.ContainsKey("title"))
                {
                    title = comp.Fields["title"].Value;
                }
                UrlHelper helper2 = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext);
                str2 = BuildLink(helper2.Content(comp.GetResolvedUrl().AddApplicationRoot()), "", title, new object());
            }
            return MvcHtmlString.Create(str2);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link)
        {
            return link.GetLinkTag(null, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IFieldSet fieldset)
        {
            return fieldset.GetNewLinkTag(null, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, IField title)
        {
            return link.GetLinkTag(title, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, object htmlAttributes)
        {
            return link.GetLinkTag(null, htmlAttributes, null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, IField title, object htmlAttributes)
        {
            return link.GetLinkTag(title, htmlAttributes, null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, string title, object htmlAttributes)
        {
            return link.GetLinkTag(null, htmlAttributes, title);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IFieldSet fieldset, string title, object htmlAttributes)
        {
            return fieldset.GetNewLinkTag(null, htmlAttributes, title);
        }
    }
}

