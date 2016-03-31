using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coats.Crafts.HtmlHelpers
{
    public static class SpamProtectionExtensions
    {
        public static string SpamProtectionTimeStamp(this HtmlHelper helper)
        {
            var builder = new TagBuilder("input");
            builder.MergeAttribute("id", "SpamProtectionTimeStamp");
            builder.MergeAttribute("name", "SpamProtectionTimeStamp");
            builder.MergeAttribute("type", "hidden");
            builder.MergeAttribute("value", ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds).ToString());
            return builder.ToString(TagRenderMode.SelfClosing);
        }

    }

    public class SpamProtectionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            long timestamp = long.MaxValue;

            if (Int64.TryParse(filterContext.RequestContext.HttpContext.Request.Params["SpamProtectionTimeStamp"], out timestamp))
            {
                long currentTime = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

                if (currentTime <= timestamp + 1)
                {
                    throw new HttpException("Spam Protection: Invalid form submission.");
                }
            }
            else
            {
                throw new HttpException("Spam Protection: Invalid form submission. Invalid timestamp parameter.");
            }
        }
    }
}
