namespace Coats.Crafts.HtmlHelpers
{
    using Castle.Windsor;
    using Coats.Crafts.Utils;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;

    public static class RichTextHelper
    {
        private const string XhtmlNamespaceUri = "http://www.w3.org/1999/xhtml";
        private const string XlinkNamespaceUri = "http://www.w3.org/1999/xlink";

        private static string RemoveNamespaceReferences(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("xmlns=\"\"", "");
                html = html.Replace(string.Format("xmlns=\"{0}\"", "http://www.w3.org/1999/xhtml"), "");
                html = html.Replace(string.Format("xmlns:xhtml=\"{0}\"", "http://www.w3.org/1999/xhtml"), "");
                html = html.Replace(string.Format("xmlns:xlink=\"{0}\"", "http://www.w3.org/1999/xlink"), "");
            }
            return html;
        }

        public static MvcHtmlString ResolveRichText(this IField field)
        {
            return field.ResolveRichText(0);
        }

        public static MvcHtmlString ResolveRichText(this string value)
        {
            XmlAttribute attributeNode;
            XmlElement element2;
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILinkFactory instance = applicationInstance.Container.Resolve<ILinkFactory>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(string.Format("<xhtml>{0}</xhtml>", value));
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
            try
            {
                foreach (XmlNode node in document.SelectNodes("//xhtml:a[@xlink:href[starts-with(string(.),'tcm:')]][@xhtml:href='' or not(@xhtml:href)]", nsmgr))
                {
                    string componentUri = node.Attributes["xlink:href"].Value;
                    string str2 = instance.ResolveLink(componentUri);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        str2 = General.AdjustUrlToContext(str2);
                        attributeNode = document.CreateAttribute("xhtml:href");
                        attributeNode.Value = str2;
                        node.Attributes.Append(attributeNode);
                        foreach (XmlAttribute attribute2 in node.SelectNodes("//@xlink:*", nsmgr))
                        {
                            node.Attributes.Remove(attribute2);
                        }
                    }
                    else
                    {
                        foreach (XmlNode node2 in node.ChildNodes)
                        {
                            node.ParentNode.InsertBefore(node2.CloneNode(true), node);
                        }
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
            finally
            {
                applicationInstance.Container.Release(instance);
            }
            foreach (XmlElement element in document.SelectNodes("//xhtml/p[@class[starts-with(string(.), 'Blockquote')]]", nsmgr))
            {
                element2 = document.CreateElement("blockquote");
                element2.InnerText = element.InnerText;
                element.ParentNode.InsertBefore(element2, element);
                element.ParentNode.RemoveChild(element);
            }
            foreach (XmlElement element in document.SelectNodes("//xhtml/p[@class[starts-with(string(.), 'Quote')]]", nsmgr))
            {
                element2 = document.CreateElement("blockquote");
                XmlAttribute attribute3 = document.CreateAttribute("class");
                attribute3.InnerText = "quote";
                element2.Attributes.Append(attribute3);
                element2.InnerText = element.InnerText;
                element.ParentNode.InsertBefore(element2, element);
                element.ParentNode.RemoveChild(element);
            }
            foreach (XmlElement element3 in document.SelectNodes("//a[@href]", nsmgr))
            {
                attributeNode = element3.GetAttributeNode("href");
                if ((attributeNode != null) && attributeNode.Value.StartsWith("/"))
                {
                    attributeNode.Value = General.AdjustUrlToContext(attributeNode.Value);
                }
            }
            foreach (XmlElement element3 in document.SelectNodes("//*[@src]", nsmgr))
            {
                XmlAttribute attribute4 = element3.GetAttributeNode("src");
                if (attribute4 != null)
                {
                    attribute4.Value = General.AdjustUrlToContext(attribute4.Value);
                }
                XmlAttribute attribute5 = element3.GetAttributeNode("title");
                if (attribute5 != null)
                {
                    element3.Attributes.Remove(attribute5);
                }
                if (element3.GetAttributeNode("alt") == null)
                {
                    XmlAttribute attribute6 = document.CreateAttribute("alt");
                    if (attribute5 != null)
                    {
                        attribute6.Value = attribute5.Value;
                        element3.Attributes.Append(attribute6);
                    }
                }
            }
            foreach (XmlElement element4 in document.SelectNodes("//iframe", nsmgr))
            {
                if (string.IsNullOrEmpty(element4.InnerText))
                {
                    element4.InnerText = "";
                }
            }
            return new MvcHtmlString(RemoveNamespaceReferences(document.DocumentElement.InnerXml));
        }

        public static MvcHtmlString ResolveRichText(this IField field, int index)
        {
            if (field.FieldType == FieldType.Xhtml)
            {
                return field.Values[index].ResolveRichText();
            }
            return new MvcHtmlString(".ResolveRichText() only works on rich text fields...");
        }
    }
}

