using System.Linq;
using System.Web;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Extensions;
using DD4T.ContentModel;
//using Coats.IndustrialPortal.Configuration;
using System;
using System.Text;
using Castle.Windsor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Coats.Crafts.Configuration;

using Newtonsoft.Json;
using System.Net;
using System.IO;
namespace Coats.Crafts.NewsletterAPI
{

    public class Datum
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Item
    {
        public List<Datum> Data { get; set; }
        public string DateCreated { get; set; }
        public string Email { get; set; }
        public int GlobalStatus { get; set; }
        public string LastModified { get; set; }
        public int ListStatus { get; set; }
        public int SubscriberID { get; set; }
    }

    public class RootObject
    {
        public object EditLink { get; set; }
        public Item Item { get; set; }
    }

    public class UnSubscribeJSON
    {
        public object Item { get; set; }
    }

    public class PublicasterServiceRequest
    {
        public static string PUBLICASTER_DELIMITER = ";";
        public static string PUBLICASTER_LINEBREAK = "\r\n";
        public static string PUBLICASTER_POST_METHOD = "POST";
        public static string PUBLICASTER_GET_METHOD = "GET";


        public bool UnSubscripePublicaster(string Email)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            ILogger Logger = accessor.Container.Resolve<ILogger>();

            bool status = false;
            if (!String.IsNullOrEmpty(Email))
            {
                string PublicasterSubscribeUrl = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterUnSubscribeUserUrl;
                string mailinglistId = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterMailingListId;
               // int PublicasterGlobalStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterGlobalStatus);
             //   int PublicasterListStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterListActiveStatus);
                PublicasterSubscribeUrl = PublicasterSubscribeUrl + mailinglistId;

                int userId = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasteruserID);


                StringBuilder publicasterJsonObject = new StringBuilder();

                UnSubscribeJSON objUnSubscribeJSON = new UnSubscribeJSON();
                objUnSubscribeJSON.Item = Email;

                publicasterJsonObject.Append(JsonConvert.SerializeObject(objUnSubscribeJSON, Formatting.Indented));

