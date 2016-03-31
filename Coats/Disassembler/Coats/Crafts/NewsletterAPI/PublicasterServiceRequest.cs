namespace Coats.Crafts.NewsletterAPI
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    public class PublicasterServiceRequest
    {
        public static string PUBLICASTER_DELIMITER = ";";
        public static string PUBLICASTER_GET_METHOD = "GET";
        public static string PUBLICASTER_LINEBREAK = "\r\n";
        public static string PUBLICASTER_POST_METHOD = "POST";

        public bool ConfirmPublicaster(string Email)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            bool flag = false;
            if (!string.IsNullOrEmpty(Email))
            {
                string publicasterSubscribeUrl = WebConfiguration.Current.PublicasterSubscribeUrl;
                string publicasterMailingListId = WebConfiguration.Current.PublicasterMailingListId;
                int num = Convert.ToInt32(WebConfiguration.Current.PublicasterGlobalStatus);
                int num2 = Convert.ToInt32(WebConfiguration.Current.PublicasterListActiveStatus);
                publicasterSubscribeUrl = publicasterSubscribeUrl + publicasterMailingListId;
                int num3 = Convert.ToInt32(WebConfiguration.Current.PublicasteruserID);
                StringBuilder builder = new StringBuilder();
                List<Datum> list = new List<Datum>();
                Datum datum = new Datum {
                    Key = "NewsletterConfirmed",
                    Value = "Yes"
                };
                list.Add(datum);
                Item item = new Item {
                    Data = list,
                    DateCreated = "/Date(1338494487897-0400)/",
                    Email = Email,
                    GlobalStatus = num,
                    ListStatus = num2,
                    SubscriberID = num3,
                    LastModified = "/Date(1338494487897-0400)/"
                };
                RootObject obj2 = new RootObject {
                    Item = item
                };
                builder.Append(JsonConvert.SerializeObject(obj2, Formatting.Indented));
                try
                {
                    HttpWebResponse response = this.sendJSON(builder.ToString(), publicasterSubscribeUrl, PUBLICASTER_POST_METHOD);
                    if ((response != null) && (response.StatusCode.ToString() == "200"))
                    {
                        flag = true;
                    }
                }
                catch (Exception exception)
                {
                    logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster" + exception, new object[0]);
                    return flag;
                }
                return flag;
            }
            return true;
        }

        public bool createJsonPublicasterRequest(string Email, string FirstName, string LastName, string Interests, string DisplayName, string Newsletter)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            if (string.IsNullOrEmpty(Interests))
            {
                Interests = " ";
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = " ";
            }
            if (string.IsNullOrEmpty(Newsletter))
            {
                Newsletter = " ";
            }
            bool flag = false;
            if ((!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(FirstName)) && !string.IsNullOrEmpty(LastName))
            {
                string publicasterSubscribeUrl = WebConfiguration.Current.PublicasterSubscribeUrl;
                string publicasterMailingListId = WebConfiguration.Current.PublicasterMailingListId;
                int num = Convert.ToInt32(WebConfiguration.Current.PublicasterGlobalStatus);
                int num2 = Convert.ToInt32(WebConfiguration.Current.PublicasterListPendingStatus);
                publicasterSubscribeUrl = publicasterSubscribeUrl + publicasterMailingListId;
                int num3 = Convert.ToInt32(WebConfiguration.Current.PublicasteruserID);
                StringBuilder builder = new StringBuilder();
                List<Datum> list = new List<Datum>();
                Datum datum = new Datum {
                    Key = "FirstName",
                    Value = FirstName
                };
                list.Add(datum);
                Datum datum2 = new Datum {
                    Key = "LastName",
                    Value = LastName
                };
                list.Add(datum2);
                Datum datum3 = new Datum {
                    Key = "DisplayName",
                    Value = DisplayName
                };
                list.Add(datum3);
                Datum datum4 = new Datum {
                    Key = "Interests",
                    Value = Interests
                };
                list.Add(datum4);
                Datum datum5 = new Datum {
                    Key = "NewsletterOpted",
                    Value = Newsletter
                };
                list.Add(datum5);
                Item item = new Item {
                    Data = list,
                    DateCreated = "/Date(1338494487897-0400)/",
                    Email = Email,
                    GlobalStatus = num,
                    ListStatus = num2,
                    SubscriberID = num3,
                    LastModified = "/Date(1338494487897-0400)/"
                };
                RootObject obj2 = new RootObject {
                    Item = item
                };
                builder.Append(JsonConvert.SerializeObject(obj2, Formatting.Indented));
                try
                {
                    HttpWebResponse response = this.sendJSON(builder.ToString(), publicasterSubscribeUrl, PUBLICASTER_POST_METHOD);
                    if ((response != null) && (response.StatusCode.ToString() == "200"))
                    {
                        flag = true;
                    }
                }
                catch (Exception exception)
                {
                    logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster" + exception, new object[0]);
                    return flag;
                }
                return flag;
            }
            return true;
        }

        private HttpWebResponse sendJSON(string publicasterJsonObject, string connectionUrl, string requestMethod)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            HttpWebResponse response = null;
            string publicasterEncryptedAccountID = WebConfiguration.Current.PublicasterEncryptedAccountID;
            string publicasterApiPassword = WebConfiguration.Current.PublicasterApiPassword;
            Uri requestUri = new Uri(connectionUrl);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUri);
            request.ContentType = "application/json";
            request.Method = requestMethod;
            request.Headers.Add("Authorization", publicasterEncryptedAccountID + ":" + publicasterApiPassword);
            try
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(publicasterJsonObject);
                    writer.Flush();
                    writer.Close();
                    HttpWebResponse response2 = (HttpWebResponse) request.GetResponse();
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string str3 = reader.ReadToEnd();
                    }
                    response = response2;
                }
            }
            catch (IOException exception)
            {
                logger.ErrorFormat("Unable to initiate communication with Publicaster" + exception, new object[0]);
            }
            catch (Exception exception2)
            {
                logger.ErrorFormat("Exception generating while sending message to Publicaster" + exception2, new object[0]);
            }
            return response;
        }

        public bool UnSubscripePublicaster(string Email)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            bool flag = false;
            if (!string.IsNullOrEmpty(Email))
            {
                string publicasterUnSubscribeUserUrl = WebConfiguration.Current.PublicasterUnSubscribeUserUrl;
                string publicasterMailingListId = WebConfiguration.Current.PublicasterMailingListId;
                publicasterUnSubscribeUserUrl = publicasterUnSubscribeUserUrl + publicasterMailingListId;
                int num = Convert.ToInt32(WebConfiguration.Current.PublicasteruserID);
                StringBuilder builder = new StringBuilder();
                UnSubscribeJSON ejson = new UnSubscribeJSON {
                    Item = Email
                };
                builder.Append(JsonConvert.SerializeObject(ejson, Formatting.Indented));
                try
                {
                    HttpWebResponse response = this.sendJSON(builder.ToString(), publicasterUnSubscribeUserUrl, PUBLICASTER_POST_METHOD);
                    if ((response != null) && (response.StatusCode.ToString() == "200"))
                    {
                        flag = true;
                    }
                }
                catch (Exception exception)
                {
                    logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster" + exception, new object[0]);
                    return flag;
                }
                return flag;
            }
            return true;
        }

        public bool UpdatePublicaster(string Email, string NewsletterConfirmed)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger logger = applicationInstance.Container.Resolve<ILogger>();
            bool flag = false;
            if (!string.IsNullOrEmpty(Email))
            {
                string publicasterSubscribeUrl = WebConfiguration.Current.PublicasterSubscribeUrl;
                string publicasterMailingListId = WebConfiguration.Current.PublicasterMailingListId;
                int num = Convert.ToInt32(WebConfiguration.Current.PublicasterGlobalStatus);
                int num2 = Convert.ToInt32(WebConfiguration.Current.PublicasterListPendingStatus);
                publicasterSubscribeUrl = publicasterSubscribeUrl + publicasterMailingListId;
                int num3 = Convert.ToInt32(WebConfiguration.Current.PublicasteruserID);
                StringBuilder builder = new StringBuilder();
                List<Datum> list = new List<Datum>();
                Datum datum = new Datum {
                    Key = "NewsletterConfirmed",
                    Value = NewsletterConfirmed
                };
                list.Add(datum);
                Item item = new Item {
                    Data = list,
                    DateCreated = "/Date(1338494487897-0400)/",
                    Email = Email,
                    GlobalStatus = num,
                    ListStatus = num2,
                    SubscriberID = num3,
                    LastModified = "/Date(1338494487897-0400)/"
                };
                RootObject obj2 = new RootObject {
                    Item = item
                };
                builder.Append(JsonConvert.SerializeObject(obj2, Formatting.Indented));
                try
                {
                    HttpWebResponse response = this.sendJSON(builder.ToString(), publicasterSubscribeUrl, PUBLICASTER_POST_METHOD);
                    if ((response != null) && (response.StatusCode.ToString() == "200"))
                    {
                        flag = true;
                    }
                }
                catch (Exception exception)
                {
                    logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster" + exception, new object[0]);
                    return flag;
                }
                return flag;
            }
            return true;
        }
    }
}

