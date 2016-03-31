using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text;
using Castle.Core.Logging;
using Tridion.ContentDelivery.UGC.WebService;
using System.Web.Script.Serialization;
using Coats.Crafts.Configuration;
using Coats.Crafts.Repositories.Interfaces;
using System.Net;
using System.Collections.Specialized;
using System.Data.Services.Client;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Models;


namespace Coats.Crafts.Repositories.Tridion
{
    public class CommentRepository : ICommentRepository
    {
        private IAppSettings _settings;

        public CommentRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        public ILogger Logger { get; set; }

        public int PublicationId
        {
            get
            {
                return _settings.PublicationId;
            }
        }

        public string CommentModeration
        {
            get
            {
                return _settings.CommentModeration;
            }
        }

        public string ODataEndPoint
        {
            get
            {
                return _settings.ODataEndPoint;
            }
        }

        public string ODataEndPointSecurityClientId
        {
            get
            {
                return _settings.ODataEndPointSecurityClientId;
            }
        }

        public string ODataEndPointSecurityClientSecret
        {
            get
            {
                return _settings.ODataEndPointSecurityClientSecret;
            }
        }

        public string ODataEndPointSecurityEndpoint
        {
            get
            {
                return _settings.ODataEndPointSecurityEndpoint;
            }
        }

