using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using Coats.Crafts.Configuration;
using Coats.Crafts.Utils;

using DD4T.Mvc.Html;
using DD4T.ContentModel;
using DD4T.Factories;
using DD4T.ContentModel.Factories;

using Castle.Windsor;

namespace Coats.Crafts.HtmlHelpers
{
    public static class RichTextHelper
    {
        /// <summary>
        /// xhtml namespace uri
        /// </summary>
        private const string XhtmlNamespaceUri = "http://www.w3.org/1999/xhtml";
        /// <summary>
        /// xlink namespace uri
        /// </summary>
        private const string XlinkNamespaceUri = "http://www.w3.org/1999/xlink";

        /// <summary>
        /// Extension method on String to resolve rich text. 
        /// Use as: Model.Field["key"].Value.ResolveRichText()
        /// </summary>
        /// <param name="value"></param>
        /// <returns>MvcHtmlString (resolved rich text)</returns>
        public static MvcHtmlString ResolveRichText(this String value)
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            var linkFactory = accessor.Container.Resolve<ILinkFactory>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format("<xhtml>{0}</xhtml>", value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("xhtml", XhtmlNamespaceUri);
            nsmgr.AddNamespace("xlink", XlinkNamespaceUri);

            try
            {

                // resolve links which haven't been resolved
                foreach (XmlNode link in doc.SelectNodes("//xhtml:a[@xlink:href[starts-with(string(.),'tcm:')]][@xhtml:href='' or not(@xhtml:href)]", nsmgr))
                {
                    string tcmuri = link.Attributes["xlink:href"].Value;
                    string linkUrl = linkFactory.ResolveLink(tcmuri);
                    if (!string.IsNullOrEmpty(linkUrl))
                    {
                        linkUrl = General.AdjustUrlToContext(linkUrl);

                        // add href
                        XmlAttribute href = doc.CreateAttribute("xhtml:href");
                        href.Value = linkUrl;
                        link.Attributes.Append(href);

                        // remove all xlink attributes
                        foreach (XmlAttribute xlinkAttr in link.SelectNodes("//@xlink:*", nsmgr))
                        {
                            link.Attributes.Remove(xlinkAttr);
                        }
                    }
                    else
                    {
                        // copy child nodes of link so we keep them
                        foreach (XmlNode child in link.ChildNodes)
                        {
                            link.ParentNode.InsertBefore(child.CloneNode(true), link);
                        }
                        // remove link node
                        link.ParentNode.RemoveChild(link);
                    }
                }
            }
            finally
            {
                accessor.Container.Release(linkFactory);
            }

            // check for any blockquote elements
            foreach (XmlElement node in doc.SelectNodes("//xhtml/p[@class[starts-with(string(.), 'Blockquote')]]", nsmgr))
            {
                XmlElement blockquote = doc.CreateElement("blockquote");
                blockquote.InnerText = node.InnerText;
                node.ParentNode.InsertBefore(blockquote, node);

                node.ParentNode.RemoveChild(node);
            }

            // check for any quote elements
            foreach (XmlElement node in doc.SelectNodes("//xhtml/p[@class[starts-with(string(.), 'Quote')]]", nsmgr))
            {
                XmlElement blockquote = doc.CreateElement("blockquote");
                XmlAttribute style = doc.CreateAttribute("class");
                style.InnerText = "quote";
                blockquote.Attributes.Append(style);

                blockquote.InnerText = node.InnerText;
                node.ParentNode.InsertBefore(blockquote, node);

                node.ParentNode.RemoveChild(node);
            }


            //From test Track: 
            // <img xlink:href="tcm:35-4285" title="Fabrics with smooth surfaces" alt="Fabrics with smooth surfaces" 
            // style="width: 200px; height: 142px;" xlink:title="SP-seam pucker problems5" src="/en/Images/SP-seam%20pucker%20problems5_tcm35-4285.jpg"> 
            //I think that in images the "title" attribute is not required. If this is an image that is also a link, 
            //the "title" attribute should be included, but should refer to the link destination - the alt should refer to the content of the image (these will usually be related but not always identical. For example, on the above image if it was a link you could have 
            //Title: <Link destination> 
            //Alt: “Fabrics with smooth surfaces” 


            // adjust any internal links to context
            foreach (XmlElement img in doc.SelectNodes("//a[@href]", nsmgr))
            {
                XmlAttribute href = img.GetAttributeNode("href");
                if (href != null)
                {
                    if (href.Value.StartsWith("/"))
                    {
                        href.Value = General.AdjustUrlToContext(href.Value);
                    }
                }
            }

            // add application context path to images
            foreach (XmlElement img in doc.SelectNodes("//*[@src]", nsmgr))
            {
                XmlAttribute xSrcAttr = img.GetAttributeNode("src");
                if (xSrcAttr != null)
                    xSrcAttr.Value = General.AdjustUrlToContext(xSrcAttr.Value);

                //remove Title Attribute
                XmlAttribute xTitleAttr = img.GetAttributeNode("title");
                if (xTitleAttr != null)
                {
                    img.Attributes.Remove(xTitleAttr);
                }
                //check for Alt
                XmlAttribute xAltAttr = img.GetAttributeNode("alt");
                if (xAltAttr == null)
                {
                    xAltAttr = doc.CreateAttribute("alt");
                    //if title exsists use that value
                    if (xTitleAttr != null)
                    {
                        xAltAttr.Value = xTitleAttr.Value;
                        img.Attributes.Append(xAltAttr);
                    }
                }
            }

            // Fix self-closing IFRAME tag - Tridion converts them into a self-closing tag - add empty string to put back to </iframe>
            foreach (XmlElement iframe in doc.SelectNodes("//iframe", nsmgr))
            {
                if (String.IsNullOrEmpty(iframe.InnerText))
                    iframe.InnerText = "";
            }

            return new MvcHtmlString(RemoveNamespaceReferences(doc.DocumentElement.InnerXml));
        }

        /// <summary>
        /// Extension method on Model.Field to resolve rich text for a specified index.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="index"></param>
        /// <returns>MvcHtmlString (resolved rich text)</returns>
        public static MvcHtmlString ResolveRichText(this IField field, int index)
        {
            if (field.FieldType == FieldType.Xhtml)
            {
                return ResolveRichText(field.Values[index]);
            }

            return new MvcHtmlString(".ResolveRichText() only works on rich text fields...");
        }

        /// <summary>
        /// Extension method on Model.Field to resolve rich text for the first value of the field.
        /// </summary>
        /// <param name="field"></param>
        /// <returns>MvcHtmlString (resolved rich text)</returns>
        public static MvcHtmlString ResolveRichText(this IField field)
        {
            return ResolveRichText(field, 0);
        }

        /// <summary>
        /// removes unwanted namespace references (like xhtml and xlink) from the html
        /// </summary>
        /// <param name="html">html as a string</param>
        /// <returns>html as a string without namespace references</returns>
        private static string RemoveNamespaceReferences(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("xmlns=\"\"", "");
                html = html.Replace(string.Format("xmlns=\"{0}\"", XhtmlNamespaceUri), "");
                html = html.Replace(string.Format("xmlns:xhtml=\"{0}\"", XhtmlNamespaceUri), "");
                html = html.Replace(string.Format("xmlns:xlink=\"{0}\"", XlinkNamespaceUri), "");
            }

            return html;
        }
    }
}
