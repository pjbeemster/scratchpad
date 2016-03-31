using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using Castle.Core.Logging;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Models;
using Coats.Crafts.Data;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Configuration;
using Coats.Crafts.Resources;
using Coats.IndustrialPortal.Providers;
using DD4T.ContentModel;
using System.Web;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.NewsletterAPI;

namespace Coats.Crafts.Controllers
{
    public class RegistrationController : Controller
    {
        public ILogger Logger { get; set; }

        private readonly IKeywordRepository _keywordrepository;
        private readonly IStoreLocatorRepository _storelocatorrepository;
        private readonly IRegistrationRepository _registrationrepository;
        private readonly string _craftlistidentifier;
        private readonly string _emailnewsletteridentifier;
        private readonly string _profilevisibleidentifier;

        private readonly IAppSettings _settings;

        public int PublicationId
        {
            get
            {
                return _settings.PublicationId;
            }
        }

        public RegistrationController(IStoreLocatorRepository storelocatorrepository, IKeywordRepository keywordrepository, IRegistrationRepository registrationrepository, IAppSettings settings)
        {
            _storelocatorrepository = storelocatorrepository;
            _keywordrepository = keywordrepository;
            _registrationrepository = registrationrepository;
            _settings = settings;

            _craftlistidentifier = string.Format(_settings.CraftListCategory, PublicationId);
            _emailnewsletteridentifier = string.Format(_settings.EmailNewsletterListCategory, PublicationId);
            _profilevisibleidentifier = string.Format(_settings.ProfileVisibleListCategory, PublicationId);
        }

        private string _locationLong;
        private string _locationLat;

        List<string> errorList = new List<string>();

        private static void MapUserProfile(UserProfile model, CoatsUserProfile userProfile)
        {
            userProfile.POSTCODE = model.AddressDetails.Postcode;
            userProfile.MAIL = model.CustomerDetails.EmailAddress;
            userProfile.NAME = model.CustomerDetails.FirstName;
            userProfile.SURNAME = model.CustomerDetails.LastName;
            userProfile.TELEPHONE = model.CustomerDetails.TelephoneNumber;
            userProfile.PASSWORD = model.CustomerDetails.Password;
            userProfile.ADDRESSBOOKID = Int32.Parse(ConfigurationManager.AppSettings["AddressBookId"]);
            userProfile.DISPLAYNAME = model.CustomerDetails.DisplayName;
            userProfile.ABOUT = model.CustomerDetails.About;
            userProfile.LONG = model.CustomerDetails.Long;
            userProfile.LAT = model.CustomerDetails.Lat;
            userProfile.Keywords = model.Keywords;
        }


        private string GetCreateUserError(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    return Helper.GetResource("EmailAlreadyExists");
                default:
                    return Helper.GetResource("CreateUserError");
            }
        }

        //private void WriteFormsCookie(string username, string displayname, string firstname, string surname, string longitude, string latitude)
        //{
        //    string userData = String.Format("{0}|{1}|{2}|{3}|{4}", displayname, firstname, surname, longitude, latitude);

        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
        //            username,
        //            DateTime.Now,
        //            DateTime.Now.Add(FormsAuthentication.Timeout),
        //            false,
        //            userData,
        //            FormsAuthentication.FormsCookiePath);

        //    // Encrypt the ticket.
        //    string encTicket = FormsAuthentication.Encrypt(ticket);

        //    // Create the cookie.
        //    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        //}


        private bool Login(LoginForm model)
        {
            try
            {
                var isValid = IsValidUser(model);

                if (isValid)
                {
                    Logger.Info("RegistrationController.Login(UserProfile model)");
                    //HttpCookie authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    //FormsAuthentication.SetAuthCookie(model.EmailAddress, false);        

                    var user = CoatsUserProfile.GetProfile(model.EmailAddress);

                    CookieHelper.WriteFormsCookie(user.MAIL, user.DISPLAYNAME, user.NAME, user.SURNAME, user.LONG, user.LAT, "");
                    //WriteFormsCookie(user.MAIL, user.DISPLAYNAME, user.NAME, user.SURNAME, user.LONG, user.LAT);

                    return true;
                }
                ModelState.AddModelError("LoginFailed", Helper.GetResource("LoginFailed"));
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Registration exception - {0}", ex);
                ModelState.AddModelError("LoginFailed", Helper.GetResource("LoginFailed"));
                return false;
            }
            return false;
        }

