namespace Coats.Crafts.Extensions
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;

    public static class SessionExtensions
    {
        private const string _returnToSearch = "RETURN_TO_SEARCH";

        public static void ClearLevel1BrandFilter(this HttpSessionStateBase session)
        {
            session.Remove(WebConfiguration.Current.SessionKey);
        }

        public static void ClearReturnToSearch(this HttpSessionStateBase session)
        {
            session["RETURN_TO_SEARCH"] = null;
        }

        public static string GetLevel1BrandFilter(this HttpSessionStateBase session)
        {
            return (session[WebConfiguration.Current.SessionKey] as string);
        }

        public static string GetReturnToSearchUrl(this HttpSessionStateBase session, HttpRequestBase request)
        {
            try
            {
                ReturnToSearch search = (ReturnToSearch) session["RETURN_TO_SEARCH"];
                string str = request.Url.AbsoluteUri.Contains<char>('?') ? request.Url.AbsoluteUri.Split(new char[] { '?' })[0] : request.Url.AbsoluteUri;
                string str2 = request.UrlReferrer.AbsoluteUri.Contains<char>('?') ? request.UrlReferrer.AbsoluteUri.Split(new char[] { '?' })[0] : request.UrlReferrer.AbsoluteUri;
                if (!str2.EndsWith("/"))
                {
                    str2 = str2 + "/";
                }
                if (str.ToLower().Contains(search.Referrer.ToLower()) && (str2.ToLower() == search.Referrer.ToLower()))
                {
                    return search.Url;
                }
                return null;
            }
            catch (Exception)
            {
                session.ClearReturnToSearch();
                return null;
            }
        }

        public static void SetLevel1BrandFilter(this HttpSessionStateBase session, string value)
        {
            session[WebConfiguration.Current.SessionKey] = value;
        }

        public static void SetReturnToSearchUrl(this HttpSessionStateBase session, string returnUrl, HttpRequestBase request)
        {
            try
            {
                ReturnToSearch search = new ReturnToSearch {
                    Url = returnUrl,
                    Referrer = request.Url.AbsoluteUri.Contains<char>('?') ? request.Url.AbsoluteUri.Split(new char[] { '?' })[0] : request.Url.AbsoluteUri
                };
                if (!search.Referrer.EndsWith("/"))
                {
                    search.Referrer = search.Referrer + "/";
                }
                session["RETURN_TO_SEARCH"] = search;
            }
            catch (Exception)
            {
                session.ClearReturnToSearch();
            }
        }
    }
}

