namespace Coats.Crafts.ControllerHelpers
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Script.Serialization;

    public class oAuthFacebook
    {
        public const string ACCESS_TOKEN = "https://graph.facebook.com/oauth/access_token";
        public const string AUTHORIZE = "https://graph.facebook.com/oauth/authorize";
        public const string CALLBACK_URL = "http://www.blahblah.com/facebookcallback.aspx";
        public const string GRAPH_API = "https://graph.facebook.com";

        public oAuthFacebook()
        {
        }

        public oAuthFacebook(string appId, string appSecret, string accessToken)
        {
            this.AppId = appId;
            this.AppSecret = appSecret;
            this.AccessToken = accessToken;
        }

        public string AuthorizationLinkGet()
        {
            return string.Format("{0}?client_id={1}&redirect_uri={2}", "https://graph.facebook.com/oauth/authorize", this.AppId, "http://www.blahblah.com/facebookcallback.aspx");
        }

        public string ExchangeAccessToken(string authToken)
        {
            string url = string.Format("{0}?grant_type=fb_exchange_token&client_id={1}&client_secret={2}&fb_exchange_token={3}", new object[] { "https://graph.facebook.com/oauth/access_token", this.AppId, this.AppSecret, authToken });
            string query = this.WebRequest(Method.GET, url, string.Empty);
            if (query.Length > 0)
            {
                NameValueCollection values = HttpUtility.ParseQueryString(query);
                if (values["access_token"] != null)
                {
                    this.AccessToken = values["access_token"];
                    return values["access_token"];
                }
            }
            return string.Empty;
        }

        public string PostToWall(string message, string imageUrl, string linkUrl, string linkName, string caption, string description)
        {
            string url = string.Format("{0}/feed?access_token={1}&message={2}&picture={3}&link={4}&name={5}&caption={6}&description={7}", new object[] { "https://graph.facebook.com", this.AccessToken, message, imageUrl, linkUrl, linkName, caption, description });
            string input = this.WebRequest(Method.POST, url, string.Empty);
            if (input.Length > 0)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
                dynamic obj2 = serializer.Deserialize(input, typeof(object));
                return (obj2.id as string);
            }
            return string.Empty;
        }

        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter writer = null;
            string str = "";
            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "[You user agent]";
            webRequest.Timeout = 0x4e20;
            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";
                writer = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    writer.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    writer.Close();
                    writer = null;
                }
            }
            str = this.WebResponseGet(webRequest);
            webRequest = null;
            return str;
        }

        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader reader = null;
            string str = "";
            try
            {
                reader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                str = reader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                reader.Close();
                reader = null;
            }
            return str;
        }

        public string AccessToken { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public enum Method
        {
            GET,
            POST
        }
    }
}

