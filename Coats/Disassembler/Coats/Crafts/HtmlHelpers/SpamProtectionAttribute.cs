namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    public class SpamProtectionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            long result = 0x7fffffffffffffffL;
            if (!long.TryParse(filterContext.RequestContext.HttpContext.Request.Params["SpamProtectionTimeStamp"], out result))
            {
                throw new HttpException("Spam Protection: Invalid form submission. Invalid timestamp parameter.");
            }
            TimeSpan span = (TimeSpan) (DateTime.Now - new DateTime(0x7b2, 1, 1));
            long totalSeconds = (long) span.TotalSeconds;
            if (totalSeconds <= (result + 1L))
            {
                throw new HttpException("Spam Protection: Invalid form submission.");
            }
        }
    }
}

