namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Data;
    using Coats.Crafts.Models;
    using Coats.Crafts.NewsletterAPI;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using Coats.Crafts.Utils;
    using Coats.IndustrialPortal.Providers;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class RegistrationController : Controller
    {
        private readonly string _craftlistidentifier;
        private readonly string _emailnewsletteridentifier;
        private readonly IKeywordRepository _keywordrepository;
        private string _locationLat;
        private string _locationLong;
        private readonly string _newsletterkeyword;
        private readonly INewsLetterRepository _newsletterkeywordrepository;
        private readonly string _profilevisibleidentifier;
        private readonly IRegistrationRepository _registrationrepository;
        private readonly IAppSettings _settings;
        private readonly IStoreLocatorRepository _storelocatorrepository;
        private List<string> errorList = new List<string>();

        public RegistrationController(IStoreLocatorRepository storelocatorrepository, IKeywordRepository keywordrepository, IRegistrationRepository registrationrepository, IAppSettings settings)
        {
            this._storelocatorrepository = storelocatorrepository;
            this._keywordrepository = keywordrepository;
            this._registrationrepository = registrationrepository;
            this._settings = settings;
            this._craftlistidentifier = string.Format(this._settings.CraftListCategory, this.PublicationId);
            this._emailnewsletteridentifier = string.Format(this._settings.EmailNewsletterListCategory, this.PublicationId);
            this._profilevisibleidentifier = string.Format(this._settings.ProfileVisibleListCategory, this.PublicationId);
        }

        private ViewResult AddKeywords(RegistrationForm model)
        {
            string selectedKeywords = base.Request["keyword-stash"];
            string str2 = base.Request["CraftType"];
            string str3 = base.Request["email-newsletter"];
            string str4 = base.Request["visibile-profile"];
            if ((((selectedKeywords != null) || (str2 != null)) || (str3 != null)) || (str4 != null))
            {
                List<TridionTcmUri> keywordsSelectListItems = GetKeywordsSelectListItems(selectedKeywords);
                List<TridionTcmUri> second = GetKeywordsSelectListItems(str2);
                List<TridionTcmUri> list3 = GetKeywordsSelectListItems(str3);
                List<TridionTcmUri> list4 = GetKeywordsSelectListItems(str4);
                IEnumerable<TridionTcmUri> enumerable = keywordsSelectListItems.Union<TridionTcmUri>(second).Union<TridionTcmUri>(list3).Union<TridionTcmUri>(list4);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                foreach (TridionTcmUri uri in enumerable)
                {
                    dictionary.Add(uri.TcmId, "tcm");
                }
                model.Keywords = dictionary;
            }
            return base.View(model);
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

        [HttpPost]
        public ActionResult EmailNewsletterSignup(EmailNewsletter emailNewsLetter)
        {
            Registration model = new Registration();
            LoginForm form = new LoginForm();
            RegistrationForm form2 = new RegistrationForm();
            model.RegistrationForm = form2;
            model.LoginForm = form;
            this.GetModelData(model.RegistrationForm);
            model.RegistrationForm.CraftTypeList = emailNewsLetter.Techniques;
            SelectListItem item = model.RegistrationForm.EmailNewsletter.SingleOrDefault<SelectListItem>(e => e.Text == "Yes");
            if (item != null)
            {
                item.Selected = true;
            }
            SelectListItem item2 = model.RegistrationForm.EmailNewsletter.SingleOrDefault<SelectListItem>(e => e.Text == "No");
            if (item2 != null)
            {
                item2.Selected = false;
            }
            model.RegistrationForm.CustomerDetails.EmailAddress = emailNewsLetter.EmailAddress;
            return base.View("Index", model);
        }

        private ViewResult FindPostcode(RegistrationForm model, bool fullForm)
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
                        model.PostCodeMapUrl = string.Format(this._settings.GoogleAPIsMapUrl + (!string.IsNullOrEmpty(this._settings.GoogleMapsAPIKey) ? ("&key=" + this._settings.GoogleMapsAPIKey) : ""), this._locationLat, this._locationLong);
                    }
                    else
                    {
                        base.ModelState.AddModelError(string.Empty, Helper.GetResource("PostcodeError"));
                        this.errorList.Add("Postcode");
                    }
                }
            }
            return base.View(model);
        }

        private string GetClientIP()
        {
            HttpContext current = HttpContext.Current;
            string str = current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(str))
            {
                str = current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return str;
        }

        private string GetCreateUserError(MembershipCreateStatus status)
        {
            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                return Helper.GetResource("EmailAlreadyExists");
            }
            return Helper.GetResource("CreateUserError");
        }

        private static List<Newsletter> GetKeywordsNewsletterList(RegistrationForm model, IEnumerable<Newsletter> newsletter)
        {
            List<Newsletter> list = new List<Newsletter>();
            foreach (Newsletter newsletter2 in newsletter)
            {
                list.Add(newsletter2);
            }
            return (from x in list
                orderby x.Header
                select x).ToList<Newsletter>();
        }

        private static List<SelectListItem> GetKeywordsSelectList(RegistrationForm model, IEnumerable<Coats.Crafts.Models.Keyword> keywords)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Coats.Crafts.Models.Keyword keyword in keywords)
            {
                SelectListItem item = new SelectListItem {
                    Text = keyword.Description,
                    Value = keyword.Id.ToString()
                };
                list.Add(item);
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
                    if (!string.IsNullOrEmpty(str))
                    {
                        item = UtilityHelper.GetTcmUri(str);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private void GetModelData(RegistrationForm registrationform)
        {
            if (registrationform.CraftTypeList == null)
            {
                registrationform.CraftTypeList = GetKeywordsSelectList(registrationform, this._keywordrepository.GetKeywordsList(this._craftlistidentifier));
            }
            if (registrationform.EmailNewsletter == null)
            {
                registrationform.EmailNewsletter = GetKeywordsSelectList(registrationform, this._keywordrepository.GetKeywordsList(this._emailnewsletteridentifier));
            }
            if (registrationform.ProfileVisible == null)
            {
                registrationform.ProfileVisible = GetKeywordsSelectList(registrationform, this._keywordrepository.GetKeywordsList(this._profilevisibleidentifier));
            }
        }

        [ValidateInput(false), HttpPost]
        public ActionResult Index(ComponentPresentation componentPresentation, Registration model, string Stage)
        {
            bool flag = base.Request.IsAjaxRequest();
            bool flag2 = false;
            bool flag3 = false;
            string errorMessage = string.Empty;
            string returnUrl = string.Empty;
            LoginForm loginForm = model.LoginForm;
            RegistrationForm registrationForm = model.RegistrationForm;
            string str3 = !string.IsNullOrWhiteSpace(base.Request["source"]) ? base.Request["source"] : string.Empty;
            if (loginForm != null)
            {
                if (!string.IsNullOrEmpty(loginForm.ReturnUrl))
                {
                    returnUrl = loginForm.ReturnUrl;
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    returnUrl = str3;
                }
                errorMessage = Helper.GetResource("LoginFailed");
                if (this.Login(loginForm))
                {
                    errorMessage = Helper.GetResource("LoginSuccess");
                    flag3 = true;
                    flag2 = true;
                }
                else
                {
                    ((dynamic) base.ViewBag).Stage = "register";
                    base.ModelState.AddModelError("LoginFailed", errorMessage);
                }
                if (flag)
                {
                    return base.Json(new { 
                        success = flag3,
                        allowRedirect = flag2,
                        redirect = returnUrl,
                        message = errorMessage
                    });
                }
            }
            if (registrationForm != null)
            {
                EmailUtility utility;
                string str4;
                string str5;
                string str6;
                Exception exception;
                if (!string.IsNullOrEmpty(registrationForm.returnUrl))
                {
                    returnUrl = registrationForm.returnUrl;
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    returnUrl = str3;
                }
                errorMessage = Helper.GetResource("RegistrationFailed");
                PublicasterServiceRequest request = new PublicasterServiceRequest();
                switch (Stage)
                {
                    case "register":
                        errorMessage = Helper.GetResource("RegistrationFailed");
                        if (!this.IsProfileValid(registrationForm))
                        {
                            ((dynamic) base.ViewBag).Stage = "register";
                            flag3 = false;
                            errorMessage = Helper.GetResource("RegistrationFailed");
                            base.ModelState.AddModelError("UnableToCreateNewUser", Helper.GetResource("UnableToCreateNewUser"));
                            break;
                        }
                        if (this.Register(registrationForm))
                        {
                            ((dynamic) base.ViewBag).Stage = "preferences";
                            flag3 = true;
                            errorMessage = Helper.GetResource("RegisteredSuccessfully");
                            this.Logger.InfoFormat("RegistrationController : User created: {0} - {1}", new object[] { registrationForm.CustomerDetails.DisplayName, registrationForm.CustomerDetails.EmailAddress });
                            try
                            {
                                if (!WebConfiguration.Current.CheckEmailNewsletterOption)
                                {
                                    utility = new EmailUtility();
                                    str4 = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                                    str5 = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];
                                    str6 = utility.SendEmail(registrationForm.CustomerDetails, str5, str4, registrationForm.CustomerDetails.EmailAddress);
                                    this.Logger.DebugFormat("RegistrationController : Register Confirmation Email > result {0}", new object[] { str6 });
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                this.Logger.Debug("Unable to send confirmation email to user.");
                            }
                        }
                        else
                        {
                            ((dynamic) base.ViewBag).Stage = "register";
                            flag3 = false;
                            errorMessage = Helper.GetResource("UnableToCreateNewUser");
                            base.ModelState.AddModelError("UnableToCreateNewUser", Helper.GetResource("UnableToCreateNewUser"));
                        }
                        break;

                    case "preferences":
                        errorMessage = Helper.GetResource("PreferencesFailed");
                        model.RegistrationForm.returnUrl = this._settings.RegisterWelcome;
                        if (this.Preferences(registrationForm))
                        {
                            ((dynamic) base.ViewBag).Stage = "profile";
                            flag3 = true;
                            errorMessage = Helper.GetResource("PreferencesSaved");
                            try
                            {
                                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                                CoatsUserProfile profile = CoatsUserProfile.GetProfile(base.User.Identity.Name);
                                utility = new EmailUtility();
                                str4 = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"];
                                str5 = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];
                                string newsletter = "No";
                                string interests = string.Empty;
                                foreach (KeyValuePair<string, string> pair in registrationForm.Keywords)
                                {
                                    interests = interests + pair.Key + ",";
                                }
                                if (interests.Length > 0)
                                {
                                    interests = interests.Substring(0, interests.LastIndexOf(",") - 1);
                                }
                                KeyValuePair<string, string> pair2 = registrationForm.Keywords.SingleOrDefault<KeyValuePair<string, string>>(e => e.Key == WebConfiguration.Current.EmailNewsletterYes);
                                if (pair2.Key != null)
                                {
                                    newsletter = "yes";
                                }
                                if (WebConfiguration.Current.CheckEmailNewsletterOption && (pair2.Key != null))
                                {
                                    newsletter = "yes";
                                    string str9 = string.Empty;
                                    if (base.Request.Url != null)
                                    {
                                        str9 = base.Request.Url.Scheme + "://" + base.Request.Url.Host;
                                    }
                                    else
                                    {
                                        str9 = ConfigurationManager.AppSettings["SiteUrl"];
                                    }
                                    string registerThankYou = this._settings.RegisterThankYou;
                                    string str11 = HttpUtility.UrlEncode(General.Encrypt(profile.MAIL.Trim()));
                                    str9 = registerThankYou + "?UserEmail=" + str11;
                                    registrationForm.CustomerDetails.SiteUrl = str9;
                                    registrationForm.CustomerDetails.DisplayName = profile.DISPLAYNAME;
                                    registrationForm.CustomerDetails.LastName = profile.SURNAME;
                                    registrationForm.CustomerDetails.FirstName = profile.NAME;
                                    registrationForm.CustomerDetails.EmailAddress = profile.MAIL;
                                    str6 = utility.SendEmail(registrationForm.CustomerDetails, str5, str4, profile.MAIL);
                                    this.Logger.DebugFormat("RegistrationController : Register Confirmation Email > result {0}", new object[] { str6 });
                                    string clientIP = this.GetClientIP();
                                    this._registrationrepository.SaveRegisterData(registrationForm.CustomerDetails.EmailAddress, clientIP, "");
                                    request.createJsonPublicasterRequest(registrationForm.CustomerDetails.EmailAddress, registrationForm.CustomerDetails.FirstName, registrationForm.CustomerDetails.LastName, interests, registrationForm.CustomerDetails.DisplayName, newsletter);
                                }
                            }
                            catch (Exception exception2)
                            {
                                exception = exception2;
                                this.Logger.Debug("Unable to send confirmation email to user.");
                            }
                        }
                        else
                        {
                            ((dynamic) base.ViewBag).Stage = "preferences";
                            flag3 = false;
                        }
                        break;

                    case "profile":
                        errorMessage = Helper.GetResource("PreferencesSaved");
                        model.RegistrationForm.returnUrl = this._settings.RegisterWelcome;
                        if (this.Preferences(registrationForm))
                        {
                            flag3 = true;
                            errorMessage = Helper.GetResource("ProfileSaved");
                            base.HttpContext.Session["registrationStatus"] = "";
                            returnUrl = this._settings.RegisterWelcome;
                            flag2 = true;
                        }
                        else
                        {
                            ((dynamic) base.ViewBag).Stage = "profile";
                            flag3 = false;
                        }
                        break;
                }
            }
            if (flag2)
            {
                returnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : base.Url.Content(WebConfiguration.Current.MyProfile);
                if (flag)
                {
                    return base.Json(new { 
                        success = flag3,
                        allowRedirect = flag2,
                        redirect = returnUrl,
                        message = errorMessage
                    });
                }
                base.HttpContext.Response.Redirect(returnUrl);
            }
            model.RegistrationForm = (registrationForm != null) ? registrationForm : new RegistrationForm();
            model.LoginForm = (loginForm != null) ? loginForm : new LoginForm();
            model.RegistrationForm.returnUrl = returnUrl;
            model.LoginForm.ReturnUrl = returnUrl;
            this.GetModelData(model.RegistrationForm);
            return base.View(model);
        }

        [HttpGet]
        public ActionResult Index(IComponentPresentation componentPresentation, EmailNewsletter emailNewsletter, string ReturnUrl = "", string stepoverride = "register")
        {
            if (!base.User.Identity.IsAuthenticated)
            {
                stepoverride = "register";
            }
            ((dynamic) base.ViewBag).Stage = stepoverride;
            try
            {
                ((dynamic) base.ViewBag).Title = componentPresentation.Component.Fields["title"].Value;
            }
            catch (Exception)
            {
            }
            Registration model = new Registration {
                RegistrationForm = new RegistrationForm(),
                LoginForm = new LoginForm()
            };
            NameValueCollection @params = base.Request.Params;
            if (!string.IsNullOrEmpty(@params["action"]))
            {
                if (@params["action"] == "addtoshoppinglist")
                {
                    ((dynamic) base.ViewBag).Title = Helper.GetResource("SignupShoppingList");
                }
                if (@params["action"] == "download")
                {
                    ((dynamic) base.ViewBag).Title = Helper.GetResource("SignupDownload");
                }
            }
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                model.LoginForm.ReturnUrl = ReturnUrl + "?" + base.Request.QueryString.ToString().Replace("iframe=true", "");
                model.RegistrationForm.returnUrl = ReturnUrl + "?" + base.Request.QueryString.ToString().Replace("iframe=true", "");
            }
            else if (!string.IsNullOrWhiteSpace(base.Request.QueryString["source"]))
            {
                model.RegistrationForm.returnUrl = model.LoginForm.ReturnUrl = base.Request.QueryString["source"];
            }
            else
            {
                model.LoginForm.ReturnUrl = base.Url.Content(WebConfiguration.Current.MyProfile);
                model.RegistrationForm.returnUrl = base.Url.Content(WebConfiguration.Current.MyProfile);
            }
            this.GetModelData(model.RegistrationForm);
            if ((emailNewsletter.Techniques != null) && (emailNewsletter.Techniques.Count > 0))
            {
                model.RegistrationForm.CraftTypeList = emailNewsletter.Techniques;
            }
            if (!string.IsNullOrEmpty(emailNewsletter.EmailAddress))
            {
                model.RegistrationForm.CustomerDetails.EmailAddress = emailNewsletter.EmailAddress;
            }
            SelectListItem item = model.RegistrationForm.EmailNewsletter.SingleOrDefault<SelectListItem>(e => e.Text == "No");
            if (item != null)
            {
                item.Selected = true;
            }
            return base.View(model);
        }

        private bool IsProfileValid(UserProfile model)
        {
            bool flag = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.CustomerDetails.EmailAddress))
                {
                    flag = false;
                    base.ModelState.AddModelError("EmailAddress", Helper.GetResource("EmailAddressEmpty"));
                    this.errorList.Add("EmailAddress");
                }
                if (string.IsNullOrEmpty(model.CustomerDetails.Password))
                {
                    flag = false;
                    base.ModelState.AddModelError("Password", Helper.GetResource("PasswordEmpty"));
                    this.errorList.Add("Password");
                }
                if (string.IsNullOrEmpty(model.CustomerDetails.VerifyPassword))
                {
                    flag = false;
                    base.ModelState.AddModelError("VerifyPassword", Helper.GetResource("VerifyPasswordEmpty"));
                    this.errorList.Add("VerifyPassword");
                }
                if (model.CustomerDetails.Password != model.CustomerDetails.VerifyPassword)
                {
                    flag = false;
                    base.ModelState.AddModelError("VerifyPassword", Helper.GetResource("ComparePassword"));
                    this.errorList.Add("PasswordMatch");
                }
                if (string.IsNullOrEmpty(model.CustomerDetails.FirstName))
                {
                    flag = false;
                    base.ModelState.AddModelError("FirstName", Helper.GetResource("FirstNameEmpty"));
                    this.errorList.Add("FirstName");
                }
                if (string.IsNullOrEmpty(model.CustomerDetails.LastName))
                {
                    flag = false;
                    base.ModelState.AddModelError("LastName", Helper.GetResource("LastNameEmpty"));
                    this.errorList.Add("LastName");
                }
                if (string.IsNullOrEmpty(model.CustomerDetails.DisplayName))
                {
                    flag = false;
                    base.ModelState.AddModelError("DisplayName", Helper.GetResource("DisplayNameEmpty"));
                    this.errorList.Add("DisplayName");
                }
            }
            return flag;
        }

        public static bool IsValidUser(LoginForm model)
        {
            return Membership.ValidateUser(model.EmailAddress, model.Password);
        }

        public static bool IsValidUser(RegistrationForm model)
        {
            return Membership.ValidateUser(model.CustomerDetails.EmailAddress, model.CustomerDetails.VerifyPassword);
        }

        private bool Login(LoginForm model)
        {
            try
            {
                if (IsValidUser(model))
                {
                    this.Logger.Info("RegistrationController.Login(UserProfile model)");
                    CoatsUserProfile profile = CoatsUserProfile.GetProfile(model.EmailAddress);
                    CookieHelper.WriteFormsCookie(profile.MAIL, profile.DISPLAYNAME, profile.NAME, profile.SURNAME, profile.LONG, profile.LAT, "");
                    return true;
                }
                base.ModelState.AddModelError("LoginFailed", Helper.GetResource("LoginFailed"));
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("Registration exception - {0}", new object[] { exception });
                base.ModelState.AddModelError("LoginFailed", Helper.GetResource("LoginFailed"));
                return false;
            }
            return false;
        }

        private bool Login(RegistrationForm model)
        {
            try
            {
                if (IsValidUser(model))
                {
                    this.Logger.Info("RegistrationController.Login(UserProfile model)");
                    return true;
                }
            }
            catch (Exception exception)
            {
                this.Logger.ErrorFormat("Registration exception - {0}", new object[] { exception });
                return false;
            }
            return false;
        }

        private static void MapUserProfile(UserProfile model, CoatsUserProfile userProfile)
        {
            userProfile.POSTCODE = model.AddressDetails.Postcode;
            userProfile.MAIL = model.CustomerDetails.EmailAddress;
            userProfile.NAME = model.CustomerDetails.FirstName;
            userProfile.SURNAME = model.CustomerDetails.LastName;
            userProfile.TELEPHONE = model.CustomerDetails.TelephoneNumber;
            userProfile.PASSWORD = model.CustomerDetails.Password;
            userProfile.ADDRESSBOOKID = int.Parse(ConfigurationManager.AppSettings["AddressBookId"]);
            userProfile.DISPLAYNAME = model.CustomerDetails.DisplayName;
            userProfile.ABOUT = model.CustomerDetails.About;
            userProfile.LONG = model.CustomerDetails.Long;
            userProfile.LAT = model.CustomerDetails.Lat;
            userProfile.Keywords = model.Keywords;
        }

        private bool Preferences(RegistrationForm model)
        {
            if (!base.User.Identity.IsAuthenticated)
            {
                return false;
            }
            if (model.AddressDetails.Postcode != null)
            {
                this.FindPostcode(model, true);
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                if (base.User.Identity.IsAuthenticated)
                {
                    CookieHelper.WriteFormsCookie(user.UserName, user.DISPLAYNAME, user.NAME, user.Lastname, model.CustomerDetails.Long, model.CustomerDetails.Lat, "");
                }
            }
            CoatsUserProfile userProfile = CoatsUserProfile.GetProfile(base.User.Identity.Name);
            if (userProfile == null)
            {
                return false;
            }
            this.AddKeywords(model);
            MapUserProfile(model, userProfile);
            userProfile.Save();
            base.HttpContext.Session["registrationStatus"] = "profile";
            return true;
        }

        private bool Register(RegistrationForm model)
        {
            if (base.ModelState.IsValid)
            {
                MembershipCreateStatus success = MembershipCreateStatus.Success;
                MembershipUser user = Membership.CreateUser(model.CustomerDetails.EmailAddress, model.CustomerDetails.Password, model.CustomerDetails.EmailAddress, null, null, true, Guid.NewGuid(), out success);
                if (success == MembershipCreateStatus.Success)
                {
                    if (user == null)
                    {
                        return false;
                    }
                    this.Logger.DebugFormat("RegistrationController POST CALLED >>>", new object[0]);
                    CoatsUserProfile userProfile = CoatsUserProfile.GetProfile(user.Email);
                    if (userProfile != null)
                    {
                        this.AddKeywords(model);
                        MapUserProfile(model, userProfile);
                        userProfile.Save();
                        this.Login(model);
                        CookieHelper.WriteFormsCookie(userProfile.MAIL, userProfile.DISPLAYNAME, userProfile.NAME, userProfile.SURNAME, userProfile.LONG, userProfile.LAT, "");
                        return true;
                    }
                }
            }
            return false;
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

