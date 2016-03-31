using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;

using Coats.Crafts.Configuration;

using DD4T.ContentModel;

namespace Coats.Crafts.Extensions
{
    [Serializable]
    public class ReturnToSearch
    {
        public string Url { get; set; }
        public string Referrer { get; set; }

    }

    public static class SessionExtensions
    {
        private const string _returnToSearch = "RETURN_TO_SEARCH";

        public static string GetLevel1BrandFilter(this HttpSessionStateBase session)
        {
            return session[WebConfiguration.Current.SessionKey] as string;
        }

        public static void SetLevel1BrandFilter(this HttpSessionStateBase session, string value)
        {
            session[WebConfiguration.Current.SessionKey] = value;
        }

        public static void ClearLevel1BrandFilter(this HttpSessionStateBase session)
        {
            session.Remove(WebConfiguration.Current.SessionKey);
        }

        public static void SetReturnToSearchUrl(this HttpSessionStateBase session, string returnUrl, HttpRequestBase request)
        {
            try
            {
                ReturnToSearch rts = new ReturnToSearch 
                { 
                    Url = returnUrl, 
                    Referrer = (request.Url.AbsoluteUri.Contains('?') ? request.Url.AbsoluteUri.Split('?')[0] : request.Url.AbsoluteUri) 
                };
                // Quick and dirty fix to ensure a forward slash on the end of the string.
                if (!rts.Referrer.EndsWith("/")) { rts.Referrer += "/"; }
                session[_returnToSearch] = rts;
            }
            catch (Exception) { session.ClearReturnToSearch(); }
        }

        public static string GetReturnToSearchUrl(this HttpSessionStateBase session, HttpRequestBase request)
        {
            try 
            {
                ReturnToSearch rts = (ReturnToSearch)session[_returnToSearch];
                string currentUrl = (request.Url.AbsoluteUri.Contains('?') ? request.Url.AbsoluteUri.Split('?')[0] : request.Url.AbsoluteUri);
                string referrerUrl = (request.UrlReferrer.AbsoluteUri.Contains('?') ? request.UrlReferrer.AbsoluteUri.Split('?')[0] : request.UrlReferrer.AbsoluteUri);
                // Quick and dirty fix to ensure a forward slash on the end of the string.
                if (!referrerUrl.EndsWith("/")) { referrerUrl += "/"; }

                if (currentUrl.ToLower().Contains(rts.Referrer.ToLower()) && referrerUrl.ToLower() == rts.Referrer.ToLower())
                {
                    return rts.Url;
                }
                return null;
            }
            catch (Exception) 
            {
                session.ClearReturnToSearch();
                return null;
            }
        }

        //public static bool HasReturnToSearchUrl(this HttpSessionStateBase session)
        //{
        //    return (session.GetReturnToSearchUrl() != null);
        //}

        public static void ClearReturnToSearch(this HttpSessionStateBase session)
        {
            session[_returnToSearch] = null;
        }

    }
}