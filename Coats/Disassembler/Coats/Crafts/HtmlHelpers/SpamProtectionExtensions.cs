namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public static class SpamProtectionExtensions
    {
        public static string SpamProtectionTimeStamp(this HtmlHelper helper)
        {
            TagBuilder builder = new TagBuilder("input");
            builder.MergeAttribute("id", "SpamProtectionTimeStamp");
            builder.MergeAttribute("name", "SpamProtectionTimeStamp");
            builder.MergeAttribute("type", "hidden");
            TimeSpan span = (TimeSpan) (DateTime.Now - new DateTime(0x7b2, 1, 1));
            builder.MergeAttribute("value", ((long) span.TotalSeconds).ToString());
            return builder.ToString(TagRenderMode.SelfClosing);
        }
    }
}

