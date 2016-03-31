namespace Coats.Crafts.Extensions
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;

    public static class StringExtensions
    {
        public static string AddApplicationRoot(this string path)
        {
            if (path != null)
            {
                if (path.StartsWith("~") || path.StartsWith(HttpContext.Current.Request.ApplicationPath))
                {
                    return path;
                }
                if (!path.StartsWith("/"))
                {
                    return string.Format("~/{0}", path);
                }
                return string.Format("~{0}", path);
            }
            return string.Empty;
        }

        public static string GetCategoryKeyword(this string path, int level)
        {
            string str = "";
            if (path.Contains<char>('\\'))
            {
                string[] source = path.Split(new char[] { '\\' });
                if (level < source.Count<string>())
                {
                    str = source[level];
                }
            }
            return str;
        }

        public static bool IsGeneralContent(this string schematitle)
        {
            return (schematitle == WebConfiguration.Current.GeneralContentSchemaTitle);
        }

        public static bool IsPromo(this string schematitle)
        {
            return (schematitle == WebConfiguration.Current.PromoSchemaTitle);
        }

        public static int Occurs(this string str, string token)
        {
            return (Regex.Split(str, token).Count<string>() - 1);
        }

        public static string ToTitleCase(this string str)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
        }
    }
}

