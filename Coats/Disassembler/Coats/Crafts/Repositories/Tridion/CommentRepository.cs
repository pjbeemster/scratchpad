namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.CDS;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using global::Tridion.ContentDelivery.UGC.WebService;

    public class CommentRepository : ICommentRepository
    {
        private IAppSettings _settings;

        public CommentRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public string AddComment(string comment, string itemUri, string userId, string userName)
        {
            string str = "0";
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.AddComment Comment {0}, itemUri {1}", new object[] { comment, itemUri });
            }
            try
            {
                WebServiceClient client = new WebServiceClient();
                string str2 = this.Authenticate("");
                TridionTcmUri tcmUri = UtilityHelper.GetTcmUri(itemUri);
                Coats.Crafts.CDS.Comment comment2 = new Coats.Crafts.CDS.Comment();
                User user = new User {
                    Id = userId,
                    Name = userName
                };
                comment = comment.Replace(Environment.NewLine, "#nl");
                comment2.Content = comment;
                if (this.CommentModeration.ToLower() == "on")
                {
                    comment2.Status = 0;
                }
                else
                {
                    comment2.Status = 2;
                }
                comment2.ItemPublicationId = tcmUri.TcmPublicationID;
                comment2.ItemId = tcmUri.TcmItemId;
                comment2.ItemType = tcmUri.TcmItemType;
                comment2.ModeratedDate = new DateTime?(DateTime.UtcNow);
                comment2.LastModifiedDate = DateTime.UtcNow;
                comment2.CreationDate = DateTime.UtcNow;
                comment2.Score = 0;
                comment2.Moderator = "";
                comment2.User = user;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = client.UploadString("/Comments", "POST", "{d:" + serializer.Serialize(comment2) + "}");
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("AddComment exception - {0}", new object[] { exception });
            }
            return str;
        }

        public string AddRating(string ratingValue, string itemUri, string userId, string displayName)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.AddRating ratingValue {0}, itemUri {1}", new object[] { ratingValue, itemUri });
            }
            string str = this.CheckRating(itemUri, userId, true);
            string str2 = "0";
            if (str != "rated")
            {
                try
                {
                    WebServiceClient client = new WebServiceClient();
                    TridionTcmUri tcmUri = UtilityHelper.GetTcmUri(itemUri);
                    User user = new User {
                        Id = userId,
                        Name = displayName
                    };
                    Rating rating = new Rating {
                        CreationDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        ItemPublicationId = tcmUri.TcmPublicationID,
                        ItemId = tcmUri.TcmItemId,
                        ItemType = tcmUri.TcmItemType,
                        RatingValue = ratingValue.ToString(),
                        User = user,
                        Id = "0"
                    };
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    str2 = client.UploadString("/Ratings", "POST", "{d:" + serializer.Serialize(rating) + "}");
                }
                catch (Exception exception)
                {
                    this.Logger.ErrorFormat("AddRating exception - {0}", new object[] { exception });
                }
            }
            return str2;
        }

        public string Authenticate(string authType = "")
        {
            int num;
            string input = string.Empty;
            string str = string.Empty;
            string oDataEndPointSecurityEndpoint = this._settings.ODataEndPointSecurityEndpoint;
            string oDataEndPointSecurityClientId = this._settings.ODataEndPointSecurityClientId;
            string oDataEndPointSecurityClientSecret = this._settings.ODataEndPointSecurityClientSecret;
            if (authType == "moderator")
            {
                oDataEndPointSecurityEndpoint = this._settings.ModeratorODataEndPointSecurityEndpoint;
                oDataEndPointSecurityClientId = this._settings.ModeratorODataEndPointSecurityClientId;
                oDataEndPointSecurityClientSecret = this._settings.ModeratorODataEndPointSecurityClientSecret;
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.Authenticate ODataEndPointSecurityEndpoint {0}", new object[] { oDataEndPointSecurityEndpoint });
            }
            try
            {
                num = Convert.ToDateTime(HttpContext.Current.Session["oAuthExpire"]).CompareTo(DateTime.Now);
            }
            catch (ArgumentException)
            {
                num = 0;
            }
            if ((num == 0) || (num < 0))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        NameValueCollection data = new NameValueCollection();
                        data["client_id"] = oDataEndPointSecurityClientId;
                        data["client_secret"] = oDataEndPointSecurityClientSecret;
                        byte[] bytes = client.UploadValues(oDataEndPointSecurityEndpoint, "POST", data);
                        input = Encoding.Default.GetString(bytes);
                    }
                    object obj2 = new JavaScriptSerializer().Deserialize<object>(input);
                    str = (string) ((dynamic) obj2)["access_token"];
                    str = HttpUtility.UrlDecode(str, Encoding.UTF8);
                    int startIndex = str.IndexOf("expiresOn=") + 10;
                    int length = str.IndexOf("&digest") - startIndex;
                    long unixTime = Convert.ToInt64(str.Substring(startIndex, length));
                    HttpContext.Current.Session["oAuthExpire"] = UtilityHelper.FromUnixTime(unixTime).ToString();
                    HttpContext.Current.Session["oAuthToken"] = str;
                }
                catch (Exception exception)
                {
                    this.Logger.ErrorFormat("Authenticate exception - {0}", new object[] { exception });
                }
                return str;
            }
            return HttpContext.Current.Session["oAuthToken"].ToString();
        }

        public string CheckRating(string itemUri, string user, bool remove)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.CheckRating itemUri {0}, itemUri {1} ", new object[] { itemUri, user });
            }
            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                ContentDeliveryService cDSClient = this.CDSClient;
                cDSClient.IgnoreMissingProperties = true;
                IQueryable<Rating> source = from p in cDSClient.Ratings.Expand("User")
                    where ((p.ItemPublicationId == tcm.TcmPublicationID) && (p.ItemId == tcm.TcmItemId)) && (p.User.Id == user)
                    select p;
                if (source.Count<Rating>() > 0)
                {
                    foreach (Rating rating in source)
                    {
                        if (remove)
                        {
                            this.DeleteRating(rating.Id);
                        }
                        else
                        {
                            return rating.RatingValue;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("GetRating exception - {0}", new object[] { exception });
                return "rated";
            }
            return "";
        }

        public string DeleteRating(string ratingID)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.DeleteRating ratingId {0}", new object[] { ratingID });
            }
            WebRequest request = WebRequest.Create(this._settings.ModeratorODataEndPoint + "/Ratings(Id=" + ratingID + ")");
            request.Headers.Clear();
            string str = this.Authenticate("moderator");
            request.Headers["authorization"] = "OAuth " + str;
            request.Method = "DELETE";
            return request.GetResponse().ToString();
        }

        public IList<Coats.Crafts.CDS.Comment> GetAllCommentsByUser(string itemUri, string username, int numberToReturn)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.GetAllCommentsByUser numberToReturn {0}, numberToReturn {1}", new object[] { numberToReturn, username });
            }
            List<CDS.Comment> list = null;
            try
            {
                TridionTcmUri tcmUri = UtilityHelper.GetTcmUri(itemUri);
                int publicationId = tcmUri.TcmPublicationID;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ContentDeliveryService cDSClient = this.CDSClient;
                if (this.CommentModeration.ToLower() == "on")
                {
                    list = (from p in cDSClient.Comments
                        where (p.ItemPublicationId == publicationId) && (p.User.Name == username)
                        where p.Status == 2
                        orderby p.CreationDate descending
                        select p).Take(numberToReturn).ToList();
                }
                else
                {
                    list = (from c in cDSClient.Comments
                        where (c.ItemPublicationId == publicationId) && (c.User.Id == username)
                        select c into p
                        orderby p.CreationDate descending
                        select p).Take(numberToReturn).ToList();
                }
                stopwatch.Stop();
                string str = stopwatch.Elapsed.ToString();
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("GetComments exception - {0}", new object[] { exception });
            }
            return list;
        }

        public int GetCommentCount(string itemUri)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.GetCommentCount itemUri {0}", new object[] { itemUri });
            }
            int num = 0;
            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                num = (from p in this.CDSClient.Comments
                    where (p.ItemPublicationId == tcm.TcmPublicationID) && (p.ItemId == tcm.TcmItemId)
                    select p).Take<Coats.Crafts.CDS.Comment>(0x3e8).Count<Coats.Crafts.CDS.Comment>();
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("GetCommentCount exception - {0}", new object[] { exception });
            }
            return num;
        }

        public IList<Coats.Crafts.CDS.Comment> GetComments(string itemUri, int numberToReturn)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.GetComments itemUri {0}, numberToReturn {1}", new object[] { itemUri, numberToReturn });
            }
            List<Coats.Crafts.CDS.Comment> list = new List<Coats.Crafts.CDS.Comment>();
            try
            {
                TridionTcmUri tcmUri = UtilityHelper.GetTcmUri(itemUri);
                int publicationId = tcmUri.TcmPublicationID;
                int itemId = tcmUri.TcmItemId;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ContentDeliveryService cDSClient = this.CDSClient;
                if (this.CommentModeration.ToLower() == "on")
                {
                    list = (from p in cDSClient.Comments.Expand("User")
                        where (p.ItemPublicationId == publicationId) && (p.ItemId == itemId)
                        where p.Status == 2
                        orderby p.CreationDate descending
                        select p).Take<Coats.Crafts.CDS.Comment>(numberToReturn).ToList<Coats.Crafts.CDS.Comment>();
                }
                else
                {
                    list = (from c in cDSClient.Comments.Expand("User")
                        where (c.ItemPublicationId == publicationId) && (c.ItemId == itemId)
                        select c into p
                        orderby p.CreationDate descending
                        select p).Take<Coats.Crafts.CDS.Comment>(numberToReturn).ToList<Coats.Crafts.CDS.Comment>();
                }
                stopwatch.Stop();
                string str = stopwatch.Elapsed.ToString();
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("GetComments exception - {0}", new object[] { exception });
            }
            return list;
        }

        public IList<Coats.Crafts.CDS.Comment> GetCommentsByUser(string itemUri, string username, int numberToReturn)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.GetCommentsByUser itemUri {0}, numberToReturn {1}, numberToReturn {2}", new object[] { itemUri, numberToReturn, username });
            }
            List<Coats.Crafts.CDS.Comment> list = null;
            try
            {
                TridionTcmUri tcmUri = UtilityHelper.GetTcmUri(itemUri);
                int publicationId = tcmUri.TcmPublicationID;
                int tcmItemId = tcmUri.TcmItemId;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ContentDeliveryService cDSClient = this.CDSClient;
                if (this.CommentModeration.ToLower() == "on")
                {
                    list = (from p in cDSClient.Comments
                        where (p.ItemPublicationId == publicationId) && (p.User.Name == username)
                        where p.Status == 2
                        orderby p.CreationDate descending
                        select p).Take<Coats.Crafts.CDS.Comment>(numberToReturn).ToList<Coats.Crafts.CDS.Comment>();
                }
                else
                {
                    list = (from c in cDSClient.Comments
                        where (c.ItemPublicationId == publicationId) && (c.User.Name == username)
                        select c into p
                        orderby p.CreationDate descending
                        select p).Take<Coats.Crafts.CDS.Comment>(numberToReturn).ToList<Coats.Crafts.CDS.Comment>();
                }
                stopwatch.Stop();
                string str = stopwatch.Elapsed.ToString();
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("GetComments exception - {0}", new object[] { exception });
            }
            return list;
        }

        public string GetRating(string itemUri)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("CommentRepository.GetRating itemUri {0}", new object[] { itemUri });
            }
            string str = "0";
            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                ContentDeliveryService cDSClient = this.CDSClient;
                cDSClient.IgnoreMissingProperties = true;
                str = (from p in cDSClient.ItemStats
                    where ((p.PublicationId == tcm.TcmPublicationID) && (p.Id == tcm.TcmItemId)) && (p.Type == tcm.TcmItemType)
                    select p).First<ItemStat>().AverageRating.ToString();
            }
            catch (DataServiceClientException exception)
            {
                if (exception.Message.Contains("ItemStats entry not found"))
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.ErrorFormat("ItemStats entry not found - {0}", new object[] { exception });
                    }
                    return str;
                }
                this.Logger.ErrorFormat("GetRating exception - {0}", new object[] { exception });
            }
            catch (Exception exception2)
            {
                this.Logger.ErrorFormat("GetRating exception - {0}", new object[] { exception2 });
            }
            return str;
        }

        public void OnSendingRequest(object sender, SendingRequestEventArgs e)
        {
            string str = this.Authenticate("");
            e.RequestHeaders.Add("Authorization", "OAuth " + str);
        }

        public ContentDeliveryService CDSClient
        {
            get
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.DebugFormat("CommentRepository.CDSClient ODataEndPoint {0}", new object[] { this.ODataEndPoint });
                }
                Uri serviceRoot = new Uri(this.ODataEndPoint);
                ContentDeliveryService service = new ContentDeliveryService(serviceRoot);
                service.SendingRequest += new EventHandler<SendingRequestEventArgs>(this.OnSendingRequest);
                return service;
            }
        }

        public string CommentModeration
        {
            get
            {
                return this._settings.CommentModeration;
            }
        }

        public ILogger Logger { get; set; }

        public string ODataEndPoint
        {
            get
            {
                return this._settings.ODataEndPoint;
            }
        }

        public string ODataEndPointSecurityClientId
        {
            get
            {
                return this._settings.ODataEndPointSecurityClientId;
            }
        }

        public string ODataEndPointSecurityClientSecret
        {
            get
            {
                return this._settings.ODataEndPointSecurityClientSecret;
            }
        }

        public string ODataEndPointSecurityEndpoint
        {
            get
            {
                return this._settings.ODataEndPointSecurityEndpoint;
            }
        }

        public int PublicationId
        {
            get
            {
                return this._settings.PublicationId;
            }
        }

        public enum CommentStatus
        {
            SubmittedNeedsModeration,
            FlaggedNeedsModeration,
            PublishedToWebSite,
            RejectedByModerator,
            FlaggedDeletionRequested,
            ModifiedByVistorNeedsModeration
        }
    }
}

