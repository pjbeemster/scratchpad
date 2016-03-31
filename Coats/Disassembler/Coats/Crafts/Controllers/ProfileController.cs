namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Data;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Interfaces;
    using Coats.Crafts.Models;
    using Coats.Crafts.NewsletterAPI;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using Coats.Crafts.Utils;
    using Coats.IndustrialPortal.Providers;
    using com.fredhopper.lang.query.location;
    using DD4T.ContentModel;
    using java.util;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class ProfileController : Controller
    {
        private readonly string _craftlistidentifier;
        private readonly string _emailnewsletteridentifier;
        private readonly IKeywordRepository _keywordrepository;
        private string _locationLat;
        private string _locationLong;
        private static string _newsletterkeyword;
        private readonly string _profilevisibleidentifier;
        private readonly IAppSettings _settings;
        private readonly IStoreLocatorRepository _storelocatorrepository;

        public ProfileController(IStoreLocatorRepository storelocatorrepository, IKeywordRepository keywordrepository, IAppSettings settings, INewsLetterRepository newsletterkeywordrepository)
        {
            this._storelocatorrepository = storelocatorrepository;
            this._keywordrepository = keywordrepository;
            this._settings = settings;
            this._craftlistidentifier = string.Format(this._settings.CraftListCategory, this.PublicationId);
            this._emailnewsletteridentifier = string.Format(this._settings.EmailNewsletterListCategory, this.PublicationId);
            this._profilevisibleidentifier = string.Format(this._settings.ProfileVisibleListCategory, this.PublicationId);
        }

        public Dictionary<string, string> CheckPostcode(string postcode)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(postcode))
            {
                GoogleMapsMarker marker = this._storelocatorrepository.GetMarkerForPostcode(postcode, "")[0];
                if (marker != null)
                {
                    this._locationLat = marker.lat.ToString();
                    dictionary.Add("lat", this._locationLat);
                    this._locationLong = marker.lng.ToString();
                    dictionary.Add("long", this._locationLong);
                }
            }
            return dictionary;
        }

        private ActionResult DefaultEmailNewsletterView()
        {
            UserProfile profile = new UserProfile();
            EmailNewsletter model = new EmailNewsletter();
            if (base.User.Identity.IsAuthenticated)
            {
                profile = this.GetModel();
                if (profile != null)
                {
                    model.IsLoggedIn = true;
                    model.EmailAddress = profile.CustomerDetails.EmailAddress;
                }
            }
            string identifier = string.Format(this._settings.CraftListCategory, this.PublicationId);
            IList<Coats.Crafts.Models.Keyword> keywordsList = this._keywordrepository.GetKeywordsList(identifier);
            Location location = null;
            if (!model.IsLoggedIn)
            {
                try
                {
                    location = new Location(base.HttpContext.Request.QueryString["fh_params"].Split(new char[] { '&' }).SingleOrDefault<string>(f => f.StartsWith("fh_location=")).Replace("fh_location=", ""));
                }
                catch (Exception)
                {
                    location = null;
                }
            }
            foreach (Coats.Crafts.Models.Keyword keyword in keywordsList)
            {
                Func<SelectListItem, bool> predicate = null;
                SelectListItem sli = new SelectListItem {
                    Text = keyword.Description,
                    Value = keyword.Id,
                    Selected = true
                };
                if (model.IsLoggedIn)
                {
                    if (profile != null)
                    {
                        if (predicate == null)
                        {
                            predicate = c => c.Value == sli.Value;
                        }
                        SelectListItem item = profile.CraftTypeList.SingleOrDefault<SelectListItem>(predicate);
                        sli.Selected = (item != null) && item.Selected;
                    }
                }
                else if (location != null)
                {
                    try
                    {
                        List list2 = location.getCriteria("techniques");
                        if (list2.size() > 0)
                        {
                            sli.Selected = list2.ToString().Contains(string.Format("techniques{0}{1}", FacetedContentHelper.NestedLevelDelimiter, sli.Text.ToLower().Replace(" ", "_")));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                model.Techniques.Add(sli);
                model.Techniques.Count<SelectListItem>(t => t.Selected);
            }
            return base.View("~/Views/Partials/EmailNewsletter.cshtml", model);
        }

        [AcceptVerbs(HttpVerbs.Head | HttpVerbs.Get)]
        public ActionResult EmailNewsletterSettings()
        {
            this.Logger.Debug("GET request for EmailNewsletterSettings triggered");
            return this.DefaultEmailNewsletterView();
        }

        [HttpPost]
        public ActionResult EmailNewsletterSettings(EmailNewsletter emailNewsLetter, string EmailNewsletterSubmit, string redirectUrl)
        {
            bool flag = base.Request.IsAjaxRequest();
            bool flag2 = false;
            bool flag3 = false;
            string str = string.Empty;
            string resource = Helper.GetResource("ProfileNotLoggedIn");
            this.Logger.Debug("POST request for EmailNewsletterSettings triggered");
            this.Logger.DebugFormat("emailNewsLetter {0} & EmailNewsletterSubmit {1}", new object[] { emailNewsLetter == null, EmailNewsletterSubmit });
            if (base.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(EmailNewsletterSubmit))
            {
                Action<SelectListItem> action = null;
                CoatsUserProfile coatsUserProfile = CoatsUserProfile.GetProfile(base.HttpContext.User.Identity.Name);
                this.Logger.DebugFormat("coatsUserProfile? {0}", new object[] { coatsUserProfile == null });
                if (coatsUserProfile != null)
                {
                    if (emailNewsLetter != null)
                    {
                        if (action == null)
                        {
                            action = delegate (SelectListItem t) {
                                Func<KeyValuePair<string, string>, bool> predicate = null;
                                if (!t.Selected)
                                {
                                    if (predicate == null)
                                    {
                                        predicate = k => k.Key == t.Value;
                                    }
                                    foreach (KeyValuePair<string, string> pair in coatsUserProfile.Keywords.Where<KeyValuePair<string, string>>(predicate).ToList<KeyValuePair<string, string>>())
                                    {
                                        this.Logger.DebugFormat("Removing {0} {1}", new object[] { pair.Key, pair.Value });
                                        coatsUserProfile.Keywords.Remove(pair.Key);
                                    }
                                }
                                if (!(!t.Selected || coatsUserProfile.Keywords.ContainsKey(t.Value)))
                                {
                                    this.Logger.DebugFormat("Adding {0}", new object[] { t.Value });
                                    coatsUserProfile.Keywords.Add(t.Value, "tcm");
                                }
                            };
                        }
                        emailNewsLetter.Techniques.ForEach(action);
                    }
                    coatsUserProfile.Save();
                }
                flag2 = true;
                resource = Helper.GetResource("PreferencesUpdated");
                redirectUrl = !string.IsNullOrEmpty(redirectUrl) ? redirectUrl : this._settings.EmailNewsLetterConfirmation;
                this.Logger.DebugFormat("Redirecting to {0}", new object[] { redirectUrl });
                if (!flag)
                {
                    return this.Redirect(redirectUrl);
                }
            }
            if (base.Request.IsAjaxRequest())
            {
                return base.Json(new { 
                    success = flag2,
                    allowRedirect = flag3,
                    redirect = str,
                    message = resource
                });
            }
            return this.DefaultEmailNewsletterView();
        }

        private ViewResult FindPostcode(UserProfile model)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(model.AddressDetails.Postcode))
            {
                float num;
                float num2;
                string s = base.Request["lat"];
                string str2 = base.Request["long"];
                bool flag = float.TryParse(s, out num2);
                bool flag2 = float.TryParse(str2, out num);
                if (flag && flag2)
                {
                    model.CustomerDetails.Lat = s;
                    model.CustomerDetails.Long = str2;
                }
                else
                {
                    dictionary = this.CheckPostcode(model.AddressDetails.Postcode);
                    if (dictionary.Count > 0)
                    {
                        model.CustomerDetails.Lat = dictionary["lat"];
                        model.CustomerDetails.Long = dictionary["long"];
                    }
                    else
                    {
                        model.RegistrationStatus = "postcode invalid";
                        base.ModelState.AddModelError(string.Empty, Helper.GetResource("PostcodeError"));
                    }
                }
            }
            return base.View(model);
        }

        private static List<SelectListItem> GetKeywordsSelectList(UserProfile model, IEnumerable<Coats.Crafts.Models.Keyword> keywords)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            List<int> list2 = new List<int>();
            TridionTcmUri tcmUri = new TridionTcmUri();
            foreach (KeyValuePair<string, string> pair in model.Keywords)
            {
                tcmUri = UtilityHelper.GetTcmUri(pair.Key);
                list2.Add(tcmUri.TcmItemId);
            }
            TridionTcmUri uri2 = new TridionTcmUri();
            foreach (Coats.Crafts.Models.Keyword keyword in keywords)
            {
                uri2 = UtilityHelper.GetTcmUri(keyword.Id);
                list.Add(list2.Contains(uri2.TcmItemId) ? new SelectListItem() : new SelectListItem());
            }
            return (from x in list
                orderby x.Text
                select x).ToList<SelectListItem>();
        }

        private static List<TridionTcmUri> GetKeywordsSelectListItems(string selectedKeywords)
        {
            List<TridionTcmUri> list = new List<TridionTcmUri>();
            if (selectedKeywords != null)
            {
                string[] strArray = selectedKeywords.Split(new char[] { ',' });
                TridionTcmUri item = new TridionTcmUri();
                foreach (string str in strArray)
                {
                    item = UtilityHelper.GetTcmUri(str);
                    list.Add(item);
                }
            }
            return list;
        }

        private UserProfile GetModel()
        {
            UserProfile userProfile = null;
            MembershipUser user = Membership.GetUser();
            if (user != null)
            {
                CoatsUserProfile ctsUserProfile = (ConfigurationManager.AppSettings["SiteIdentifier"] != null) ? CoatsUserProfile.GetProfile(user.Email, ConfigurationManager.AppSettings["SiteIdentifier"]) : CoatsUserProfile.GetProfile(user.Email);
                userProfile = this.MapUserProfile(ctsUserProfile, user.Email);
                this.GetModelData(userProfile);
            }
            return userProfile;
        }

        private void GetModelData(UserProfile userProfile)
        {
            userProfile.CraftTypeList = GetKeywordsSelectList(userProfile, this._keywordrepository.GetKeywordsList(this._craftlistidentifier));
            userProfile.EmailNewsletter = GetKeywordsSelectList(userProfile, this._keywordrepository.GetKeywordsList(this._emailnewsletteridentifier));
            userProfile.ProfileVisible = GetKeywordsSelectList(userProfile, this._keywordrepository.GetKeywordsList(this._profilevisibleidentifier));
        }

        [Authorize]
        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            UserProfile model = this.GetModel();
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["title"].Value;
            }
            catch (Exception)
            {
            }
            if (model == null)
            {
                base.Response.Redirect(base.Url.Content(WebConfiguration.Current.AccessDenied.AddApplicationRoot()));
            }
            if (model != null)
            {
                if (base.Request.Url != null)
                {
                    model.returnUrl = base.Request.Url.ToString();
                }
                return base.View(model);
            }
            return null;
        }

        [HttpPost, ValidateInput(true)]
        public ActionResult Index(ComponentPresentation componentPresentation, UserProfile model)
        {
            this.Logger.DebugFormat("Called Update Profile >>>> ", new object[0]);
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["title"].Value;
            }
            catch (Exception)
            {
            }
            string email = string.Empty;
            if (base.User.Identity.IsAuthenticated)
            {
                email = base.User.Identity.Name;
            }
            bool flag = this.IsEditModelValid(model);
            if (flag)
            {
                try
                {
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                    {
                        flag = false;
                        this.Logger.DebugFormat("Update Profile mUser null", new object[0]);
                    }
                    if (model.AddressDetails.Postcode != null)
                    {
                        this.FindPostcode(model);
                    }
                    if (model.RegistrationStatus == "postcode invalid")
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        this.Logger.DebugFormat("User profile start  >>>>", new object[0]);
                        CoatsUserProfile userProfile = CoatsUserProfile.GetProfile(user.Email);
                        if (userProfile != null)
                        {
                            string selectedKeywords = base.Request["CraftType"];
                            string str3 = base.Request["email-newsletter"];
                            string str4 = base.Request["visibile-profile"];
                            PublicasterServiceRequest request = new PublicasterServiceRequest();
                            string newsletter = "No";
                            if (str3.Trim() == WebConfiguration.Current.EmailNewsletterYes.Trim())
                            {
                                newsletter = "yes";
                            }
                            if (((selectedKeywords != null) || (str3 != null)) || (str4 != null))
                            {
                                List<TridionTcmUri> keywordsSelectListItems = GetKeywordsSelectListItems(selectedKeywords);
                                List<TridionTcmUri> second = GetKeywordsSelectListItems(str3);
                                List<TridionTcmUri> list3 = GetKeywordsSelectListItems(str4);
                                IEnumerable<TridionTcmUri> enumerable = keywordsSelectListItems.Union<TridionTcmUri>(second).Union<TridionTcmUri>(list3);
                                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                foreach (TridionTcmUri uri in enumerable)
                                {
                                    dictionary.Add(uri.TcmId, "tcm");
                                }
                                userProfile.Keywords = dictionary;
                                model.Keywords = dictionary;
                            }
                            if (!string.IsNullOrEmpty(model.NewPassword))
                            {
                                if (model.NewPassword == model.VerifyNewPassword)
                                {
                                    if (!string.IsNullOrEmpty(model.CurrentPassword))
                                    {
                                        CoatsUserProfile profile = CoatsUserProfile.GetProfile(email);
                                        if (model.CurrentPassword == profile.PASSWORD)
                                        {
                                            model.CustomerDetails.Password = model.NewPassword;
                                        }
                                        else
                                        {
                                            flag = false;
                                            base.ModelState.AddModelError("CurrentPasswordIncorrect", Helper.GetResource("CurrentPasswordIncorrect"));
                                        }
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    base.ModelState.AddModelError("NewPasswordsNotMatch", Helper.GetResource("NewPasswordsNotMatch"));
                                }
                            }
                            if (flag)
                            {
                                this.MapUserProfileForUpdate(model, userProfile);
                                userProfile.Save();
                                if ((str3 != null) && WebConfiguration.Current.CheckEmailNewsletterOption)
                                {
                                    if (str3.Trim() == WebConfiguration.Current.EmailNewsletterYes.Trim())
                                    {
                                        EmailUtility utility = new EmailUtility();
                                        string fromEmailAddress = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                                        string emailTemplate = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];
                                        string registerThankYou = this._settings.RegisterThankYou;
                                        string str9 = HttpUtility.UrlEncode(General.Encrypt(userProfile.MAIL.Trim()));
                                        registerThankYou = registerThankYou + "?UserEmail=" + str9;
                                        model.CustomerDetails.SiteUrl = registerThankYou;
                                        model.CustomerDetails.DisplayName = userProfile.DISPLAYNAME;
                                        model.CustomerDetails.EmailAddress = userProfile.MAIL;
                                        string str10 = utility.SendEmail(model.CustomerDetails, emailTemplate, fromEmailAddress, userProfile.MAIL);
                                        this.Logger.DebugFormat("ProfileController : Register Confirmation Email > result {0}", new object[] { str10 });
                                        request.createJsonPublicasterRequest(userProfile.MAIL, userProfile.NAME, userProfile.SURNAME, selectedKeywords, userProfile.DISPLAYNAME, newsletter);
                                    }
                                    else
                                    {
                                        request.UnSubscripePublicaster(userProfile.MAIL);
                                    }
                                }
                                CoatsUserProfile profile3 = CoatsUserProfile.GetProfile(userProfile.MAIL);
                                CookieHelper.WriteFormsCookie(userProfile.MAIL, userProfile.DISPLAYNAME, userProfile.NAME, userProfile.SURNAME, userProfile.LONG, userProfile.LAT, "");
                                ((dynamic) base.ViewBag).profileStatus = "saved";
                                if (profile3 == null)
                                {
                                    flag = false;
                                    base.ModelState.AddModelError(string.Empty, "");
                                    this.Logger.DebugFormat("Update Profile newUser null", new object[0]);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.Logger.DebugFormat("Update Profile exception {0} {1}", new object[] { exception.Message, exception.InnerException });
                    flag = false;
                }
            }
            if (base.ModelState.ContainsKey("CustomerDetails.DisplayName"))
            {
                base.ModelState["CustomerDetails.DisplayName"].Errors.Clear();
            }
            if (base.ModelState.ContainsKey("CustomerDetails.EmailAddress"))
            {
                base.ModelState["CustomerDetails.EmailAddress"].Errors.Clear();
            }
            if (!flag)
            {
                UserProfile profile4 = this.GetModel();
                base.ModelState.AddModelError(string.Empty, "");
                var typeArray = (from x in base.ModelState
                    where x.Value.Errors.Count > 0
                    select new { 
                        Key = x.Key,
                        Errors = x.Value.Errors
                    }).ToArray();
                foreach (var type in typeArray)
                {
                    this.Logger.DebugFormat("Update Profile error {0}", new object[] { type });
                }
                this.GetModelData(profile4);
                base.Session["feedback"] = Helper.GetResource("ProfileError");
                ((dynamic) base.ViewBag).profileStatus = "error";
                return base.View(profile4);
            }
            UserProfile profile5 = this.GetModel();
            base.Session["feedback"] = Helper.GetResource("ProfileChangesSaved");
            return base.View(profile5);
        }

        private bool IsEditModelValid(IUserProfile model)
        {
            bool flag = true;
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.CustomerDetails.FirstName))
                    {
                        flag = false;
                    }
                    if (string.IsNullOrEmpty(model.CustomerDetails.LastName))
                    {
                        flag = false;
                    }
                    return flag;
                }
                flag = false;
            }
            catch (Exception exception)
            {
                this.Logger.DebugFormat("IsEditModelValid exception >>>> ", new object[] { exception });
            }
            return flag;
        }

        private UserProfile MapUserProfile(CoatsUserProfile ctsUserProfile, string userEmail)
        {
            return new UserProfile { 
                CustomerDetails = { 
                    FirstName = ctsUserProfile.NAME,
                    LastName = ctsUserProfile.SURNAME,
                    TelephoneNumber = ctsUserProfile.TELEPHONE,
                    EmailAddress = userEmail,
                    Password = ctsUserProfile.PASSWORD,
                    VerifyPassword = ctsUserProfile.PASSWORD,
                    DisplayName = ctsUserProfile.DISPLAYNAME,
                    About = ctsUserProfile.ABOUT,
                    Long = ctsUserProfile.LONG,
                    Lat = ctsUserProfile.LAT
                },
                AddressDetails = { 
                    BuildingNameNumber = ctsUserProfile.BUILDING,
                    City = ctsUserProfile.CITY,
                    Postcode = ctsUserProfile.POSTCODE,
                    Street = ctsUserProfile.STREET
                },
                Keywords = ctsUserProfile.Keywords
            };
        }

        private void MapUserProfileForUpdate(UserProfile model, CoatsUserProfile userProfile)
        {
            this.Logger.DebugFormat("MapUserProfileForUpdate", new object[0]);
            try
            {
                userProfile.NAME = model.CustomerDetails.FirstName;
                userProfile.SURNAME = model.CustomerDetails.LastName;
                userProfile.TELEPHONE = model.CustomerDetails.TelephoneNumber;
                userProfile.ABOUT = model.CustomerDetails.About;
                userProfile.LONG = model.CustomerDetails.Long;
                userProfile.LAT = model.CustomerDetails.Lat;
                userProfile.Keywords = model.Keywords;
                userProfile.POSTCODE = model.AddressDetails.Postcode;
                userProfile.PASSWORD = model.CustomerDetails.Password;
            }
            catch (Exception exception)
            {
                this.Logger.DebugFormat("MapUserProfileForUpdate ex {0}", new object[] { exception.Message });
            }
        }

        public ILogger Logger { get; set; }

        public int PublicationId
        {
            get
            {
                return this._settings.PublicationId;
            }
        }
    }
}