        private bool Login(RegistrationForm model)
        {
            try
            {
                var isValid = IsValidUser(model);

                if (isValid)
                {
                    //HttpCookie authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                    //FormsAuthentication.SetAuthCookie(model.CustomerDetails.EmailAddress, false);
                    Logger.Info("RegistrationController.Login(UserProfile model)");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Registration exception - {0}", ex);
                return false;
            }

            return false;
        }

        public static bool IsValidUser(LoginForm model)
        {
            return Membership.ValidateUser(model.EmailAddress, model.Password);
        }

        public static bool IsValidUser(RegistrationForm model)
        {
            return Membership.ValidateUser(model.CustomerDetails.EmailAddress, model.CustomerDetails.VerifyPassword);
        }

        private static List<TridionTcmUri> GetKeywordsSelectListItems(string selectedKeywords)
        {
            List<TridionTcmUri> keywordTcmList = new List<TridionTcmUri>();

            if (selectedKeywords != null)
            {
                var selectedArray = selectedKeywords.Split(',');

                TridionTcmUri tcm = new TridionTcmUri();

                foreach (var listKeywordsSelectItem in selectedArray)
                {
                    if (!String.IsNullOrEmpty(listKeywordsSelectItem))
                    {
                        tcm = UtilityHelper.GetTcmUri(listKeywordsSelectItem);
                        keywordTcmList.Add(tcm);
                    }
                }
            }

            return keywordTcmList;
        }


        private static List<SelectListItem> GetKeywordsSelectList(RegistrationForm model, IEnumerable<Coats.Crafts.Models.Keyword> keywords)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var item in keywords)
            {
                selectList.Add(new SelectListItem { Text = item.Description, Value = item.Id.ToString() });
            }

