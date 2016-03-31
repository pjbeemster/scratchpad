
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DD4T.ContentModel;
using Coats.Crafts.Configuration;


namespace Coats.Crafts.HtmlHelpers
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper helper, IField image)
        {
            return MvcHtmlString.Create(image.GetImageTag(String.Empty));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IField image, object htmlAttributes)
        {
            //var newatts = new object();
            //string att = htmlAttributes.ToString();
            //if (att.Contains("data_img"))
            //{
            //    newatts = att.Replace("data_img", "data-img");
            //}
            return MvcHtmlString.Create(image.GetImageTag(htmlAttributes));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IComponent image)
        {
            return MvcHtmlString.Create(image.GetImageTag(String.Empty));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, IComponent image, object htmlAttributes)
        {
            return MvcHtmlString.Create(image.GetImageTag(htmlAttributes));
        }

        public static string GetImageTag(this IField image, object htmlAttributes)
        {
            var imgHtml = string.Empty;

            // Create tag builder
            var builder = new TagBuilder("img");

            if (image != null)
            {
                var img = image.LinkedComponentValues[0];
                if (img != null)
                {
                    imgHtml = img.GetImageTag(htmlAttributes);
                }
            }

            // Render tag
            return imgHtml;
        }

        public static string GetImageTag(this IComponent image, object htmlAttributes, string classCss)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Add attributes
            BuildImage(image, htmlAttributes, builder);
            builder.MergeAttribute("class", classCss);

            // Render tag
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        private static void BuildImage(IComponent image, object htmlAttributes, TagBuilder builder)
        {

            builder.MergeAttribute("src", image.GetImageURL());
            builder.MergeAttribute("alt", image.GetImageAltText());
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            if (image.MetadataFields.ContainsKey("defaultMetadata"))
            {
                if (image.MetadataFields["defaultMetadata"].EmbeddedValues[0].ContainsKey("title"))
                {
                    // add title attribute to img
                    builder.MergeAttribute("title", image.MetadataFields["defaultMetadata"].EmbeddedValues[0]["title"].Value);
                    if (string.IsNullOrEmpty(builder.Attributes["alt"]))
                    {
                        //add alt attribute from title
                        builder.MergeAttribute("alt", image.MetadataFields["defaultMetadata"].EmbeddedValues[0]["title"].Value, true);
                    }
                    //remove title attribute - 
                    // commented out Remove.("title")
                    // request to have all images having a title attribute - COATSCRAFTSWARRANTY-18 - G Coutts
                   // builder.Attributes.Remove("title");
                }
            }

            //if (image.MetadataFields.ContainsKey("title"))
            //{
            //    if (string.IsNullOrEmpty(builder.Attributes["alt"]))
            //    {
            //        //add alt attribute from title
            //        builder.MergeAttribute("alt", image.MetadataFields["title"].Value);
            //    }
            //    //remove title attribute
            //    builder.Attributes.Remove("title");
            //}
        }

        public static string GetImageTag(this IComponent image, object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            BuildImage(image, htmlAttributes, builder);

            // Render tag
            string ret = string.Empty;
            string outstring = builder.ToString(TagRenderMode.SelfClosing);
            if (outstring.Contains("data_img"))
            {
                ret = outstring.Replace("data_img", "data-img");
            }
            else
            {
                ret = outstring;
            }
            //Added by Ajaya on 22-7-14 for Brand Logo
            if (GetImageRedirectURL(image) != string.Empty)
            {
                ret = BuildImageWithLink(image, ret);
            }
            return ret;
        }

		public static string GetImagePhysicalPath(this IField image)
		{

			var img = image.LinkedComponentValues[0];
			if (img == null) {
				return String.Empty;
			}

            // This img component could actually be a Generic.Image or Generic.Imagetext.Multi or Generic.YouTube
            var imgURL = String.Empty;

            // Generic.Image?
            if (img.Multimedia != null)
                if (img.Multimedia.Url != null)
                    return GetImagePhysicalPath(img.Multimedia.Url);

            // Generic.Imagetext.Multi?
            if (img.Fields.ContainsKey("imageTextCollection"))
                if (img.Fields["imageTextCollection"].EmbeddedValues.Count > 0)
                     if (img.Fields["imageTextCollection"].EmbeddedValues[0]["image"].LinkedComponentValues.Count > 0)
                           return GetImagePhysicalPath(img.Fields["imageTextCollection"].EmbeddedValues[0]["image"].LinkedComponentValues[0].Multimedia.Url);

            // Generic.YouTube
            if (img.Fields.ContainsKey("youtubeVideo"))
                if (img.Fields["youtubeVideo"].EmbeddedValues.Count > 0)
                     if (img.Fields["youtubeVideo"].EmbeddedValues[0]["thumb"].LinkedComponentValues.Count > 0)
                           return GetImagePhysicalPath(img.Fields["youtubeVideo"].EmbeddedValues[0]["thumb"].LinkedComponentValues[0].Multimedia.Url);

			return imgURL;
		}

		public static string GetImagePhysicalPath(string imgUrl)
		{
			var urlHelper = new UrlHelper(((MvcHandler)HttpContext.Current.Handler).RequestContext);

			Uri requestUri = urlHelper.RequestContext.HttpContext.Request.Url;
			string baseUrl = requestUri.GetLeftPart(UriPartial.Authority);
			var imgURL = String.Format("{0}{1}", baseUrl, urlHelper.Content(imgUrl));
			
			return imgURL;
		} 

		//public static string GetImageURL(string imagePath)
		//{
		//    return WebConfiguration.Current.RemoteImagePath + imagePath;
		//}



        public static string GetImageURL(this IField image)
        {
            var img = image.LinkedComponentValues[0];
            if (img == null)
                return String.Empty;

            var urlHelper = new UrlHelper(
                ((MvcHandler)HttpContext.Current.Handler).RequestContext);

            var imgURL = urlHelper.Content(img.Multimedia.Url);

            if (WebConfiguration.Current.RemoteImagePath != null)
            {
                imgURL = WebConfiguration.Current.RemoteImagePath + imgURL;
            }

            return imgURL;
        }

        public static string GetImageURL(this IComponent image)
        {
            var urlHelper = new UrlHelper(
                ((MvcHandler)HttpContext.Current.Handler).RequestContext);

            var imgURL = urlHelper.Content(image.Multimedia.Url);

            if (WebConfiguration.Current.RemoteImagePath != null)
            {
                imgURL = WebConfiguration.Current.RemoteImagePath + imgURL;
            }
            return imgURL;
        }

        public static string GetImageAltText(this IComponent image)
        {
            var alt = String.Empty;

            if (image.MetadataFields.ContainsKey("alt"))
            {
                alt = image.MetadataFields["alt"].Value;
            }
            return alt;
        }
        /// <summary>
        /// Get GetImageRedirectURL ,added by Ajaya on 22-7-14
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetImageRedirectURL(this IComponent image)
        {
            var redirecturl = String.Empty;

            if (image.MetadataFields.ContainsKey("redirecturl"))
            {
                redirecturl = image.MetadataFields["redirecturl"].Value;
            }
            return redirecturl;
        }

        /// <summary>
        /// Get Get Image In Anchor Tag ,added by Ajaya on 22-7-14
        /// </summary>
        /// <param name="image"></param>
        /// <param name="InnerImage"></param>
        /// <returns></returns>
        public static string BuildImageWithLink(this IComponent image, string InnerImage)
        {
            string linktag = string.Empty;


            // Create tag builder
            var builder = new TagBuilder("a");
            // Add attributes

            builder.MergeAttribute("href", GetImageRedirectURL(image));
            if (WebConfiguration.Current.OurBrandsOpenInNewTab.Contains(image.Title))
            {
                builder.MergeAttribute("target", "_blank");
            }
            
            builder.InnerHtml = InnerImage;
            
            linktag = builder.ToString();


            return linktag;

        }
    }
    public class ReplaceChar
    {
        public static RouteValueDictionary AnonymousObjectToHtmlAttributes(object htmlAttributes)
        {
            RouteValueDictionary result = new RouteValueDictionary();
            if (htmlAttributes != null)
            {
                foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(htmlAttributes))
                {
                    result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }
            return result;
        }
   
    }

}