namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts;
    using Coats.Crafts.Attributes;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using Coats.Crafts.SmartTartget.SmartTargetDeploymentWebService;
    using DD4T.ContentModel;
    using DD4T.Mvc.Controllers;
    using Microsoft.Security.Application;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;

    public class CommentsController : TridionControllerBase
    {
        private readonly ICommentRepository _commentrepository;

        public CommentsController(ICommentRepository commentrepository)
        {
            this._commentrepository = commentrepository;
        }

        [HttpPost]
        public ActionResult AddComment(CommentIDs commentform)
        {
            try
            {
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                if (base.ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(commentform.myComment))
                    {
                        return this.redirect("error=1");
                    }
                    if (user.UserName == null)
                    {
                        return this.redirect("error");
                    }
                    string safeHtmlFragment = Sanitizer.GetSafeHtmlFragment(commentform.myComment.Replace("\r\n", " "));
                    int commentCount = this._commentrepository.GetCommentCount(commentform.ComponentID);
                    string data = this._commentrepository.AddComment(safeHtmlFragment, commentform.ComponentID, user.UserName, user.DISPLAYNAME);
                    if (!data.Equals("0"))
                    {
                        try
                        {
                            IComponent componentInfo = this.GetComponentInfo(commentform.ComponentID);
                            CommentEmail model = new CommentEmail {
                                ComponentName = ((TridionItem) componentInfo).Title,
                                ComponentID = commentform.ComponentID,
                                User = user.UserName
                            };
                            EmailUtility utility = new EmailUtility();
                            string toEmailAddress = ConfigurationManager.AppSettings["CommentModerationAdminEmail"];
                            string fromEmailAddress = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                            string emailTemplate = ConfigurationManager.AppSettings["CommentModerationAdminEmailTemplate"];
                            string str6 = utility.SendEmail(model, emailTemplate, fromEmailAddress, toEmailAddress);
                            this.Logger.DebugFormat("CommnetsController : Comments received Email > result {0}", new object[] { str6 });
                        }
                        catch (Exception)
                        {
                            this.Logger.Debug("Unable to send comments received Email to CountryAdmin.");
                        }
                    }
                    if (base.Request.IsAjaxRequest())
                    {
                        return base.Json(data, JsonRequestBehavior.AllowGet);
                    }
                    return this.redirect("commented");
                }
                return this.redirect("error=1");
            }
            catch (Exception exception2)
            {
                this.Logger.DebugFormat("CommentsController > AddComment exception {0}", new object[] { exception2.Message });
                return this.redirect("error");
            }
        }

        public ActionResult AddRating(string ratingOption, string hdnItemId, string hdnFredHopperId)
        {
            try
            {
                if (base.User.Identity.IsAuthenticated)
                {
                    int num;
                    MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                    if (int.TryParse(ratingOption, out num) && ((num > 0) && (num < 6)))
                    {
                        string data = this._commentrepository.AddRating(ratingOption, hdnItemId, user.UserName, user.DISPLAYNAME);
                        string rating = this._commentrepository.GetRating(hdnItemId);
                        this.UpdateFredHopper(hdnFredHopperId, "rating", "text", "en_US", rating);
                        if (base.Request.IsAjaxRequest())
                        {
                            return base.Json(data, JsonRequestBehavior.AllowGet);
                        }
                        return this.redirect("rated");
                    }
                }
                return this.redirect("ratingerror");
            }
            catch (Exception exception)
            {
                this.Logger.DebugFormat("CommentsController > AddRating exception {0}", new object[] { exception.Message });
                return this.redirect("error");
            }
        }

        public ActionResult AggregateRatingTag(string compid, string rating, bool large = false)
        {
            string str = rating;
            if (string.IsNullOrEmpty(rating) || (rating == "0"))
            {
                str = this._commentrepository.GetRating(compid);
            }
            string str2 = ((str == "0") || (str == string.Empty)) ? Helper.GetResource("NotYetRated") : string.Format(Helper.GetResource("RatedOutOf"), str);
            string content = ("<div class=\"rateit large\" data-rateit-value=\"" + str + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"23\" data-rateit-starheight=\"20\" title=\"" + str2 + "\"></div>") + "<div itemprop=\"aggregateRating\" itemscope itemtype=\"http://schema.org/AggregateRating\" style=\"display:none;\"><span itemprop=\"ratingValue\">" + str + "</span></div>";
            return base.Content(content);
        }

        public ActionResult AllCommentsByUser(string compid, string username, int numberToReturn)
        {
            IList<Comment> data = this._commentrepository.GetAllCommentsByUser(compid, username, numberToReturn);
            string debugJson = WebConfiguration.Current.DebugJson;
            if (base.Request.IsAjaxRequest())
            {
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            ((dynamic) base.ViewBag).username = username;
            return base.View(data);
        }

        [ChildActionOnly]
        public ActionResult CommentCount(string compid, string jsondebug)
        {
            int commentCount = this._commentrepository.GetCommentCount(compid);
            string debugJson = WebConfiguration.Current.DebugJson;
            if (base.Request.IsAjaxRequest() || (jsondebug == "1"))
            {
                return base.Json(commentCount, JsonRequestBehavior.AllowGet);
            }
            return base.PartialView(commentCount);
        }

        public ActionResult Comments(string componentTitle, string introText, string compid, string fredHopperId, int numberToReturn, string jsondebug)
        {
            Stopwatch stopwatch = new Stopwatch();
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Start();
            }
            IList<Comment> comments = this._commentrepository.GetComments(compid, numberToReturn);
            CommentsPackage model = new CommentsPackage();
            string userRating = this.GetUserRating(compid);
            model.CommentList = comments;
            model.introText = introText;
            model.title = componentTitle;
            model.IDs.ComponentID = compid;
            model.IDs.FredHopperID = fredHopperId;
            model.IDs.UserRating = userRating;
            CommentIDs.Componenttitle = componentTitle;
            string debugJson = WebConfiguration.Current.DebugJson;
            if (base.Request.IsAjaxRequest() || (jsondebug == "1"))
            {
                return base.Json(comments, JsonRequestBehavior.AllowGet);
            }
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Stop();
                string str3 = stopwatch.Elapsed.ToString();
                this.Logger.DebugFormat("CommentsController.Comments time elapsed:" + str3, new object[0]);
            }
            return base.View(model);
        }

        public ActionResult CommentsByUser(string compid, string username, int numberToReturn)
        {
            IList<Comment> data = this._commentrepository.GetCommentsByUser(compid, username, numberToReturn);
            string debugJson = WebConfiguration.Current.DebugJson;
            if (base.Request.IsAjaxRequest())
            {
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            return base.View(data);
        }

        private IComponent GetComponentInfo(string tcm)
        {
            IContainerAccessor applicationInstance = base.HttpContext.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IComponentFactory>().GetComponent(tcm);
        }

        public string GetUserRating(string compid)
        {
            if (base.User.Identity.IsAuthenticated)
            {
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                return this._commentrepository.CheckRating(compid, user.UserName, false);
            }
            return "";
        }

        [HttpGet]
        public ActionResult Index()
        {
            return base.View();
        }

        public ActionResult Rating(string compid)
        {
            string rating = this._commentrepository.GetRating(compid);
            if (base.Request.IsAjaxRequest())
            {
                return base.Json(rating, JsonRequestBehavior.AllowGet);
            }
            return base.Content(rating);
        }

        public ActionResult RatingTag(string compid, string rating, bool large = false)
        {
            string str = rating;
            string str2 = ((str == "0") || (str == string.Empty)) ? Helper.GetResource("NotYetRated") : string.Format(Helper.GetResource("RatedOutOf"), str);
            if (large)
            {
                return base.Content("<div class=\"rateit large\" data-rateit-value=\"" + str + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"23\" data-rateit-starheight=\"20\" title=\"" + str2 + "\"></div>");
            }
            return base.Content("<div class=\"rateit\" data-rateit-value=\"" + str + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\" data-rateit-starwidth=\"17\" title=\"" + str2 + "\"></div>");
        }

        private ActionResult redirect(string status)
        {
            if ((base.Request.UrlReferrer.ToString().Contains("?error") || base.Request.UrlReferrer.ToString().Contains("#error")) && (status.ToLower() == "error"))
            {
                return this.Redirect(base.Request.UrlReferrer.ToString());
            }
            if ((base.Request.UrlReferrer.ToString().Contains("?commented") || base.Request.UrlReferrer.ToString().Contains("#commented")) && (status.ToLower() == "commented"))
            {
                return this.Redirect(base.Request.UrlReferrer.ToString());
            }
            if (base.Request.UrlReferrer.ToString().Contains("?"))
            {
                return this.Redirect(base.Request.UrlReferrer + "&" + status);
            }
            return this.Redirect(base.Request.UrlReferrer + "?" + status);
        }

        private void SendToFredHopper(string fhUpdateXml, string fredHopperId)
        {
            using (SmartTargetDeploymentClient client = new SmartTargetDeploymentClient())
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(fhUpdateXml);
                client.Open();
                bool flag = client.deploy(Encoding.UTF8.GetBytes(document.OuterXml), fredHopperId);
                client.Close();
            }
        }

        private void UpdateFredHopper(string fredHopperId, string attrId, string attrType, string locale, string value)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<items>");
            builder.AppendFormat("<item identifier=\"{0}\" operation=\"update\">", fredHopperId);
            builder.AppendFormat("<attribute identifier=\"{0}\" type=\"{1}\">", attrId, attrType);
            builder.AppendFormat("<name locale=\"{0}\">{1}</name>", locale, attrId);
            builder.AppendFormat("<value>{0}</value>", value);
            builder.Append("</attribute>");
            builder.Append("</item>");
            builder.Append("</items>");
            this.SendToFredHopper(builder.ToString(), fredHopperId);
        }

        private void UpdateFredHopperComments(string itemId, string fredHopperId, int commentCount)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<items>");
            builder.AppendFormat("<item identifier=\"{0}\" operation=\"update\">", fredHopperId);
            builder.Append("<attribute identifier=\"commentcount\" type=\"int\">");
            builder.Append("<name locale=\"en_US\">commentcount</name>");
            builder.AppendFormat("<value>{0}</value>", commentCount);
            builder.Append("</attribute>");
            IList<Comment> comments = this._commentrepository.GetComments(itemId, 3);
            for (int i = 0; i < comments.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append("<attribute identifier=\"lastcommentdate\" type=\"int\">");
                    builder.Append("<name locale=\"en_US\">lastcommentdate</name>");
                    builder.AppendFormat("<value>{0}</value>", comments[i].LastModifiedDate.ToString("yyMMddHHmm"));
                    builder.Append("</attribute>");
                }
                builder.AppendFormat("<attribute identifier=\"comment{0}\" type=\"text\">", i + 1);
                builder.AppendFormat("<name locale=\"en_US\">comment{0}</name>", i + 1);
                builder.AppendFormat("<value>{0}</value>", comments[i].Content);
                builder.Append("</attribute>");
                builder.AppendFormat("<attribute identifier=\"commentby{0}\" type=\"text\">", i + 1);
                builder.AppendFormat("<name locale=\"en_US\">commentby{0}</name>", i + 1);
                builder.AppendFormat("<value>{0}</value>", HttpUtility.HtmlDecode(comments[i].User.Name));
                builder.Append("</attribute>");
                builder.AppendFormat("<attribute identifier=\"commentdate{0}\" type=\"text\">", i + 1);
                builder.AppendFormat("<name locale=\"en_US\">commentdate{0}</name>", i + 1);
                builder.AppendFormat("<value>{0}</value>", comments[i].LastModifiedDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                builder.Append("</attribute>");
            }
            builder.Append("</item>");
            builder.Append("</items>");
            this.SendToFredHopper(builder.ToString(), fredHopperId);
        }

        public ILogger Logger { get; set; }

        public class CommentIDs
        {
            public static string Componenttitle;

            public string ComponentID { get; set; }

            public string FredHopperID { get; set; }

            [AllowHtml, CustomResourceRequired("CommentRequired")]
            public string myComment { get; set; }

            public string UserRating { get; set; }
        }

        public class CommentsPackage
        {
            private CommentsController.CommentIDs _ids = null;
            public string introText;
            public string title;

            public IList<Comment> CommentList { get; set; }

            public CommentsController.CommentIDs IDs
            {
                get
                {
                    if (this._ids == null)
                    {
                        this._ids = new CommentsController.CommentIDs();
                    }
                    return this._ids;
                }
            }
        }
    }
}

