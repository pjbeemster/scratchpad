namespace Coats.Crafts.HtmlHelpers
{
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class VideoHelper
    {
        public static string GetEmbeddedYouTubeUrl(string url)
        {
            string str = url;
            Uri uri = new Uri(url);
            int length = uri.Segments.Length;
            string str2 = uri.Segments[uri.Segments.Length - 1];
            return string.Format("http://www.youtube.com/embed/{0}?showinfo=0&rel=0&showsearch=0&controls=1&wmode=transparent", str2);
        }

        public static string GetVideoTag(this IComponent video, object htmlAttributes)
        {
            TagBuilder builder2;
            TagBuilder builder = new TagBuilder("div");
            builder.MergeAttribute("itemscope", null, true);
            builder.MergeAttribute("itemtype", "http://schema.org/VideoObject", true);
            string str = "";
            IFieldSet set = video.Fields["youtubeVideo"].EmbeddedValues[0];
            if (set.ContainsKey("title"))
            {
                builder2 = new TagBuilder("meta");
                builder2.MergeAttribute("itemprop", "name");
                builder2.MergeAttribute("content", set["title"].Value);
                str = str + builder2.ToString(TagRenderMode.SelfClosing);
            }
            if (set.ContainsKey("caption"))
            {
                builder2 = new TagBuilder("meta");
                builder2.MergeAttribute("itemprop", "description");
                builder2.MergeAttribute("content", set["caption"].Value);
                str = str + builder2.ToString(TagRenderMode.SelfClosing);
            }
            if (set.ContainsKey("thumb"))
            {
                builder2 = new TagBuilder("meta");
                builder2.MergeAttribute("itemprop", "image");
                builder2.MergeAttribute("content", set["thumb"].GetImageURL());
                str = str + builder2.ToString(TagRenderMode.SelfClosing);
            }
            TagBuilder builder3 = new TagBuilder("meta");
            builder3.MergeAttribute("itemprop", "url");
            builder3.MergeAttribute("content", video.GetVideoURL());
            str = str + builder3.ToString(TagRenderMode.SelfClosing);
            TagBuilder builder4 = new TagBuilder("iframe");
            builder4.MergeAttribute("src", video.GetVideoURL());
            if (set.ContainsKey("width"))
            {
                builder4.MergeAttribute("width", set["width"].Value);
            }
            if (set.ContainsKey("height"))
            {
                builder4.MergeAttribute("height", set["height"].Value);
            }
            builder4.MergeAttributes<string, object>(new RouteValueDictionary(htmlAttributes));
            builder.InnerHtml = str + builder4.ToString(TagRenderMode.Normal);
            return builder.ToString(TagRenderMode.Normal);
        }

        public static string GetVideoTag(this IField video, object htmlAttributes)
        {
            string videoTag = string.Empty;
            TagBuilder builder = new TagBuilder("iframe");
            if (video != null)
            {
                IComponent component = video.LinkedComponentValues[0];
                if (component != null)
                {
                    videoTag = component.GetVideoTag(htmlAttributes);
                }
            }
            return videoTag;
        }

        public static string GetVideoURL(this IComponent video)
        {
            string url = string.Empty;
            if (video.Fields.ContainsKey("youtubeVideo"))
            {
                url = video.Fields["youtubeVideo"].EmbeddedValues[0]["url"].Value;
            }
            return GetEmbeddedYouTubeUrl(url);
        }

        public static MvcHtmlString Video(this HtmlHelper helper, IField video)
        {
            return MvcHtmlString.Create(video.GetVideoTag(string.Empty));
        }

        public static MvcHtmlString Video(this HtmlHelper helper, IField video, object htmlAttributes)
        {
            return MvcHtmlString.Create(video.GetVideoTag(htmlAttributes));
        }
    }
}

