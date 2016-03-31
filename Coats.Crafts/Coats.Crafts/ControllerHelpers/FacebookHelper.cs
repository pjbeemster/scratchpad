using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Extensions;
using DD4T.ContentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using Castle.Windsor;
using System.Text.RegularExpressions;
using Coats.Crafts.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Facebook;
using Coats.Crafts.Models;

namespace Coats.Crafts.ControllerHelpers
{
    public class DynamicJsonObject : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }
        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];
            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }
            return this.Dictionary.ContainsKey(binder.Name);
        }
    }

    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) })); }
        }
    }
    public class oAuthFacebook
    {
        public enum Method { GET, POST };
        public const string GRAPH_API = "https://graph.facebook.com";
        public const string AUTHORIZE = GRAPH_API + "/oauth/authorize";
        public const string ACCESS_TOKEN = GRAPH_API + "/oauth/access_token";
        public const string CALLBACK_URL = "http://www.blahblah.com/facebookcallback.aspx";

        //private string _appId = "";
        //private string _appSecret = "";
        //private string _accessToken = "";

        #region Properties

        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string AccessToken { get; set; }

        #endregion

        public oAuthFacebook() { }
        public oAuthFacebook(string appId, string appSecret, string accessToken)
        {
            AppId = appId;
            AppSecret = appSecret;
            AccessToken = accessToken;
        }

        /// <summary>
        /// Get the link to Facebook's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            return string.Format("{0}?client_id={1}&redirect_uri={2}", AUTHORIZE, this.AppId, CALLBACK_URL);
        }

        public string ExchangeAccessToken(string authToken)
        {
            string accessTokenUrl = string.Format("{0}?grant_type=fb_exchange_token&client_id={1}&client_secret={2}&fb_exchange_token={3}",
            ACCESS_TOKEN, AppId, AppSecret, authToken);

            string response = WebRequest(Method.GET, accessTokenUrl, String.Empty);

            if (response.Length > 0)
            {
                //Store the returned access_token
                NameValueCollection qs = HttpUtility.ParseQueryString(response);

                if (qs["access_token"] != null)
                {
                    AccessToken = qs["access_token"];
                    return qs["access_token"];
                }
            }
            return string.Empty;
        }

        public string PostToWall(string message, string imageUrl, string linkUrl, string linkName, string caption, string description)
        {
            /*
             access_token=YOUR_ACCESS_TOKEN&message=YOUR_MESSAGE&picture=YOUR_PICTURE_URL&link=YOUR_LINK&name=YOUR_LINK_NAME&caption=YOUR_CAPTION
             */

            string postUrl = string.Format("{0}/feed?access_token={1}&message={2}&picture={3}&link={4}&name={5}&caption={6}&description={7}",
                                        GRAPH_API,
                                        this.AccessToken,
                                        message,
                                        imageUrl,
                                        linkUrl,
                                        linkName,
                                        caption,
                                        description);

            string response = WebRequest(Method.POST, postUrl, String.Empty);

            if (response.Length > 0)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
                dynamic postEntry = jss.Deserialize(response, typeof(object)) as dynamic;
                return postEntry.id as string;
            }
            return string.Empty;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "[You user agent]";
            webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());

                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
    }
}