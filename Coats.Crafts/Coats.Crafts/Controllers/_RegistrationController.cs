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
using Coats.Crafts.Extensions;
using Coats.Crafts.Resources;
using Coats.IndustrialPortal.Providers;
using DD4T.ContentModel;
using System.Web;

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

        private void WriteFormsCookie(string username, string displayname, string firstname, string surname, string longitude, string latitude)
        {
            string userData = String.Format("{0}|{1}|{2}|{3}|{4}", displayname, firstname, surname, longitude, latitude);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    username,
                    DateTime.Now,
                    DateTime.Now.Add(FormsAuthentication.Timeout),
                    false,
                    userData,
                    FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }


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
                    WriteFormsCookie(user.MAIL, user.DISPLAYNAME, user.NAME, user.SURNAME, user.LONG, user.LAT);

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
                    tcm = UtilityHelper.GetTcmUri(listKeywordsSelectItem);
                    keywordTcmList.Add(tcm);
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
            return selectList;
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
                        model.PostCodeMapUrl = string.Format(ConfigurationManager.AppSettings["GoogleAPIsMapUrl"], _locationLat, _locationLong);
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

            var userProfile = CoatsUserProfile.GetProfile(User.Identity.Name);

            if (userProfile != null)
            {
                // Keywords appear to be overwritten with blank values on second submit - need to find a way to persist the data through as this keywords method links all radio buttons together
                AddKeywords(model);

                MapUserProfile(model, userProfile);
                userProfile.Save();
            }

            return true;
        }

        private bool Register(RegistrationForm model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            // Do we need to check model state here?
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

                        WriteFormsCookie(userProfile.MAIL, userProfile.DISPLAYNAME, userProfile.NAME, userProfile.SURNAME, userProfile.LONG, userProfile.LAT);
                    }
                }
                else
                {

                    Logger.DebugFormat("RegistrationController USER  >>>");
                }
            }

            return true;
        }


        [HttpGet]
        public ActionResult Index(IComponentPresentation componentPresentation, EmailNewsletter emailNewsletter)
        {
            try { ViewBag.Title = componentPresentation.Component.Fields["title"].Value; }
            catch (Exception) { }

            var registration = new Registration();
            var login = new LoginForm();
            var reg = new RegistrationForm();

            registration.RegistrationForm = reg;
            registration.LoginForm = login;

            var qParams = HttpContext.Request.Params;

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

            ViewBag.Stage = (!User.Identity.IsAuthenticated) ? "register" : "already-registered";

            if (!String.IsNullOrEmpty(Request.QueryString["redirect"]))
            {
                Session["RequestedAuthorizedPageUrl"] = Request.QueryString["redirect"];
                registration.LoginForm.ReturnUrl = Request.QueryString["redirect"];
                registration.RegistrationForm.CustomerDetails.ReturnUrl = Request.QueryString["redirect"];
            }

            if (!String.IsNullOrEmpty(Request.QueryString["instruction"]))
            {
                Session["RequestedAuthorizedPageInstruction"] = Request.QueryString["instruction"];
            }

            GetModelData(registration.RegistrationForm);

            if (emailNewsletter.Techniques != null && emailNewsletter.Techniques.Count > 0)
            {
                registration.RegistrationForm.CraftTypeList = emailNewsletter.Techniques;
                var singleOrDefault = registration.RegistrationForm.EmailNewsletter.SingleOrDefault(e => e.Text == "Yes");
                if (singleOrDefault != null)
                    singleOrDefault.Selected = true;
                var selectListItem = registration.RegistrationForm.EmailNewsletter.SingleOrDefault(e => e.Text == "No");
                if (selectListItem != null)
                    selectListItem.Selected = false;
            }
            if (!string.IsNullOrEmpty(emailNewsletter.EmailAddress))
            {
                registration.RegistrationForm.CustomerDetails.EmailAddress = emailNewsletter.EmailAddress;
            }

            return View(registration);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Index(ComponentPresentation componentPresentation, Registration model, String Stage, String Callback = String.Empty)
        {
            if (!String.IsNullOrEmpty(Stage))
            {

                var regFrom = model.RegistrationForm;
                String NuStage = String.Empty;

                switch (Stage)
                {
                    case "register":

                        // Check registration form isn't null
                        if (regFrom != null)
                        {

                            bool Registered = Register(regFrom);

                            // Set sign up stage based on registration success or not
                            NuStage = Registered ? "preferences" : "register";
                        }


                        /*
                         * 
                         * Check if we need to perform a callback action
                         * ----------
                         * Callbacks are performed after the first step is complete,
                         * no requirement to show the last two stages
                         * 
                         */

                        if (!String.IsNullOrEmpty(Callback))
                        {
                            // Hacky way to force redirect using context
                            ControllerContext.HttpContext.Response.Redirect("http://google.co.uk/");
                        }

                        break;
                    case "preferences":

                        // Check registration form isn't null
                        if (regFrom != null)
                        {
                            bool PreferencesSaved = Preferences(regFrom);

                            // Set sign up stage based on whether or not the user is valid
                            NuStage = PreferencesSaved ? "profile" : "register";
                        }

                        break;
                    case "profile":

                        // Check registration form isn't null
                        if (regFrom != null)
                        {
                            bool PreferencesSaved = Preferences(regFrom);

                            // Set sign up stage based on whether or not the user is valid
                            NuStage = PreferencesSaved ? "complete" : "register";
                        }

                        break;
                }

                ViewBag.Stage = NuStage;
            }

            // Get keyword data
            GetModelData(model.RegistrationForm);

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
            string craftstypes = Request["CraftType"];
            string newsletter = Request["email-newsletter"];
            string profilevisible = Request["visibile-profile"];

            if (craftstypes != null || newsletter != null || profilevisible != null)
            {
                List<TridionTcmUri> craftsTcmList = GetKeywordsSelectListItems(craftstypes);
                List<TridionTcmUri> craftsNewsLetterList = GetKeywordsSelectListItems(newsletter);
                List<TridionTcmUri> craftsProfileVisibleList = GetKeywordsSelectListItems(profilevisible);

                var craftsKeywords = craftsTcmList.Union(craftsNewsLetterList).Union(craftsProfileVisibleList).ToList();

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
            registrationform.CraftTypeList = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_craftlistidentifier));
            registrationform.EmailNewsletter = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_emailnewsletteridentifier));
            registrationform.ProfileVisible = GetKeywordsSelectList(registrationform, _keywordrepository.GetKeywordsList(_profilevisibleidentifier));
        }

        private void CheckEmail()
        {
            //if we're on step 2 and 3 of the registration process the validation will
            //fire an error because the email address is available.  Let's just remove the error.
            var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();

            if (errors.Any())
            {
                if (errors[0].Key == "RegistrationForm.CustomerDetails.EmailAddress")
                {
                    ModelState["RegistrationForm.CustomerDetails.EmailAddress"].Errors.Clear();
                }
            }
        }
    }







    // Kept in empty LoginForm as an example of how to combine models in a controller

    public class LoginForm : Login
    {
    }

    public class RegistrationForm : UserProfile
    {
        public string Formtype { get; set; }
    }

    public class Registration
    {
        public LoginForm LoginForm { get; set; }
        public RegistrationForm RegistrationForm { get; set; }
        public PasswordReminder PasswordReminder { get; set; }
    }


}
