namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    public static class GravatarHtmlHelper
    {
        private static string GetDescription(this Enum en)
        {
            MemberInfo[] member = en.GetType().GetMember(en.ToString());
            if ((member != null) && (member.Length > 0))
            {
                object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    return ((DescriptionAttribute) customAttributes[0]).Description;
                }
            }
            return en.ToString();
        }

        private static string GetMd5Hash(string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static HtmlString GravatarImage(this HtmlHelper htmlHelper, string emailAddress, int size = 80, DefaultImage defaultImage = 2, string defaultImageUrl = "", bool forceDefaultImage = false, Rating rating = 0, bool forceSecureRequest = false)
        {
            TagBuilder builder = new TagBuilder("img");
            emailAddress = string.IsNullOrEmpty(emailAddress) ? string.Empty : emailAddress.Trim().ToLower();
            builder.Attributes.Add("src", string.Format("{0}://{1}.gravatar.com/avatar/{2}?s={3}{4}{5}{6}", new object[] { (htmlHelper.ViewContext.HttpContext.Request.IsSecureConnection || forceSecureRequest) ? "https" : "http", (htmlHelper.ViewContext.HttpContext.Request.IsSecureConnection || forceSecureRequest) ? "secure" : "www", GetMd5Hash(emailAddress), size.ToString(), "&d=" + (!string.IsNullOrEmpty(defaultImageUrl) ? HttpUtility.UrlEncode(defaultImageUrl) : defaultImage.GetDescription()), forceDefaultImage ? "&f=y" : "", "&r=" + rating.GetDescription() }));
            builder.Attributes.Add("class", "gravatar");
            builder.Attributes.Add("alt", "Gravatar image");
            return new HtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public enum DefaultImage
        {
            [Description("")]
            Default = 0,
            [Description("404")]
            Http404 = 1,
            [Description("identicon")]
            Identicon = 3,
            [Description("monsterid")]
            MonsterId = 4,
            [Description("mm")]
            MysteryMan = 2,
            [Description("retro")]
            Retro = 6,
            [Description("wavatar")]
            Wavatar = 5
        }

        public enum Rating
        {
            [Description("g")]
            G = 0,
            [Description("pg")]
            PG = 1,
            [Description("r")]
            R = 2,
            [Description("x")]
            X = 3
        }
    }
}

