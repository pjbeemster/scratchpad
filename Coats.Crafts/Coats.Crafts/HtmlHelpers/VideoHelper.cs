
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DD4T.ContentModel;

using Coats.Crafts.Extensions;

namespace Coats.Crafts.HtmlHelpers
{
    public static class VideoHelper
    {
        public static MvcHtmlString Video(this HtmlHelper helper, IField video)
        {
            return MvcHtmlString.Create(video.GetVideoTag(String.Empty));
        }

        public static MvcHtmlString Video(this HtmlHelper helper, IField video, object htmlAttributes)
        {
            return MvcHtmlString.Create(video.GetVideoTag(htmlAttributes));
        }

        public static string GetVideoTag(this IField video, object htmlAttributes)
        {
            var imgHtml = string.Empty;

            // Create tag builder
            var builder = new TagBuilder("iframe");

            if (video != null)
            {
                var img = video.LinkedComponentValues[0];
                if (img != null)
                {
                    imgHtml = img.GetVideoTag(htmlAttributes);
                }
            }

            // Render tag
            return imgHtml;
        }

        public static string GetVideoTag(this IComponent video, object htmlAttributes)
        {

            // Create tag builder

            var containerBuilder = new TagBuilder("div");
            //add microdata
            containerBuilder.MergeAttribute("itemscope", null, true);
            containerBuilder.MergeAttribute("itemtype", "http://schema.org/VideoObject", true);

            var microDataHtml = "";

            var vid = video.Fields["youtubeVideo"].EmbeddedValues[0];

            if (vid.ContainsKey("title"))
            {
                var metaTag = new TagBuilder("meta");
                metaTag.MergeAttribute("itemprop", "name");
                metaTag.MergeAttribute("content", vid["title"].Value);

                microDataHtml += metaTag.ToString(TagRenderMode.SelfClosing);
            }

            if (vid.ContainsKey("caption"))
            {
                var metaTag = new TagBuilder("meta");
                metaTag.MergeAttribute("itemprop", "description");
                metaTag.MergeAttribute("content", vid["caption"].Value);

                microDataHtml += metaTag.ToString(TagRenderMode.SelfClosing);
            }

            if (vid.ContainsKey("thumb"))
            {
                var metaTag = new TagBuilder("meta");
                metaTag.MergeAttribute("itemprop", "image");
                metaTag.MergeAttribute("content", vid["thumb"].GetImageURL());

                microDataHtml += metaTag.ToString(TagRenderMode.SelfClosing);
            }

            var metaTagUrl = new TagBuilder("meta");
            metaTagUrl.MergeAttribute("itemprop", "url");
            metaTagUrl.MergeAttribute("content", video.GetVideoURL());

            microDataHtml += metaTagUrl.ToString(TagRenderMode.SelfClosing);

            var builder = new TagBuilder("iframe");

            // Add attributes
            builder.MergeAttribute("src", video.GetVideoURL());

            if (vid.ContainsKey("width"))
            {
                builder.MergeAttribute("width", vid["width"].Value);
            }

            if (vid.ContainsKey("height"))
            {
                builder.MergeAttribute("height", vid["height"].Value);
            }

            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag

            containerBuilder.InnerHtml = microDataHtml + builder.ToString(TagRenderMode.Normal);

            return containerBuilder.ToString(TagRenderMode.Normal);
        }



        public static string GetVideoURL(this IComponent video)
        {
            var videoCompSrc = string.Empty;

            if (video.Fields.ContainsKey("youtubeVideo"))
            {
                videoCompSrc = video.Fields["youtubeVideo"].EmbeddedValues[0]["url"].Value;
            }

            var embeddedUrl = GetEmbeddedYouTubeUrl(videoCompSrc);
            return embeddedUrl;
        }

        public static string GetEmbeddedYouTubeUrl(string url)
        {
            string embeddedUrl = url;
            Uri videoUri = new Uri(url);

            int segmentCt = videoUri.Segments.Length;

            string youtubeID = videoUri.Segments[videoUri.Segments.Length - 1];

            // ASH: Returning the controls to youtube content ("controls=1").
            //embeddedUrl = string.Format("http://www.youtube.com/embed/{0}?showinfo=0&rel=0&showsearch=0&controls=0&wmode=transparent", youtubeID);
            embeddedUrl = string.Format("http://www.youtube.com/embed/{0}?showinfo=0&rel=0&showsearch=0&controls=1&wmode=transparent", youtubeID);

            return embeddedUrl;
        }
    }
}