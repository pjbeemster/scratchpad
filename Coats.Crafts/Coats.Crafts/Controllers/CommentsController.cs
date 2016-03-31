using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using DD4T.Mvc.Controllers;
using DD4T.ContentModel;
using Castle.Windsor;
using DD4T.ContentModel.Factories;

using Castle.Core.Logging;

using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Configuration;

using Coats.Crafts.Resources;
using Coats.Crafts.SmartTartget.SmartTargetDeploymentWebService;
using Coats.Crafts.Resources;

using XSS = Microsoft.Security.Application;
using System.Web;
using Coats.Crafts.Attributes;
//Added By Ajaya for Comment Moderatin
using System.Configuration;
using Coats.Crafts.ControllerHelpers;
namespace Coats.Crafts.Controllers
{
    public class CommentsController : TridionControllerBase
    {
        private readonly ICommentRepository _commentrepository;

        public ILogger Logger { get; set; }

        public CommentsController(ICommentRepository commentrepository)
        {
            _commentrepository = commentrepository;
        }


        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Adds the rating.
        /// </summary>
        /// <param name="ratingOption">The ratingOption.</param>
        /// <param name="hdnItemId">The HDN item id.</param>
        /// <param name="hdnFredHopperId">
        /// The ID of the item to update, usually an amalgamation of the Tridion [component ID]_[presentation ID]
        /// e.g. "tcm_70-17897-16_tcm_70-17920-3211111"
        /// An example of this is in "[...]\Views\TridionComponent\DisplayComments.cshtml"
        /// </param>
        /// <returns></returns>
        public ActionResult AddRating(string ratingOption, string hdnItemId, string hdnFredHopperId)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    //CoatsUserProfile user = ProfileHelper.GetUser();
                    var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;

                    int rating;

                    if (int.TryParse(ratingOption, out rating))
                    {
                        if (rating > 0 && rating < 6)
                        {
                            var rate = _commentrepository.AddRating(ratingOption, hdnItemId, user.UserName, user.DISPLAYNAME);

                            string curRating = _commentrepository.GetRating(hdnItemId);

                            // ASH - Update FredHopper... (if you couldn't guess by the method name!!!)
                            UpdateFredHopper(hdnFredHopperId, "rating", "text", "en_US", curRating);

                            if (Request.IsAjaxRequest())
                            {
                                return Json(rate, JsonRequestBehavior.AllowGet);
                            }
                            return redirect("rated");
                        }
                    }
                }

