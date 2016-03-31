namespace Coats.Crafts.HtmlHelpers
{
    using Coats.Crafts.Configuration;
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class ImageHelper
    {
        private static void BuildImage(IComponent image, object htmlAttributes, TagBuilder builder)
        {
            builder.MergeAttribute("src", image.GetImageURL());
            builder.MergeAttribute("alt", image.GetImageAltText());
            builder.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes));
            if (image.MetadataFields.ContainsKey("defaultMetadata") && image.MetadataFields["defaultMetadata"].EmbeddedValues[0].ContainsKey("title"))
            {
                builder.MergeAttribute("title", image.MetadataFields["defaultMetadata"].EmbeddedValues[0]["title"].Value);
                if (string.IsNullOrEmpty(builder.Attributes["alt"]))
                {
                    builder.MergeAttribute("alt", image.MetadataFields["defaultMetadata"].EmbeddedValues[0]["title"].Value, true);
                }
            }
        }

        public static string BuildImageWithLink(this IComponent image, string InnerImage)
        {
            TagBuilder builder = new TagBuilder("a");
            builder.MergeAttribute("href", image.GetImageRedirectURL());
            if (WebConfiguration.Current.OurBrandsOpenInNewTab.Contains(image.Title))
            {
                builder.MergeAttribute("target", "_blank");
            }
            builder.InnerHtml = InnerImage;
            return builder.ToString();
        }

        public static string GetImageAltText(this IComponent image)
        {
            string str = string.Empty;
            if (image.MetadataFields.ContainsKey("alt"))
            {
                str = image.MetadataFields["alt"].Value;
            }
            return str;
        }

        public static string GetImagePhysicalPath(this IField image)
        {
            IComponent component = image.LinkedComponentValues[0];
            if (component == null)
            {
                return string.Empty;
            }
            string str = string.Empty;
            if ((component.Multimedia != null) && (component.Multimedia.Url != null))
            {
                return GetImagePhysicalPath(component.Multimedia.Url);
            }
            if ((component.Fields.ContainsKey("imageTextCollection") && (component.Fields["imageTextCollection"].EmbeddedValues.Count > 0)) && (component.Fields["imageTextCollection"].EmbeddedValues[0]["image"].LinkedComponentValues.Count > 0))
            {
                return GetImagePhysicalPath(component.Fields["imageTextCollection"].EmbeddedValues[0]["image"].LinkedComponentValues[0].Multimedia.Url);
            }
            if ((component.Fields.ContainsKey("youtubeVideo") && (component.Fields["youtubeVideo"].EmbeddedValues.Count > 0)) && (component.Fields["youtubeVideo"].EmbeddedValues[0]["thumb"].LinkedComponentValues.Count > 0))
            {
                return GetImagePhysicalPath(component.Fields["youtubeVideo"].EmbeddedValues[0]["thumb"].LinkedComponentValues[0].Multimedia.Url);
            }
            return str;
        }

        public static string GetImagePhysicalPath(string imgUrl)
        {
            UrlHelper helper = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext);
            string leftPart = helper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            return string.Format("{0}{1}", leftPart, helper.Content(imgUrl));
        }

        public static string GetImageRedirectURL(this IComponent image)
        {
            string str = string.Empty;
            if (image.MetadataFields.ContainsKey("redirecturl"))
            {
                str = image.MetadataFields["redirecturl"].Value;
            }
            return str;
        }

        public static string GetImageTag(this IComponent image, object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("img");
            BuildImage(image, htmlAttributes, builder);
            string innerImage = string.Empty;
            string str2 = builder.ToString(TagRenderMode.SelfClosing);
            if (str2.Contains("data_img"))
            {
                innerImage = str2.Replace("data_img", "data-img");
            }
            else
            {
                innerImage = str2;
            }
            if (image.GetImageRedirectURL() != string.Empty)
            {
                innerImage = image.BuildImageWithLink(innerImage);
            }
            return innerImage;
        }

        public static string GetImageTag(this IField image, object htmlAttributes)
        {
            string imageTag = string.Empty;
            TagBuilder builder = new TagBuilder("img");
            if (image != null)
            {
                IComponent component = image.LinkedComponentValues[0];
                if (component != null)
                {
                    imageTag = component.GetImageTag(htmlAttributes);
                }
            }
            return imageTag;
        }

        public static string GetImageTag(this IComponent image, object htmlAttributes, string classCss)
        {
            TagBuilder builder = new TagBuilder("img");
            BuildImage(image, htmlAttributes, builder);
            builder.MergeAttribute("class", classCss);
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        public static string GetImageURL(this IComponent image)
        {
            string str = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext).Content(image.Multimedia.Url);
            if (WebConfiguration.Current.RemoteImagePath != null)
            {
                str = WebConfiguration.Current.RemoteImagePath + str;
            }
            return str;
        }

        public static string GetImageURL(this IField image)
        {
            IComponent component = image.LinkedComponentValues[0];
            if (component == null)
            {
                return string.Empty;
            }
            string str = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext).Content(component.Multimedia.Url);
            if (WebConfiguration.Current.RemoteImagePath != null)
            {
                str = WebConfiguration.Current.RemoteImagePath + str;
            }
            return str;
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IComponent image)
        {
            return MvcHtmlString.Create(image.GetImageTag(string.Empty));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IField image)
        {
            return MvcHtmlString.Create(image.GetImageTag(string.Empty));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IComponent image, object htmlAttributes)
        {
            return MvcHtmlString.Create(image.GetImageTag(htmlAttributes));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IField image, object htmlAttributes)
        {
            return MvcHtmlString.Create(image.GetImageTag(htmlAttributes));
        }
    }
}