                try
                {
                    HttpWebResponse conn = sendJSON(publicasterJsonObject.ToString(), PublicasterSubscribeUrl, PUBLICASTER_POST_METHOD);


                    if (conn != null && conn.StatusCode.ToString() == "200")
                    {
                        status = true;
                    }
                }
                catch (Exception ex)
                {

                    Logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster"
                           + ex);
                    return status;
                }
            }
            else
            {
                status = true;
            }
            return status;

        }

        public bool ConfirmPublicaster(string Email)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            ILogger Logger = accessor.Container.Resolve<ILogger>();

            bool status = false;
            if (!String.IsNullOrEmpty(Email))
            {
                string PublicasterSubscribeUrl = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterSubscribeUrl;
                string mailinglistId = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterMailingListId;
                int PublicasterGlobalStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterGlobalStatus);
                int PublicasterListStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterListActiveStatus);
                PublicasterSubscribeUrl = PublicasterSubscribeUrl + mailinglistId;

                int userId = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasteruserID);


                StringBuilder publicasterJsonObject = new StringBuilder();

                List<Datum> parts = new List<Datum>();
                // Add parts to the list.
                parts.Add(new Datum() { Key = "NewsletterConfirmed", Value = "Yes" });


                Item item = new Item();
                item.Data = parts;
                item.DateCreated = "/Date(1338494487897-0400)/";
                item.Email = Email;
                item.GlobalStatus = PublicasterGlobalStatus;
                item.ListStatus = PublicasterListStatus;
                //  item.SubscriberID = 1975581;
                item.SubscriberID = userId;
                item.LastModified = "/Date(1338494487897-0400)/";
                RootObject root = new RootObject();
                root.Item = item;

                publicasterJsonObject.Append(JsonConvert.SerializeObject(root, Formatting.Indented));

                try
                {
                    HttpWebResponse conn = sendJSON(publicasterJsonObject.ToString(), PublicasterSubscribeUrl, PUBLICASTER_POST_METHOD);


                    if (conn != null && conn.StatusCode.ToString() == "200")
                    {
                        status = true;
                    }
                }
                catch (Exception ex)
                {

                    Logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster"
                           + ex);
                    return status;
                }
            }
            else
            {
                status = true;
            }
            return status;

        }
        public bool UpdatePublicaster(string Email, string NewsletterConfirmed)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            ILogger Logger = accessor.Container.Resolve<ILogger>();

            bool status = false;
            if (!String.IsNullOrEmpty(Email))
            {
                string PublicasterSubscribeUrl = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterSubscribeUrl;
                string mailinglistId = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterMailingListId;
                int PublicasterGlobalStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterGlobalStatus);
                int PublicasterListStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterListPendingStatus);
                PublicasterSubscribeUrl = PublicasterSubscribeUrl + mailinglistId;

                int userId = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasteruserID);


                StringBuilder publicasterJsonObject = new StringBuilder();
                
                List<Datum> parts = new List<Datum>();
                // Add parts to the list.
                parts.Add(new Datum() { Key = "NewsletterConfirmed", Value = NewsletterConfirmed });
                

                Item item = new Item();
                item.Data = parts;
                item.DateCreated = "/Date(1338494487897-0400)/";
                item.Email = Email;
                item.GlobalStatus = PublicasterGlobalStatus;
                item.ListStatus = PublicasterListStatus;
              //  item.SubscriberID = 1975581;
                item.SubscriberID = userId;
                item.LastModified = "/Date(1338494487897-0400)/";
                RootObject root = new RootObject();
                root.Item = item;

                publicasterJsonObject.Append(JsonConvert.SerializeObject(root, Formatting.Indented));

                try
                {
                    HttpWebResponse conn = sendJSON(publicasterJsonObject.ToString(), PublicasterSubscribeUrl, PUBLICASTER_POST_METHOD);


                    if (conn != null && conn.StatusCode.ToString() == "200")
                    {
                        status = true;
                    }
                }
                catch (Exception ex)
                {

                    Logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster"
                           + ex);
                    return status;
                }
            }
            else
            {
                status = true;
            }
            return status;

        }
        public bool createJsonPublicasterRequest(string Email, string FirstName, string LastName, string Interests, string DisplayName, string Newsletter)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            ILogger Logger = accessor.Container.Resolve<ILogger>();

            if (String.IsNullOrEmpty(Interests))
                Interests = " ";
            if (String.IsNullOrEmpty(DisplayName))
                DisplayName = " ";
            if (String.IsNullOrEmpty(Newsletter))
                Newsletter = " ";

            bool status = false;
            if (!String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName))
            {
                string PublicasterSubscribeUrl = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterSubscribeUrl;
                string mailinglistId = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterMailingListId;
                int PublicasterGlobalStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterGlobalStatus);
                int PublicasterListStatus = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterListPendingStatus);
                PublicasterSubscribeUrl = PublicasterSubscribeUrl + mailinglistId;

                int userId = Convert.ToInt32(Coats.Crafts.Configuration.WebConfiguration.Current.PublicasteruserID);


                StringBuilder publicasterJsonObject = new StringBuilder();
                
                List<Datum> parts = new List<Datum>();
                // Add parts to the list.
                parts.Add(new Datum() { Key = "FirstName", Value = FirstName });
                parts.Add(new Datum() { Key = "LastName", Value = LastName });
                parts.Add(new Datum() { Key = "DisplayName", Value = DisplayName });
                parts.Add(new Datum() { Key = "Interests", Value = Interests });
                parts.Add(new Datum() { Key = "NewsletterOpted", Value = Newsletter });
                

                Item item = new Item();
                item.Data = parts;
                item.DateCreated = "/Date(1338494487897-0400)/";
                item.Email = Email;
                item.GlobalStatus = PublicasterGlobalStatus;
                item.ListStatus = PublicasterListStatus;
              //  item.SubscriberID = 1975581;
                item.SubscriberID = userId;
                item.LastModified = "/Date(1338494487897-0400)/";
                RootObject root = new RootObject();
                root.Item = item;

                publicasterJsonObject.Append(JsonConvert.SerializeObject(root, Formatting.Indented));

                try
                {
                    HttpWebResponse conn = sendJSON(publicasterJsonObject.ToString(), PublicasterSubscribeUrl, PUBLICASTER_POST_METHOD);


                    if (conn != null && conn.StatusCode.ToString() == "200")
                    {
                        status = true;
                    }
                }
                catch (Exception ex)
                {

                    Logger.ErrorFormat("Exception generating in createJsonPublicasterRequest method of PublicasterServiceRequest while sending message to Publicaster"
                           + ex);
                    return status;
                }
            }
            else
            {
                status = true;
            }
            return status;

        }

        private HttpWebResponse sendJSON(string publicasterJsonObject, string connectionUrl, string requestMethod)
        {
            IContainerAccessor accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            ILogger Logger = accessor.Container.Resolve<ILogger>();

            HttpWebResponse conn = null;
            string publicasterEncryptedAccountId = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterEncryptedAccountID;
            string publicasterApiPassword = Coats.Crafts.Configuration.WebConfiguration.Current.PublicasterApiPassword;

            Uri url = new Uri(connectionUrl);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = requestMethod;
            httpWebRequest.Headers.Add("Authorization", publicasterEncryptedAccountId + ":" + publicasterApiPassword);
           

            try
            {

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {


                    streamWriter.Write(publicasterJsonObject);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                    conn = httpResponse;
                }
            }
            catch (IOException e)
            {
                Logger.ErrorFormat("Unable to initiate communication with Publicaster" + e);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Exception generating while sending message to Publicaster" + e);
            }
            return conn;
        }

    }
}