            return selectList.OrderBy(x => x.Text).ToList();
        }


        public Dictionary<string, string> CheckPostcode(string postcode)
        {
            Dictionary<string, string> dicPostcode = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(postcode))
            {
                // Get all possible locations from Google
                List<GoogleMapsMarker> Locations = _storelocatorrepository.GetMarkerForPostcode(postcode);

                // Get first returned result
                GoogleMapsMarker currentLoc = Locations[0];

                // Set variables
                if (currentLoc != null)
                {
                    _locationLat = currentLoc.lat.ToString();
                    dicPostcode.Add("lat", _locationLat);
                    _locationLong = currentLoc.lng.ToString();
                    dicPostcode.Add("long", _locationLong);
                }
            }

            return dicPostcode;
        }


        private ViewResult FindPostcode(RegistrationForm model, bool fullForm)
        {
            Dictionary<string, string> dicPostcode = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(model.AddressDetails.Postcode))
            {
                float lngCheck;
                float latCheck;

                string lat = Request["lat"];
                string lng = Request["long"];

                bool checkFloatLat = float.TryParse(lat, out latCheck);
                bool checkFloatLong = float.TryParse(lng, out lngCheck);

                if (checkFloatLat && checkFloatLong)
                {
                    model.CustomerDetails.Lat = lat;
                    model.CustomerDetails.Long = lng;
                }
                else
                {

                    dicPostcode = CheckPostcode(model.AddressDetails.Postcode);

                    if (dicPostcode.Count > 0)
                    {
                        model.CustomerDetails.Lat = dicPostcode["lat"];
                        model.CustomerDetails.Long = dicPostcode["long"];
                        string mapsKeyParam = !String.IsNullOrEmpty(_settings.GoogleMapsAPIKey) ? "&key=" + _settings.GoogleMapsAPIKey : "";
                        model.PostCodeMapUrl = string.Format(_settings.GoogleAPIsMapUrl + mapsKeyParam, _locationLat, _locationLong);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Helper.GetResource("PostcodeError"));
                        errorList.Add("Postcode");
                    }
                }
            }

            return View(model);

        }

        private bool Preferences(RegistrationForm model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (model.AddressDetails.Postcode != null)
            {
                FindPostcode(model, true);

                var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;

                //update the cookie with lat and long
                if (User.Identity.IsAuthenticated)
                {
                    CookieHelper.WriteFormsCookie(user.UserName, user.DISPLAYNAME, user.NAME, user.Lastname, model.CustomerDetails.Long, model.CustomerDetails.Lat, "");
                }

            }

            var userProfile = CoatsUserProfile.GetProfile(User.Identity.Name);

            if (userProfile == null)
            {
                return false;
            }

            AddKeywords(model);

            MapUserProfile(model, userProfile);
            userProfile.Save();
            //Session value is used to identify if user is on the profile stage 
            //and has clicked away from the lightbox and then clicked on the login link
            HttpContext.Session["registrationStatus"] = "profile";

            return true;
        }

        private bool Register(RegistrationForm model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            var stati = new MembershipCreateStatus();
            var mUser = Membership.CreateUser(model.CustomerDetails.EmailAddress,
                                                            model.CustomerDetails.Password,
                                                            model.CustomerDetails.EmailAddress, null, null, true,
                                                            Guid.NewGuid(), out stati);
            if (stati == MembershipCreateStatus.Success)
            {
                if (mUser != null)
                {
                    Logger.DebugFormat("RegistrationController POST CALLED >>>");

                    var userProfile = CoatsUserProfile.GetProfile(mUser.Email);

                    if (userProfile != null)
                    {
                        AddKeywords(model);

                        MapUserProfile(model, userProfile);
                        userProfile.Save();

                        Login(model);

                        CookieHelper.WriteFormsCookie(userProfile.MAIL, userProfile.DISPLAYNAME, userProfile.NAME, userProfile.SURNAME, userProfile.LONG, userProfile.LAT, "");
                        return true;
                    }
                }
                else
                {
                    return false;
                    Logger.DebugFormat("RegistrationController USER  >>>");
                }
            }

            return false;
        }


        [HttpGet]
        public ActionResult Index(IComponentPresentation componentPresentation, EmailNewsletter emailNewsletter, String ReturnUrl = "", String stepoverride = "register")
        {
            // Make sure unregistered users only ever get step one
            if (!User.Identity.IsAuthenticated)
            {
                stepoverride = "register";
            }

            // Show logged in users the preferences view by default - avoids some issues when using the back button after an action was completed in the lightbox
            if (User.Identity.IsAuthenticated && stepoverride == "register")
            {
                stepoverride = "preferences";
            }

            if (HttpContext.Session["registrationStatus"] == "profile")
            {
                stepoverride = "profile";
            }

            // Set viewbag property, used to switch which form is shown to the user
            ViewBag.Stage = stepoverride;

            try { ViewBag.Title = componentPresentation.Component.Fields["title"].Value; }
            catch (Exception) { }

            var registration = new Registration();

            registration.RegistrationForm = new RegistrationForm();
            registration.LoginForm = new LoginForm();

            var qParams = Request.Params;

            // Check if callback action is defined and add title if necessary
            if (!String.IsNullOrEmpty(qParams["action"]))
            {
                if (qParams["action"] == "addtoshoppinglist")
                {
                    ViewBag.Title = Helper.GetResource("SignupShoppingList");
                }
                if (qParams["action"] == "download")
                {
                    ViewBag.Title = Helper.GetResource("SignupDownload");
                }
            }

            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                registration.LoginForm.ReturnUrl = ReturnUrl + "?" + Request.QueryString.ToString().Replace("iframe=true", "");
                registration.RegistrationForm.returnUrl = ReturnUrl + "?" + Request.QueryString.ToString().Replace("iframe=true", "");
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(Request.QueryString["source"]))
                {
                    registration.RegistrationForm.returnUrl = registration.LoginForm.ReturnUrl = Request.QueryString["source"];
                }
                else
                {
                    registration.LoginForm.ReturnUrl = Url.Content(WebConfiguration.Current.MyProfile);
                    registration.RegistrationForm.returnUrl = Url.Content(WebConfiguration.Current.MyProfile);
                }
            }

            GetModelData(registration.RegistrationForm);

            // Bind provided data to registration model to be used in next view generation
            if (emailNewsletter.Techniques != null && emailNewsletter.Techniques.Count > 0)
            {
                registration.RegistrationForm.CraftTypeList = emailNewsletter.Techniques;
            }
            if (!string.IsNullOrEmpty(emailNewsletter.EmailAddress))
            {
                registration.RegistrationForm.CustomerDetails.EmailAddress = emailNewsletter.EmailAddress;
            }

            // Set no as default option for newsetter
            var singleOrDefault = registration.RegistrationForm.EmailNewsletter.SingleOrDefault(e => e.Text == "No");

            if (singleOrDefault != null)
            {
                singleOrDefault.Selected = true;
            }

            return View(registration);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Index(ComponentPresentation componentPresentation, Registration model, String Stage)
        {

            Boolean ajaxRequest = Request.IsAjaxRequest();
            Boolean allowRedirect = false;
            Boolean success = false;

            String message = String.Empty;
            String fbRedirect = String.Empty;

            LoginForm loginForm = model.LoginForm;
            RegistrationForm regisForm = model.RegistrationForm;

            /*
             * 
             * This logic is now split into two distinct regions to make it easier
             * to follow. In future it would be nice if the login and registration
             * forms posted to different methods to make model rebinding (as well
             * as persisting data) much easier (avoids nulls on view generation).
             * 
             * Ajax requests are returned a JSON object which provides values that 
             * are used to tell the client how to respond to an action. It
             * essentially duplicates the logic here for refdirections etc.
             * 
             * 
             */

            var source = !String.IsNullOrWhiteSpace(Request["source"]) ? Request["source"] : string.Empty;

            #region Login handler

            /*-------------------------*/

            if (loginForm != null)
            {
                if (!String.IsNullOrEmpty(loginForm.ReturnUrl))
                {
                    fbRedirect = loginForm.ReturnUrl;
                }

                if (!String.IsNullOrEmpty(source))
                {
                    fbRedirect = source;
                }

                message = Helper.GetResource("LoginFailed");

                if (Login(loginForm))
                {
                    message = Helper.GetResource("LoginSuccess"); //"Logged in successfully";
                    success = true;

                    allowRedirect = true;
                }
                else
                {
                    ViewBag.Stage = "register";

                    ModelState.AddModelError("LoginFailed", message);
                }


                if (ajaxRequest)
                {
                    return Json(
                            new
                            {
                                success = success,
                                allowRedirect = allowRedirect,
                                redirect = fbRedirect,
                                message = message
                            }
                        );
                }
            }

            #endregion

            #region Registration handler

            /*-------------------------*/

            if (regisForm != null)
            {

                if (!String.IsNullOrEmpty(regisForm.returnUrl))
                {
                    fbRedirect = regisForm.returnUrl;
                }

                if (!String.IsNullOrEmpty(source))
                {
                    fbRedirect = source;
                }
                PublicasterServiceRequest oPublicasterServiceRequest = new PublicasterServiceRequest();
                switch (Stage)
                {
                    case "register":

                        message = Helper.GetResource("RegistrationFailed"); //"Registration failed. Please try again later.";

                        if (IsProfileValid(regisForm))
                        {
                            if (Register(regisForm))
                            {

                                //Insert to Records into Table
                                //Added by Ajaya for double opt
                                //string clientIP = GetClientIP();
                                //_registrationrepository.SaveRegisterData(regisForm.CustomerDetails.EmailAddress, clientIP, "");
                                //oPublicasterServiceRequest.createJsonPublicasterRequest(regisForm.CustomerDetails.EmailAddress, regisForm.CustomerDetails.FirstName, regisForm.CustomerDetails.LastName, "", regisForm.CustomerDetails.DisplayName, "no");
                                ViewBag.Stage = "preferences";
                                success = true;
                                message = Helper.GetResource("RegisteredSuccessfully"); //"New user registered";

                                Logger.InfoFormat("RegistrationController : User created: {0} - {1}", regisForm.CustomerDetails.DisplayName, regisForm.CustomerDetails.EmailAddress);

                                try
                                {
                                    // Just before we carry on, let's pop out an email to the user to let them know the good news!
                                    //Added by Ajaya for double opt
                                    if (!Coats.Crafts.Configuration.WebConfiguration.Current.CheckEmailNewsletterOption)
                                    {
                                        EmailUtility util = new EmailUtility();
                                        string adminEmail = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"]; // Re-using address
                                        string registerConfirmationEmailTemplate = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];
                                        string result = util.SendEmail(regisForm.CustomerDetails, registerConfirmationEmailTemplate, adminEmail, regisForm.CustomerDetails.EmailAddress);
                                        Logger.DebugFormat("RegistrationController : Register Confirmation Email > result {0}", result);
                                    }

                                }
                                catch (Exception err)
                                {
                                    Logger.Debug("Unable to send confirmation email to user.");
                                }

                                /*
                                 * 
                                 * Here is how we decide if we redirect after the first action or not.
                                 * Remove this block if we want to funnel users towards completing
                                 * three steps instead of performing intended action after completion 
                                 * of step one.
                                 * 
                                 * Basically checking if the url contains an action to performed after 
                                 * successful login/registration
                                 * 
                                 * If we don't use this logic, we'll need to add a little script (JS)
                                 * that pulls the redirect url out of the iframe in the lightbox so we
                                 * can trigger it if the users aborts the process after step one by
                                 * closing the lightbox or clicking outside of it
                                 * 
                                 * eg: top.setOnCloseRedirect(redirectUrl);
                                 * 
                                 * 
                                 */
                                //Commented by Ajaya for DE
                                //if (fbRedirect.Contains("action="))
                                //{
                                //    allowRedirect = true;
                                //}
                            }
                            else
                            {
                                ViewBag.Stage = "register";
                                success = false;
                                message = Helper.GetResource("UnableToCreateNewUser"); //"Unable to create new user";
                                ModelState.AddModelError("UnableToCreateNewUser", Helper.GetResource("UnableToCreateNewUser"));
                            }
                        }
                        else
                        {
                            ViewBag.Stage = "register";
                            success = false;
                            message = Helper.GetResource("RegistrationFailed");
                            ModelState.AddModelError("UnableToCreateNewUser", Helper.GetResource("UnableToCreateNewUser"));
                        }

                        break;

                    case "preferences":

                        message = Helper.GetResource("PreferencesFailed"); //"Unable to save preferences. Please try again later.";

                        //Added by Ajaya for double opt
                     
                        model.RegistrationForm.returnUrl = _settings.RegisterWelcome;
                        if (Preferences(regisForm))
                        {
                            ViewBag.Stage = "profile";
                            success = true;
                            message = Helper.GetResource("PreferencesSaved");// "Your preferences have been saved.";

                            try
                            {
                                // Just before we carry on, let's pop out an email to the user to let them know the good news!
                                var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;
                                var userProfile = CoatsUserProfile.GetProfile(User.Identity.Name);
                                EmailUtility util = new EmailUtility();
                                string adminEmail = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"]; // Re-using address
                                string registerConfirmationEmailTemplate = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];
                                //Added by Ajaya for double opt
                                string IsNewsletter = "No";
                                string Interests = string.Empty;
                                foreach(var Keys in regisForm.Keywords)
                                {
                                    Interests += Keys.Key+",";
                                }

                                if (Interests.Length > 0)
                                {
                                    Interests = Interests.Substring(0, (Interests.LastIndexOf(",")-1));
                                }

                                var singleOrDefault = regisForm.Keywords.SingleOrDefault(e => e.Key == Coats.Crafts.Configuration.WebConfiguration.Current.EmailNewsletterYes);

                                if (singleOrDefault.Key != null)
                                {
                                    IsNewsletter = "yes";
                                }
                                if (Coats.Crafts.Configuration.WebConfiguration.Current.CheckEmailNewsletterOption)
                                {
                                   
                                    if (singleOrDefault.Key != null)
                                    {
                                        IsNewsletter = "yes";
                                        string redirect = string.Empty;
                                        if (Request.Url != null)
                                        {
                                            redirect = Request.Url.Scheme + "://" + Request.Url.Host;
                                        }
                                        else
                                        {
                                            redirect = ConfigurationManager.AppSettings["SiteUrl"];
                                        }


                                        string ThankYou = _settings.RegisterThankYou;
                                        string querystrEmail = HttpUtility.UrlEncode(Utils.General.Encrypt(userProfile.MAIL.Trim()));
                                        ThankYou += "?UserEmail=" + querystrEmail;
                                        redirect = ThankYou;
                                        regisForm.CustomerDetails.SiteUrl = redirect;
                                        regisForm.CustomerDetails.DisplayName = userProfile.DISPLAYNAME;
                                        regisForm.CustomerDetails.LastName = userProfile.SURNAME;
                                        regisForm.CustomerDetails.FirstName  = userProfile.NAME ;
                                        regisForm.CustomerDetails.EmailAddress = userProfile.MAIL;
                                        //string result = util.SendEmail(userProfile, registerConfirmationEmailTemplate, adminEmail, userProfile.MAIL);
                                        string result = util.SendEmail(regisForm.CustomerDetails, registerConfirmationEmailTemplate, adminEmail, userProfile.MAIL);
                                        Logger.DebugFormat("RegistrationController : Register Confirmation Email > result {0}", result);

                                        //Insert to Records into Table
                                        //Added by Ajaya for double opt
                                        string clientIP = GetClientIP();
                                        _registrationrepository.SaveRegisterData(regisForm.CustomerDetails.EmailAddress, clientIP, "");
                                         oPublicasterServiceRequest.createJsonPublicasterRequest(regisForm.CustomerDetails.EmailAddress, regisForm.CustomerDetails.FirstName, regisForm.CustomerDetails.LastName, Interests, regisForm.CustomerDetails.DisplayName, IsNewsletter);
                                    }
                                }
                              //  oPublicasterServiceRequest.createJsonPublicasterRequest(regisForm.CustomerDetails.EmailAddress, regisForm.CustomerDetails.FirstName, regisForm.CustomerDetails.LastName, Interests, regisForm.CustomerDetails.DisplayName, IsNewsletter);

                            }
                            catch (Exception err)
                            {
                                Logger.Debug("Unable to send confirmation email to user.");
                            }

                        }
                        else
                        {
                            ViewBag.Stage = "preferences";
                            success = false;
                        }

                        break;

                    case "profile":

                        message = Helper.GetResource("PreferencesSaved"); //"Unable to save your profile. Please try again later.";
                        //Added by Ajaya for double opt
                        model.RegistrationForm.returnUrl = _settings.RegisterWelcome;
                        if (Preferences(regisForm))
                        {

                            success = true;
                            message = Helper.GetResource("ProfileSaved"); //"Your profile has been saved";
                            HttpContext.Session["registrationStatus"] = "";

                            //Added by Ajaya for double opt
                            string WelcomeUrl = _settings.RegisterWelcome;
                            fbRedirect = WelcomeUrl;

                            //  ViewBag.Stage = "thanksMessage";

                            allowRedirect = true;
                        }
                        else
                        {
                            ViewBag.Stage = "profile";
                            success = false;
                        }

                        break;




                }
            }

            if (allowRedirect)
            {
                fbRedirect = !String.IsNullOrEmpty(fbRedirect) ? fbRedirect : Url.Content(WebConfiguration.Current.MyProfile);

                if (ajaxRequest)
                {
                    return Json(
                            new
                            {
                                success = success,
                                allowRedirect = allowRedirect,
                                redirect = fbRedirect,
                                message = message
                            }
                        );
                }

                HttpContext.Response.Redirect(fbRedirect);
            }

            // Make sure we never have empty nested models, instantiate new ones if null
            model.RegistrationForm = regisForm != null ? regisForm : new RegistrationForm();
            model.LoginForm = loginForm != null ? loginForm : new LoginForm();

            // Rebind newly built return url string - this will only work if no data has been posted for this property
            model.RegistrationForm.returnUrl = fbRedirect;
            model.LoginForm.ReturnUrl = fbRedirect;

            // Get keyword data for lists
            GetModelData(model.RegistrationForm);

            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult EmailNewsletterSignup(EmailNewsletter emailNewsLetter)
        {
            var registration = new Registration();
            var login = new LoginForm();
            var reg = new RegistrationForm();

            registration.RegistrationForm = reg;
            registration.LoginForm = login;

            GetModelData(registration.RegistrationForm);

            registration.RegistrationForm.CraftTypeList = emailNewsLetter.Techniques;
            var singleOrDefault = registration.RegistrationForm.EmailNewsletter.SingleOrDefault(e => e.Text == "Yes");
            if (singleOrDefault != null)
                singleOrDefault.Selected = true;
            var selectListItem = registration.RegistrationForm.EmailNewsletter.SingleOrDefault(e => e.Text == "No");
            if (selectListItem != null)
                selectListItem.Selected = false;
            registration.RegistrationForm.CustomerDetails.EmailAddress = emailNewsLetter.EmailAddress;
            return View("Index", registration);
        }

        private ViewResult AddKeywords(RegistrationForm model)
        {
            /*
             * 
             * Here we're binding keyword-stash as well as the other options.
             * Could likely have used hidden fields for CraftType data
             * instead of this in retrospect. This is used to persist the
             * previously selected options through the posts which would
             * otherwise be missed on subsequent profile updates.
             * 
             * 
             */
            string stash = Request["keyword-stash"];
            string craftstypes = Request["CraftType"];
            string newsletter = Request["email-newsletter"];
            string profilevisible = Request["visibile-profile"];

            if (stash != null || craftstypes != null || newsletter != null || profilevisible != null)
            {
                List<TridionTcmUri> craftsKeywordStash = GetKeywordsSelectListItems(stash);
                List<TridionTcmUri> craftsPreferredTechniques = GetKeywordsSelectListItems(craftstypes);
                List<TridionTcmUri> craftsNewsLetterList = GetKeywordsSelectListItems(newsletter);
                List<TridionTcmUri> craftsProfileVisibleList = GetKeywordsSelectListItems(profilevisible);

                var craftsKeywords = craftsKeywordStash.Union(craftsPreferredTechniques).Union(craftsNewsLetterList).Union(craftsProfileVisibleList).ToList();

                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (TridionTcmUri tcm in craftsKeywords)
                {
                    dic.Add(tcm.TcmId, "tcm");
                }

                model.Keywords = dic;
            }

            return View(model);
        }


        private bool IsProfileValid(UserProfile model)
        {
            var isValid = true;

            if (model != null)
            {
                if (String.IsNullOrEmpty(model.CustomerDetails.EmailAddress))
                {
                    isValid = false;
                    ModelState.AddModelError("EmailAddress", Helper.GetResource("EmailAddressEmpty"));
                    errorList.Add("EmailAddress");
                }

                if (String.IsNullOrEmpty(model.CustomerDetails.Password))
                {
                    isValid = false;
                    ModelState.AddModelError("Password", Helper.GetResource("PasswordEmpty"));
                    errorList.Add("Password");
                }

                if (String.IsNullOrEmpty(model.CustomerDetails.VerifyPassword))
                {
                    isValid = false;
                    ModelState.AddModelError("VerifyPassword", Helper.GetResource("VerifyPasswordEmpty"));
                    errorList.Add("VerifyPassword");
                }

                if (model.CustomerDetails.Password != model.CustomerDetails.VerifyPassword)
                {
                    isValid = false;
                    ModelState.AddModelError("VerifyPassword", Helper.GetResource("ComparePassword"));
                    errorList.Add("PasswordMatch");
                }

                if (String.IsNullOrEmpty(model.CustomerDetails.FirstName))
                {
                    isValid = false;
                    ModelState.AddModelError("FirstName", Helper.GetResource("FirstNameEmpty"));
                    errorList.Add("FirstName");
                }

                if (String.IsNullOrEmpty(model.CustomerDetails.LastName))
                {
                    isValid = false;
                    ModelState.AddModelError("LastName", Helper.GetResource("LastNameEmpty"));
                    errorList.Add("LastName");
                }

                if (String.IsNullOrEmpty(model.CustomerDetails.DisplayName))
                {
                    isValid = false;
                    ModelState.AddModelError("DisplayName", Helper.GetResource("DisplayNameEmpty"));
                    errorList.Add("DisplayName");
                }
            }

            return isValid;
        }


        private void GetModelData(RegistrationForm registrationform)
        {
            // Only get data if not already set - prevents overwriting of choices on failure
            if (registrationform.CraftTypeList == null)
            {
                registrationform.CraftTypeList = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_craftlistidentifier));
            }

            // Only get data if not already set - prevents overwriting of choices on failure
            if (registrationform.EmailNewsletter == null)
            {
                registrationform.EmailNewsletter = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_emailnewsletteridentifier));
            }

            // Only get data if not already set - prevents overwriting of choices on failure
            if (registrationform.ProfileVisible == null)
            {
                registrationform.ProfileVisible = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_profilevisibleidentifier));
            }
        }

        //Added by Ajaya for double opt
        private string GetClientIP()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
    }







    // Kept in empty LoginForm as an example of how to combine models in a controller

    public class LoginForm : Login
    {
    }

    public class RegistrationForm : UserProfile
    {
    }

    public class Registration
    {
        public LoginForm LoginForm { get; set; }
        public RegistrationForm RegistrationForm { get; set; }
        public PasswordReminder PasswordReminder { get; set; }
    }
}
