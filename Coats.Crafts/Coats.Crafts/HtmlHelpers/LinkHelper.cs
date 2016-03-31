using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DD4T.ContentModel;
using DD4T.ContentModel.Factories;
using Castle.Windsor;
using Coats.Crafts.Extensions;


namespace Coats.Crafts.HtmlHelpers
{
    public static class LinkHelper
    {
        /// <summary>
        /// resolve the url for the current component via dynamic linking, if component is a multimedia component, then Multimedia.Url will be returned.
        /// 25/4/2012 updated to check if Url has already been set
        /// </summary>
        /// <param name="component">the component to call the method on</param>
        /// <returns>resolved url</returns>
        public static string GetResolvedUrl(this IComponent component)
        {
            string link = "#nocomponenturi";
            if (!String.IsNullOrEmpty(component.Url))
            {
                link = component.Url.Replace(DD4T.Utils.ConfigurationHelper.DefaultPage, String.Empty);
            }

            if (component.Multimedia != null)
            {
                link = component.Multimedia.Url;
            }

            return link.TrimEnd('/');

            //return string.IsNullOrEmpty(component.Url) ? 
            //    GetResolvedUrl(component, null, null) : 
            //    component.Url.Replace(DD4T.Utils.ConfigurationHelper.DefaultPage, String.Empty);
            //return string.IsNullOrEmpty(component.Url) ? String.Empty : component.Url;
        }

        /// <summary>
        /// resolve the url for the current component via dynamic linking, if component is a multimedia component, then Multimedia.Url will be returned.
        /// </summary>
        /// <param name="component">the component to call the method on</param>        
        /// <param name="sourcePageUri">source page uri</param>
        /// <param name="excludeComponentTemplateUri">component template uri to exclude</param>
        /// <returns>resolved url</returns>
        public static string GetResolvedUrl(this IComponent component, string sourcePageUri, string excludeComponentTemplateUri)
        {
            string link = null;
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            if (accessor != null)
            {
                var linkFactory = accessor.Container.Resolve<ILinkFactory>();

                try
                {
                    if (component.Multimedia != null)
                    {
                        link = component.Multimedia.Url;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(sourcePageUri) && string.IsNullOrEmpty(excludeComponentTemplateUri))
                        {
                            link = linkFactory.ResolveLink(component.Id);
                        }
                        else
                        {
                            link = linkFactory.ResolveLink(sourcePageUri, component.Id, excludeComponentTemplateUri);
                        }
                    }
                }
                finally
                {
                    accessor.Container.Release(linkFactory);
                }
            }

            if (link == null)
                link = String.Empty;

            return link.Replace(DD4T.Utils.ConfigurationHelper.DefaultPage, String.Empty);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link)
        {
            return link.GetLinkTag(null, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, object htmlAttributes)
        {
            return link.GetLinkTag(null, htmlAttributes, null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, IField title)
        {
            return link.GetLinkTag(title, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, IField title, object htmlAttributes)
        {
            return link.GetLinkTag(title, htmlAttributes, null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IField link, string title, object htmlAttributes)
        {
            return link.GetLinkTag(null, htmlAttributes, title);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IFieldSet fieldset)
        {
            return fieldset.GetNewLinkTag(null, new object(), null);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, IFieldSet fieldset, string title, object htmlAttributes)
        {
            return fieldset.GetNewLinkTag(null, htmlAttributes, title);
        }

        /// <summary>
        /// Return a link based on IComponent
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="comp">The comp.</param>
        /// <returns></returns>
        public static MvcHtmlString Link(this HtmlHelper helper, IComponent comp)
        {
            var linktitle = string.Empty;
            var linktag = string.Empty;

            if (comp != null)
            {
                if (comp.Fields.ContainsKey("title"))
                {
                    linktitle = comp.Fields["title"].Value;
                }

                var urlHelper = new UrlHelper(
                                       ((MvcHandler)HttpContext.Current.Handler).RequestContext);

                string linkurl = urlHelper.Content(comp.GetResolvedUrl().AddApplicationRoot());
                linktag = BuildLink(linkurl, "", linktitle, new object());
            }
            return MvcHtmlString.Create(linktag);
        }


        public static MvcHtmlString GetLinkTag(this IField link, IField title, object htmlAttributes, string statictitle)
        {
            var linkComp = link.EmbeddedValues[0];
                return GetNewLinkTag(linkComp, title, htmlAttributes, statictitle);
        }

        public static MvcHtmlString GetNewLinkTag(this IFieldSet linkSet, IField title, object htmlAttributes, string statictitle)
        {
            var urlHelper = new UrlHelper(
                ((MvcHandler)HttpContext.Current.Handler).RequestContext);

            string linktag;
            {
                var linkurl = String.Empty;

                if (linkSet.ContainsKey("linkComponent"))
                {
                    if (linkSet["linkComponent"].LinkedComponentValues[0].GetResolvedUrl() != "")
                    {
                        linkurl = urlHelper.Content(
                            linkSet["linkComponent"].LinkedComponentValues[0].GetResolvedUrl().AddApplicationRoot());
                    }
                }
                else
                {
                    if (linkSet.ContainsKey("linkURL"))
                    {
                        linkurl = linkSet["linkURL"].Value;
                        if (!String.IsNullOrEmpty(linkurl))
                        {
                            if (!linkurl.StartsWith("http://"))
                            {
                                linkurl = urlHelper.Content(linkurl.AddApplicationRoot());
                            }
                        }
                    }
                }

                if (String.IsNullOrEmpty(linkurl))
                    linkurl = "#";

                var target = "_self";
                if (linkSet.ContainsKey("boolNewWindow"))
                {
                    target = linkSet["boolNewWindow"].Value == "True" ? "_blank" : "_self";
                }

                string titlestring;
                if (title == null)
                {
                    titlestring = statictitle ?? linkSet["linkTitle"].Value;
                }
                else
                {
                    titlestring = title.Value;
                }

                linktag = BuildLink(linkurl, target, titlestring, htmlAttributes);
            }

            // Render tag
            return MvcHtmlString.Create(linktag);
        }

        public static string BuildLink(string linkurl, string target, string title, object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder("a");
            // Add attributes
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            builder.MergeAttribute("href", linkurl);
            builder.MergeAttribute("target", target);
            builder.MergeAttribute("title", title);

            builder.SetInnerText(title);

            string linktag = builder.ToString();

            return linktag;
        }

    }
}