                return redirect("ratingerror");
            }
            catch (Exception ex)
            {
                Logger.DebugFormat("CommentsController > AddRating exception {0}", ex.Message);
                return redirect("error");
            }
        }


        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddComment(CommentIDs commentform)
        {
            try
            {
                //CoatsUserProfile user = ProfileHelper.GetUser();
                var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;

                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(commentform.myComment))
                    {
                        if (user.UserName != null)
                        {
                            // Strip out line breaks then encode
                            // COATSCRAFTS-44 
                            // Changed as HtmlEncode can prodice strings with named entities like &aacute; which are not supported in Xml.
                            String myComment = XSS.Sanitizer.GetSafeHtmlFragment(commentform.myComment.Replace("\r\n", " "));
                            //String myComment = Server.HtmlEncode(commentform.myComment.Replace("\r\n", " "));

                            // ASH - Get the comment count BEFORE we update.
                            //       The reason for this is due to a little bit of latency when updating the UGC.
                            //       Getting the count before enables us to manually increment it when updating FredHopper.
                            int commentCount = _commentrepository.GetCommentCount(commentform.ComponentID);
                            //

                            var comment = _commentrepository.AddComment(myComment, commentform.ComponentID, user.UserName, user.DISPLAYNAME);

                            // ASH - Update FredHopper... (if you couldn't guess by the method name!!!)
                            // if comment != "0" then comment has been successful
                            //will Comment by Ajaya for Comment Moderation & same thing will be done through GUI extension
                            //if (!comment.Equals("0"))
                            //{
                            //    UpdateFredHopperComments(commentform.ComponentID, commentform.FredHopperID, (commentCount + 1));
                            //}
                            if (!comment.Equals("0"))
                            {
                                try
                                {
                                    IComponent compo = GetComponentInfo(commentform.ComponentID);
                                    // Just before we carry on, let's pop out an email to the user to let them know the good news!
                                    
                                    Models.CommentEmail email = new Models.CommentEmail();
                                    //email.ComponentName = compo.Fields["title"].Value;
                                    email.ComponentName = ((DD4T.ContentModel.TridionItem)(compo)).Title;
                                    email.ComponentID = commentform.ComponentID;
                                    email.User = user.UserName;
                                    EmailUtility util = new EmailUtility();
                                    string adminEmail = ConfigurationManager.AppSettings["CommentModerationAdminEmail"]; // Re-using address
                                    string fromEmail = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                                    string commentModerationEmailTemplate = ConfigurationManager.AppSettings["CommentModerationAdminEmailTemplate"];
                                    string result = util.SendEmail(email, commentModerationEmailTemplate, fromEmail, adminEmail);
                                    Logger.DebugFormat("CommnetsController : Comments received Email > result {0}", result);

                                }
                                catch (Exception err)
                                {
                                    Logger.Debug("Unable to send comments received Email to CountryAdmin.");
                                }
                            }
                            if (Request.IsAjaxRequest())
                            {
                                return Json(comment, JsonRequestBehavior.AllowGet);
                            }
                            return redirect("commented");
                        }
                        return redirect("error");
                    }
                    return redirect("error=1");
                }
                return redirect("error=1");
            }
            catch (Exception ex)
            {
                Logger.DebugFormat("CommentsController > AddComment exception {0}", ex.Message);
                return redirect("error");
            }

        }

        /// <summary>
        /// Ratings the specified compid.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <returns></returns>
        public ActionResult Rating(string compid)
        {
            var rating = _commentrepository.GetRating(compid);
            if (Request.IsAjaxRequest())
            {
                return Json(rating, JsonRequestBehavior.AllowGet);
            }
            return Content(rating);
        }

        /// <summary>
        /// Ratings the specified compid.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <returns></returns>
        public string GetUserRating(string compid)
        {
            if (User.Identity.IsAuthenticated)
            {
                //CoatsUserProfile user = ProfileHelper.GetUser();
                var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;

                var rating = _commentrepository.CheckRating(compid, user.UserName, false);

                return rating;
            }
            return "";
            //if (Request.IsAjaxRequest())
            //{
            //    return Json(rating, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Content(rating);
            //}
        }


        /// <summary>
        /// Ratings the specified compid.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <param name="rating"></param>
        /// <param name="large"></param>
        /// <returns></returns>
        public ActionResult RatingTag(string compid, string rating, bool large = false)
        {
            var ratingValue = rating;

            string title = ratingValue == "0" || ratingValue == string.Empty ? Helper.GetResource("NotYetRated") : String.Format(Helper.GetResource("RatedOutOf"), ratingValue);

            if (large)
            {
                return Content("<div class=\"rateit large\" data-rateit-value=\"" + ratingValue + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"23\" data-rateit-starheight=\"20\" title=\"" + title + "\"></div>");
            }
            return Content("<div class=\"rateit\" data-rateit-value=\"" + ratingValue + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"17\" title=\"" + title + "\"></div>");
        }

        public ActionResult AggregateRatingTag(string compid, string rating, bool large = false)
        {
            var ratingValue = rating;

            if (string.IsNullOrEmpty(rating) || rating == "0")
            {
                ratingValue = _commentrepository.GetRating(compid);
            }

            string title = ratingValue == "0" || ratingValue == string.Empty ? Helper.GetResource("NotYetRated") : String.Format(Helper.GetResource("RatedOutOf"), ratingValue);

            string content = "<div class=\"rateit large\" data-rateit-value=\"" + ratingValue +
                             "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"23\" data-rateit-starheight=\"20\" title=\"" +
                             title + "\"></div>";
            content +=
                "<div itemprop=\"aggregateRating\" itemscope itemtype=\"http://schema.org/AggregateRating\" style=\"display:none;\"><span itemprop=\"ratingValue\">" + ratingValue + "</span></div>";

            return Content(content);
        }

        /// <summary>
        /// Comments the specified compid.
        /// </summary>
        /// <param name="introText" />
        /// <param name="compid">The compid.</param>
        /// <param name="fredHopperId"></param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <param name="jsondebug">The jsondebug.</param>
        /// <param name="componentTitle"></param>
        /// <returns></returns>
        public ActionResult Comments(string componentTitle, string introText, string compid, string fredHopperId, int numberToReturn, string jsondebug)
        {
            Stopwatch sw = new Stopwatch();

            if (Logger.IsDebugEnabled)
            {
                sw.Start();
            }
            
            var comments = _commentrepository.GetComments(compid, numberToReturn);

            var commentsPackage = new CommentsPackage();

            string rating = GetUserRating(compid);

            commentsPackage.CommentList = comments;
            commentsPackage.introText = introText;
            commentsPackage.title = componentTitle;
            commentsPackage.IDs.ComponentID = compid;
            commentsPackage.IDs.FredHopperID = fredHopperId;
            commentsPackage.IDs.UserRating = rating;

            CommentIDs.Componenttitle = componentTitle;
            string json = WebConfiguration.Current.DebugJson;

            if (Request.IsAjaxRequest() || jsondebug == "1")
            {
                return Json(comments, JsonRequestBehavior.AllowGet);
            }

            if (Logger.IsDebugEnabled)
            {
                sw.Stop();
                string timer = sw.Elapsed.ToString();
                Logger.DebugFormat("CommentsController.Comments time elapsed:" + timer);
            }

            return View(commentsPackage);
        }


        /// <summary>
        /// Commentses the specified compid.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <param name="jsondebug">The jsondebug.</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CommentCount(string compid, string jsondebug)
        {
            var comments = _commentrepository.GetCommentCount(compid);

            string json = WebConfiguration.Current.DebugJson;

            if (Request.IsAjaxRequest() || jsondebug == "1")
            {
                return Json(comments, JsonRequestBehavior.AllowGet);
            }
            return PartialView(comments);
        }


        /// <summary>
        /// Commentses the by user.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <param name="username">The username.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <returns></returns>
        public ActionResult CommentsByUser(string compid, string username, int numberToReturn)
        {
            var comments = _commentrepository.GetCommentsByUser(compid, username, numberToReturn);

            string json = WebConfiguration.Current.DebugJson;

            if (Request.IsAjaxRequest())
            {
                return Json(comments, JsonRequestBehavior.AllowGet);
            }
            return View(comments);
        }


        /// <summary>
        /// Commentses the by user.
        /// </summary>
        /// <param name="compid">The compid.</param>
        /// <param name="username">The username.</param>
        /// <param name="numberToReturn">The number to return.</param>
        /// <returns></returns>
        public ActionResult AllCommentsByUser(string compid, string username, int numberToReturn)
        {
            var comments = _commentrepository.GetAllCommentsByUser(compid, username, numberToReturn);

            string json = WebConfiguration.Current.DebugJson;

            if (Request.IsAjaxRequest())
            {
                return Json(comments, JsonRequestBehavior.AllowGet);
            }
            ViewBag.username = username;
            return View(comments);
        }

        public class CommentIDs
        {
            public static string Componenttitle;
            public string ComponentID { get; set; }
            public string FredHopperID { get; set; }
            [CustomResourceRequired("CommentRequired")]
            [AllowHtml]
            public string myComment { get; set; }
            public string UserRating { get; set; }
        }

        public class CommentsPackage
        {
            private CommentIDs _ids = null;
            public string title;
            public string introText;
            public IList<CDS.Comment> CommentList { get; set; }
            public CommentIDs IDs
            {
                get
                {
                    if (_ids == null)
                    {
                        _ids = new CommentIDs();
                    }
                    return _ids;
                }
            }
        }


        /// <summary>
        /// Sends an individual update to FredHopper
        /// </summary>
        /// <param name="fredHopperId">
        /// The ID of the item to update, usually an amalgamation of the Tridion [component ID]_[presentation ID]
        /// e.g. "tcm_70-17897-16_tcm_70-17920-3211111"
        /// </param>
        /// <param name="attrId">
        /// The attribute identifier attribute, e.g. "commentcount"
        /// </param>
        /// <param name="attrType">
        /// The attribute type attribute, e.g. "int", "text", etc.
        /// </param>
        /// <param name="locale">
        /// The name locale attribute, e.g. "en_US"
        /// </param>
        /// <param name="value">
        /// The actual value to be set
        /// </param>
        private void UpdateFredHopper(string fredHopperId, string attrId, string attrType, string locale, string value)
        {
            // ----------------------------------------------------------------------------
            // ASH:
            //      Just dropping this code snippet in here because something similar will be
            //      fired off to FredHopper at the same time as a new comment is made.
            // ----------------------------------------------------------------------------
            // ----------------------------------------------------------------------------
            // Working WCF hijack for updating FredHopper.
            // ----------------------------------------------------------------------------
            // Example update XML snippet
            //<items>
            //  <item identifier="tcm_70-17897-16_tcm_70-17920-3211111" operation="update">
            //    <attribute identifier="commentcount" type="int">
            //      <name locale="en_US">commentcount</name>
            //      <value>427</value>
            //    </attribute>
            //  </item>
            //</items>
            // ----------------------------------------------------------------------------

            // Component_Presentation
            StringBuilder fhUpdateXml = new StringBuilder();
            fhUpdateXml.Append("<items>");
            fhUpdateXml.AppendFormat("<item identifier=\"{0}\" operation=\"update\">", fredHopperId);
            fhUpdateXml.AppendFormat("<attribute identifier=\"{0}\" type=\"{1}\">", attrId, attrType);
            fhUpdateXml.AppendFormat("<name locale=\"{0}\">{1}</name>", locale, attrId);
            fhUpdateXml.AppendFormat("<value>{0}</value>", value);
            fhUpdateXml.Append("</attribute>");
            fhUpdateXml.Append("</item>");
            fhUpdateXml.Append("</items>");
            SendToFredHopper(fhUpdateXml.ToString(), fredHopperId);
            // ----------------------------------------------------------------------------

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fredHopperId"></param>
        /// <param name="commentCount"></param>
        private void UpdateFredHopperComments(string itemId, string fredHopperId, int commentCount)
        {
            // Example update XML snippet
            //<items>
            //  <item identifier="tcm_70-17897-16_tcm_70-17920-3211111" operation="update">
            //    <attribute identifier="commentcount" type="int">
            //      <name locale="en_US">commentcount</name>
            //      <value>427</value>
            //    </attribute>
            //    <attribute identifier="lastcommentdate" type="int">
            //      <name locale="en_US">lastcommentdate</name>
            //      <!-- in the format "yyMMddHHmm" -->
            //      <value>1304191030</value>
            //    </attribute>
            //    <attribute identifier="comment1" type="text">
            //      <name locale="en_US">comment1</name>
            //      <value>Nice stitch work!</value>
            //    </attribute>
            //    <attribute identifier="commentby1" type="text">
            //      <name locale="en_US">commentby1</name>
            //      <value>Nora Bone</value>
            //    </attribute>
            //    <attribute identifier="commentcount" type="int">
            //      <name locale="en_US">commentcount</name>
            //      <value>427</value>
            //    </attribute>
            //  </item>
            //</items>
            // ----------------------------------------------------------------------------
            StringBuilder fhUpdateXml = new StringBuilder();
            fhUpdateXml.Append("<items>");
            fhUpdateXml.AppendFormat("<item identifier=\"{0}\" operation=\"update\">", fredHopperId);
            // Add Comment Count attribute -->
            fhUpdateXml.Append("<attribute identifier=\"commentcount\" type=\"int\">");
            fhUpdateXml.Append("<name locale=\"en_US\">commentcount</name>");
            //fhUpdateXml.AppendFormat("<value>{0}</value>", _commentrepository.GetCommentCount(itemId));
            fhUpdateXml.AppendFormat("<value>{0}</value>", commentCount);
            fhUpdateXml.Append("</attribute>");

            // Loop through the comments
            var comments = _commentrepository.GetComments(itemId, 3);
            for (int i = 0; i < comments.Count; i++)
            {
                if (i == 0)
                {
                    // Add Last Comment Date attribute -->
                    // xxx A string in the format "yyyyMMddHHmm"
                    // String now in the format "yyMMddHHmm" because the full year was blowing the int boundary.
                    // The other dates lower down are generally stored as text, so no need to change them.
                    fhUpdateXml.Append("<attribute identifier=\"lastcommentdate\" type=\"int\">");
                    fhUpdateXml.Append("<name locale=\"en_US\">lastcommentdate</name>");
                    //fhUpdateXml.AppendFormat("<value>{0}</value>", comments[i].LastModifiedDate.ToString("yyyyMMddHHmm"));
                    fhUpdateXml.AppendFormat("<value>{0}</value>", comments[i].LastModifiedDate.ToString("yyMMddHHmm"));
                    fhUpdateXml.Append("</attribute>");
                }

                // Add Comment n attribute -->
                fhUpdateXml.AppendFormat("<attribute identifier=\"comment{0}\" type=\"text\">", i + 1);
                fhUpdateXml.AppendFormat("<name locale=\"en_US\">comment{0}</name>", i + 1);
                fhUpdateXml.AppendFormat("<value>{0}</value>", comments[i].Content);
                fhUpdateXml.Append("</attribute>");
                // Add Comment By n attribute -->
                fhUpdateXml.AppendFormat("<attribute identifier=\"commentby{0}\" type=\"text\">", i + 1);
                fhUpdateXml.AppendFormat("<name locale=\"en_US\">commentby{0}</name>", i + 1);
                fhUpdateXml.AppendFormat("<value>{0}</value>", HttpUtility.HtmlDecode(comments[i].User.Name));
                fhUpdateXml.Append("</attribute>");
                // Add Comment Date n attribute -->
                fhUpdateXml.AppendFormat("<attribute identifier=\"commentdate{0}\" type=\"text\">", i + 1);
                fhUpdateXml.AppendFormat("<name locale=\"en_US\">commentdate{0}</name>", i + 1);
                fhUpdateXml.AppendFormat("<value>{0}</value>", comments[i].LastModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                fhUpdateXml.Append("</attribute>");
            }
            fhUpdateXml.Append("</item>");
            fhUpdateXml.Append("</items>");

            SendToFredHopper(fhUpdateXml.ToString(), fredHopperId);
        }

        private void SendToFredHopper(string fhUpdateXml, string fredHopperId)
        {
            using (var client = new SmartTargetDeploymentClient())
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(fhUpdateXml);
                client.Open();
                bool status = client.deploy(Encoding.UTF8.GetBytes(xml.OuterXml), fredHopperId);
                client.Close();
            }
        }

        private ActionResult redirect(string status)
        {
            if ((Request.UrlReferrer.ToString().Contains("?error") || Request.UrlReferrer.ToString().Contains("#error")) && status.ToLower() == "error")
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            if ((Request.UrlReferrer.ToString().Contains("?commented") || Request.UrlReferrer.ToString().Contains("#commented")) && status.ToLower() == "commented")
            {
              
                return Redirect(Request.UrlReferrer.ToString());
            }

            if (Request.UrlReferrer.ToString().Contains("?"))
            {
                return Redirect(Request.UrlReferrer + "&" + status);
            }

            return Redirect(Request.UrlReferrer + "?" + status);
        }

        private IComponent GetComponentInfo(string tcm)
        {
            var accessor = HttpContext.ApplicationInstance as IContainerAccessor;

            var factory = accessor.Container.Resolve<IComponentFactory>();

            IComponent c = factory.GetComponent(tcm);
            return c;
        }
    }
}
