using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Coats.Crafts.Configuration;
using System.Globalization;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Text.RegularExpressions;

namespace Coats.Crafts.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this String str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(str);
        }

        public static string AddApplicationRoot(this String path)
        {
            //var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            //var logger = accessor.Container.Resolve<ILogger>();
            //logger.DebugFormat("StringExtension AddApplicationRoot - path = {0}", path);
            //logger.DebugFormat("StringExtension AddApplicationRoot - applicationpath = {0}", HttpContext.Current.Request.ApplicationPath);

            if (path != null)
            {
                if (path.StartsWith("~") || path.StartsWith(HttpContext.Current.Request.ApplicationPath))
                {
                    return path;
                }
                else
                {
                    if (!path.StartsWith("/"))
                        return string.Format("~/{0}", path);

                    return string.Format("~{0}", path);
                }
            }

            return string.Empty;
        }

        public static bool IsPromo(this String schematitle)
        {
            return schematitle == WebConfiguration.Current.PromoSchemaTitle;
        }

        public static bool IsGeneralContent(this String schematitle)
        {
            return schematitle == WebConfiguration.Current.GeneralContentSchemaTitle;
        }


        /// <summary>
        /// Gets the category keyword.
        /// Returns a keyword based on the path and the level down the path required
        /// @Model.Component.Categories[0].Keywords[0].Path.GetCategoryKeyword(2)
        /// i.e. If path = "\Access\Tools\Tool One"
        /// Access = 1, Tools = 2, Tool One = 3 etc..
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>

        public static string GetCategoryKeyword(this String path, int level)
        {
            string category = "";

            if (path.Contains('\\'))
            {
                string[] splitCat = path.Split('\\');

                if (level < splitCat.Count())
                {
                    category = splitCat[level];
                }
            }

            return category;
        }

        /// <summary>
        /// Simple occurs counter for a given token
        /// </summary>
        /// <param name="str">Container string</param>
        /// <param name="token">The token to count</param>
        /// <returns>The number of occurances of token within string</returns>
        public static int Occurs(this String str, string token)
        {
            return ((Regex.Split(str, token).Count()) - 1);
        }

    }
}