        /// <summary>
        /// Gets the CDS client.
        /// </summary>
        /// <value>The CDS client.</value>
        public CDS.ContentDeliveryService CDSClient
        {
            get
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("CommentRepository.CDSClient ODataEndPoint {0}", ODataEndPoint);

                Uri uri = new Uri(ODataEndPoint);
                CDS.ContentDeliveryService cds = new CDS.ContentDeliveryService(uri);
                // we need to attach authentication headers with the request
                cds.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequest);
                return cds;
            }
        }


        /// <summary>
        /// Called when [sending request] from the CDS client to attach security header.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Data.Services.Client.SendingRequestEventArgs"/> instance containing the event data.</param>
        public void OnSendingRequest(object sender, SendingRequestEventArgs e)
        {
            string auth;
            auth = Authenticate();

            // Add an Authorization header that contains an OAuth WRAP access token to the request.
            e.RequestHeaders.Add("Authorization", "OAuth " + auth);
        }


        public enum CommentStatus
        {
            SubmittedNeedsModeration = 0,
            FlaggedNeedsModeration = 1,
            PublishedToWebSite = 2,
            RejectedByModerator = 3,
            FlaggedDeletionRequested = 4,
            ModifiedByVistorNeedsModeration = 5
        }


        /// <summary>
        /// Adds the UGC comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="itemUri">The item URI.</param>
        /// <returns></returns>
        public string AddComment(string comment, string itemUri, string userId, string userName)
        {
            string result = "0";

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.AddComment Comment {0}, itemUri {1}", comment, itemUri);

            try
            {
                WebServiceClient ugcCall = new WebServiceClient();

                string auth = Authenticate();

                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);

                CDS.Comment cdsComment = new CDS.Comment();

                CDS.User cdsUser = new CDS.User();

                cdsUser.Id = userId;
                cdsUser.Name = userName;

                comment = comment.Replace(System.Environment.NewLine, "#nl");

                cdsComment.Content = comment;

                if (CommentModeration.ToLower() == "on")
                {
                    cdsComment.Status = (int)CommentStatus.SubmittedNeedsModeration;
                }
                else
                {
                    cdsComment.Status = (int)CommentStatus.PublishedToWebSite;
                }

                cdsComment.ItemPublicationId = tcm.TcmPublicationID;
                cdsComment.ItemId = tcm.TcmItemId;
                cdsComment.ItemType = tcm.TcmItemType;
                //cdsComment.Id = "0";
                cdsComment.ModeratedDate = DateTime.UtcNow;
                cdsComment.LastModifiedDate = DateTime.UtcNow;
                cdsComment.CreationDate = DateTime.UtcNow;
                cdsComment.Score = 0;
                cdsComment.Moderator = "";
                cdsComment.User = cdsUser;

                JavaScriptSerializer oSerializer = new JavaScriptSerializer();

                result = ugcCall.UploadString("/Comments", "POST", "{d:" + oSerializer.Serialize(cdsComment) + "}");
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("AddComment exception - {0}", ex);
            }

            return result;
        }



        /// <summary>
        /// Adds the UGC rating.
        /// </summary>
        /// <param name="ratingValue">The rating value.</param>
        /// <param name="itemUri">The item URI.</param>
        /// <returns></returns>
        public string AddRating(string ratingValue, string itemUri, string userId, string displayName)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.AddRating ratingValue {0}, itemUri {1}", ratingValue, itemUri);

            string exists = CheckRating(itemUri, userId, true);

            string result = "0";

            if (exists != "rated")
            {
                try
                {
                    WebServiceClient ugcCall = new WebServiceClient();
                    TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);

                    CDS.User cdsUser = new CDS.User();

                    cdsUser.Id = userId;
                    cdsUser.Name = displayName;

                    CDS.Rating cdsRating = new CDS.Rating
                    {
                        CreationDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        ItemPublicationId = tcm.TcmPublicationID,
                        ItemId = tcm.TcmItemId,
                        ItemType = tcm.TcmItemType,
                        RatingValue = ratingValue.ToString(),
                        User = cdsUser,
                        Id = "0"
                    };

                    JavaScriptSerializer oSerializer = new JavaScriptSerializer();

                    result = ugcCall.UploadString("/Ratings", "POST", "{d:" + oSerializer.Serialize(cdsRating) + "}");
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("AddRating exception - {0}", ex);
                }
            }
            return result;
        }



        /// <summary>
        /// Gets the UGC rating.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <returns></returns>
        public string GetRating(string itemUri)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.GetRating itemUri {0}", itemUri);

            string rating = "0";

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                var client = CDSClient;
                client.IgnoreMissingProperties = true;

                var myItemStats = client.ItemStats.Where(p => p.PublicationId == tcm.TcmPublicationID && p.Id == tcm.TcmItemId && p.Type == tcm.TcmItemType);

                CDS.ItemStat stat = myItemStats.First();
                rating = stat.AverageRating.ToString();
            }                
            catch (DataServiceClientException dex)
            {
                // Catch this exception for errors where items do not have stats yet
                // This takes alot of noise out of the log files
                if (dex.Message.Contains("ItemStats entry not found"))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.ErrorFormat("ItemStats entry not found - {0}", dex);
                }
                else
                {
                    Logger.ErrorFormat("GetRating exception - {0}", dex);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetRating exception - {0}", ex);
            }
            return rating;

        }


        /// <summary>
        /// Checks the rating.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public string CheckRating(string itemUri, string user, bool remove)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.CheckRating itemUri {0}, itemUri {1} ", itemUri, user);

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                var client = CDSClient;
                client.IgnoreMissingProperties = true;
                var myRatings = client.Ratings.Expand("User").Where(p => p.ItemPublicationId == tcm.TcmPublicationID && p.ItemId == tcm.TcmItemId && p.User.Id == user);

                // if the user has rated this before then we need to delete previous ratings
                // overrides default functionality of allowing multiple ratings
                if (myRatings.Count() > 0)
                {
                    foreach (CDS.Rating rat in myRatings)
                    {
                        if (remove)
                        {
                            //we're updating so delete old rating
                            DeleteRating(rat.Id);
                        } else {
                            //we're just reading the user's rating so return the value
                            return rat.RatingValue;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetRating exception - {0}", ex);
                return "rated";
            }

            return "";
        }



        /// <summary>
        /// Deletes the rating.
        /// </summary>
        /// <param name="ratingID">The rating ID.</param>
        /// <returns></returns>
        public string DeleteRating(string ratingID)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.DeleteRating ratingId {0}", ratingID);

            WebRequest wr = WebRequest.Create(_settings.ModeratorODataEndPoint + "/Ratings(Id=" + ratingID + ")");
            wr.Headers.Clear();
            string auth = Authenticate("moderator");
            wr.Headers["authorization"] = "OAuth " + auth;
            wr.Method = "DELETE";
            var response = wr.GetResponse();
            return response.ToString();
        }


        /// <summary>
        /// Gets the UGC comment count.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <returns></returns>
        public int GetCommentCount(string itemUri)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.GetCommentCount itemUri {0}", itemUri);

            int commentCount = 0;

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                var client = CDSClient;
                commentCount = client.Comments.Where(p => p.ItemPublicationId == tcm.TcmPublicationID && p.ItemId == tcm.TcmItemId).Take(1000).Count();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetCommentCount exception - {0}", ex);
            }

            return commentCount;
        }


        /// <summary>
        /// Gets the UGC comments.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <returns></returns>
        public IList<CDS.Comment> GetComments(string itemUri, int numberToReturn)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.GetComments itemUri {0}, numberToReturn {1}", itemUri, numberToReturn);

            List<CDS.Comment> comments = new List<CDS.Comment>();

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);

                int publicationId = tcm.TcmPublicationID;
                int itemId = tcm.TcmItemId;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                var client = CDSClient;

                //var clientExpanded = client.Comments.Expand("User");

                if (CommentModeration.ToLower() == "on")
                {
                    // Comment moderation is on so return comments that have a status of PublishedToWebSite
                    comments = (from c in client.Comments.Expand("User").Where(p => p.ItemPublicationId == publicationId && p.ItemId == itemId)
                                where c.Status == (int)CommentStatus.PublishedToWebSite
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }
                else
                {
                    // Comment moderation is off so return all comments regardless of status
                    comments = (from c in client.Comments.Expand("User").Where(p => p.ItemPublicationId == publicationId && p.ItemId == itemId)
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }


                sw.Stop();
                string timer1 = sw.Elapsed.ToString();

            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetComments exception - {0}", ex);
            }

            return comments;
        }

        
        /// <summary>
        /// Gets the UGC comments by user.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public IList<CDS.Comment> GetCommentsByUser(string itemUri, string username, int numberToReturn)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.GetCommentsByUser itemUri {0}, numberToReturn {1}, numberToReturn {2}", itemUri, numberToReturn, username);

            List<CDS.Comment> comments = null;

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                int publicationId = tcm.TcmPublicationID;
                int itemId = tcm.TcmItemId;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                var client = CDSClient;

                if (CommentModeration.ToLower() == "on")
                {
                    // Comment moderation is on so return comments that have a status of PublishedToWebSite
                    comments = (from c in client.Comments.Where(p => p.ItemPublicationId == publicationId && p.User.Name == username)
                                where c.Status == (int)CommentStatus.PublishedToWebSite
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }
                else
                {
                    // Comment moderation is off so return all comments regardless of status
                    comments = (from c in client.Comments.Where(p => p.ItemPublicationId == publicationId && p.User.Name == username)
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }

                sw.Stop();
                string timer1 = sw.Elapsed.ToString();

            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetComments exception - {0}", ex);
            }

            return comments;
        }





        /// <summary>
        /// Gets all comments by user.
        /// </summary>
        /// <param name="itemUri">The item URI.</param>
        /// <param name="username">The username.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <returns></returns>
        public IList<CDS.Comment> GetAllCommentsByUser(string itemUri, string username, int numberToReturn)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.GetAllCommentsByUser numberToReturn {0}, numberToReturn {1}", numberToReturn, username);

            List<CDS.Comment> comments = null;

            try
            {
                TridionTcmUri tcm = UtilityHelper.GetTcmUri(itemUri);
                int publicationId = tcm.TcmPublicationID;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                var client = CDSClient;

                //var clientExpanded = client.Comments.Expand("User");

                if (CommentModeration.ToLower() == "on")
                {
                    // Comment moderation is on so return comments that have a status of PublishedToWebSite
                    comments = (from c in client.Comments.Where(p => p.ItemPublicationId == publicationId && p.User.Name == username)
                                where c.Status == (int)CommentStatus.PublishedToWebSite
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }
                else
                {
                    // Comment moderation is off so return all comments regardless of status
                    comments = (from c in client.Comments.Where(p => p.ItemPublicationId == publicationId && p.User.Id == username)
                                select c).OrderByDescending(p => p.CreationDate).Take(numberToReturn).ToList();
                }

                sw.Stop();
                string timer1 = sw.Elapsed.ToString();

            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("GetComments exception - {0}", ex);
            }

            return comments;
        }


        /// <summary>
        /// Returns the oAuth Token based on 
        /// </summary>
        /// <returns></returns>
        public string Authenticate(string authType = "")
        {
            string Result = string.Empty;
            string auth = string.Empty;

            int compareValue;

            string securityEndPoint = _settings.ODataEndPointSecurityEndpoint;
            string securityClientId = _settings.ODataEndPointSecurityClientId;
            string securityClientSecret = _settings.ODataEndPointSecurityClientSecret;

            if (authType == "moderator")
            {
                securityEndPoint = _settings.ModeratorODataEndPointSecurityEndpoint;
                securityClientId = _settings.ModeratorODataEndPointSecurityClientId;
                securityClientSecret = _settings.ModeratorODataEndPointSecurityClientSecret;
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("CommentRepository.Authenticate ODataEndPointSecurityEndpoint {0}", securityEndPoint);

            try
            {
                DateTime authExpiry = Convert.ToDateTime(HttpContext.Current.Session["oAuthExpire"]);
                compareValue = authExpiry.CompareTo(DateTime.Now);
            }
            catch (ArgumentException)
            {
                compareValue = 0;
            }

            if (compareValue == 0 || compareValue < 0)
            {
                try
                {
                    using (var wb = new WebClient())
                    {
                        var data = new NameValueCollection();

                        data["client_id"] = securityClientId;
                        data["client_secret"] = securityClientSecret;
                       
                        var response = wb.UploadValues(securityEndPoint, "POST", data);
                        Result = Encoding.Default.GetString(response);
                    }

                    var jss = new JavaScriptSerializer();
                    dynamic newdata = jss.Deserialize<dynamic>(Result);
                    auth = newdata["access_token"];

                    auth = HttpUtility.UrlDecode(auth, Encoding.UTF8);

                    //strip out the expiry date and place it in session so we can compare the next time it's used
                    int dateStart = auth.IndexOf("expiresOn=") + 10;
                    int digestStart = auth.IndexOf("&digest");
                    int dateEnd = digestStart - dateStart;


                    long epochDate = Convert.ToInt64(auth.Substring(dateStart, dateEnd));

                    HttpContext.Current.Session["oAuthExpire"] = UtilityHelper.FromUnixTime(epochDate).ToString();
                    HttpContext.Current.Session["oAuthToken"] = auth;
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Authenticate exception - {0}", ex);
                }
            }
            else
            {
                auth = HttpContext.Current.Session["oAuthToken"].ToString();
            }

            return auth;
        }

    }
}