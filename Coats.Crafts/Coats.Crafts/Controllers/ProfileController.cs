using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;
using Coats.Crafts.Interfaces;
using Coats.Crafts.Models;
using Coats.IndustrialPortal.Providers;
using Coats.Crafts.Repositories.Interfaces;
using System.Configuration;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Data;
using Coats.Crafts.Resources;
using DD4T.ContentModel;
using System.Web;
using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.NewsletterAPI;

namespace Coats.Crafts.Controllers
{
    public class ProfileController : Controller
    {
        public ILogger Logger { get; set; }

        private readonly IKeywordRepository _keywordrepository;
        private readonly string _craftlistidentifier;
        private readonly string _emailnewsletteridentifier;
        private readonly string _profilevisibleidentifier;

        private readonly IStoreLocatorRepository _storelocatorrepository;
        private readonly IAppSettings _settings;
        private string _locationLong;
        private string _locationLat;

        public int PublicationId
        {
            get
            {
                return _settings.PublicationId;
            }
        }

        public ProfileController(IStoreLocatorRepository storelocatorrepository, IKeywordRepository keywordrepository, IAppSettings settings)
        {
            _storelocatorrepository = storelocatorrepository;
            _keywordrepository = keywordrepository;
            _settings = settings;

            _craftlistidentifier = string.Format(_settings.CraftListCategory, PublicationId);
            _emailnewsletteridentifier = string.Format(_settings.EmailNewsletterListCategory, PublicationId);
            _profilevisibleidentifier = string.Format(_settings.ProfileVisibleListCategory, PublicationId);
        }

        [Authorize]
        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            var userProfile = GetModel();

            try { ViewBag.Title = componentPresentation.Component.Fields["title"].Value; }
            catch (Exception) { }

            if (userProfile == null)
            {
                Response.Redirect(Url.Content(WebConfiguration.Current.AccessDenied.AddApplicationRoot()));
            }

            if (userProfile != null)
            {
                if (Request.Url != null) userProfile.returnUrl = Request.Url.ToString();

                return View(userProfile);
            }
            return null;
        }



        [HttpPost, ValidateInput(true)]
        public ActionResult Index (ComponentPresentation componentPresentation, UserProfile model)
        {
            Logger.DebugFormat("Called Update Profile >>>> ");

            try { ViewBag.Title = componentPresentation.Component.Fields["title"].Value; }
            catch (Exception) { }

            string user = string.Empty;

            if (User.Identity.IsAuthenticated)
            {
                user = User.Identity.Name;
            }

            var isValid = IsEditModelValid(model);

            if (isValid)
            {
                try
                {
                    var mUser = Membership.GetUser();

                    if (mUser == null)
                    {
                        isValid = false;
                        Logger.DebugFormat("Update Profile mUser null");
                    }

                    if (model.AddressDetails.Postcode != null)
                    {
                        FindPostcode(model);
                    }

                    if (model.RegistrationStatus == "postcode invalid")
                    {
                        isValid = false;
                    }

                    if (isValid)
                    {
                        Logger.DebugFormat("User profile start  >>>>");

                        var coatsUserProfile = CoatsUserProfile.GetProfile(mUser.Email);

                        if (coatsUserProfile != null)
                        {
                            string craftstypes = Request["CraftType"];
                            string newsletter = Request["email-newsletter"];
                            string profilevisible = Request["visibile-profile"];
                            //Added by Ajaya for Double Opt
                            PublicasterServiceRequest oPublicasterServiceRequest = new PublicasterServiceRequest();
                            string IsNewsletter = "No";
                            
                             if(newsletter.Trim() == Coats.Crafts.Configuration.WebConfiguration.Current.EmailNewsletterYes.Trim())
                               {
                                    IsNewsletter = "yes";
                               }

                           
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

                                coatsUserProfile.Keywords = dic;
                                model.Keywords = dic;
                            }


                            if (!string.IsNullOrEmpty(model.NewPassword))
                            {
                                if (model.NewPassword == model.VerifyNewPassword)
                                {
                                    if (!string.IsNullOrEmpty(model.CurrentPassword))
                                    {
                                        var testUser = CoatsUserProfile.GetProfile(user);

                                        if (model.CurrentPassword == testUser.PASSWORD)
                                        {
                                            // update password
                                            model.CustomerDetails.Password = model.NewPassword;
                                        }
                                        else
                                        {
                                            isValid = false;
                                            ModelState.AddModelError("CurrentPasswordIncorrect", Helper.GetResource("CurrentPasswordIncorrect"));
                                        }
                                    }
                                    else
                                    {
                                        isValid = false;
                                        //ModelState.AddModelError("CurrentPasswordBlank", Helper.GetResource("CurrentPasswordBlank"));
                                    }
                                }
                                else
                                {
                                    isValid = false;
                                    ModelState.AddModelError("NewPasswordsNotMatch", Helper.GetResource("NewPasswordsNotMatch"));
                                }
                            }

                            //Email address has changed
                            //if (mUser.Email != coatsUserProfile.MAIL)
                            //{
                            //    var testUser = CoatsUserProfile.GetProfile(coatsUserProfile.MAIL);

                            //    try
                            //    {
                            //        if (!string.IsNullOrEmpty(testUser.MAIL))
                            //        {
                            //            //User has tried to change email address but email address already exists
                            //            isValid = false;
                            //            ModelState.AddModelError(string.Empty, "Email address already exists.");
                            //        }
                            //    } catch(Exception ex)
                            //    {
                            //        isValid = true;
                            //    }
                            //}

                            if (isValid)
                            {
                                MapUserProfileForUpdate(model, coatsUserProfile);
                                coatsUserProfile.Save();
                                //Added By Ajaya for Double Opt
                                if (newsletter != null)
                                {
                                    bool IsCheckEmailNewsletterOption = Coats.Crafts.Configuration.WebConfiguration.Current.CheckEmailNewsletterOption;

                                    if (IsCheckEmailNewsletterOption)
                                    {
                                        if (newsletter.Trim() == Coats.Crafts.Configuration.WebConfiguration.Current.EmailNewsletterYes.Trim())
                                        {
                                            EmailUtility util = new EmailUtility();
                                            string adminEmail = ConfigurationManager.AppSettings["PasswordReminderEmailFrom"]; // Re-using address
                                            string registerConfirmationEmailTemplate = ConfigurationManager.AppSettings["RegisterConfirmationEmailTemplate"];

                                            string ThankYou = _settings.RegisterThankYou;
                                            string querystrEmail = HttpUtility.UrlEncode(Utils.General.Encrypt(coatsUserProfile.MAIL.Trim()));
                                            ThankYou += "?UserEmail=" + querystrEmail;
                                            model.CustomerDetails.SiteUrl = ThankYou;
                                            model.CustomerDetails.DisplayName = coatsUserProfile.DISPLAYNAME;
                                            model.CustomerDetails.EmailAddress = coatsUserProfile.MAIL;
                                            string result = util.SendEmail(model.CustomerDetails, registerConfirmationEmailTemplate, adminEmail, coatsUserProfile.MAIL);
                                            Logger.DebugFormat("ProfileController : Register Confirmation Email > result {0}", result);

                                            // if (newsletter.Trim() == Coats.Crafts.Configuration.WebConfiguration.Current.EmailNewsletterYes.Trim())
                                            oPublicasterServiceRequest.createJsonPublicasterRequest(coatsUserProfile.MAIL, coatsUserProfile.NAME, coatsUserProfile.SURNAME, craftstypes, coatsUserProfile.DISPLAYNAME, IsNewsletter);

                                        }
                                        else
                                        {
                                            oPublicasterServiceRequest.UnSubscripePublicaster(coatsUserProfile.MAIL);
                                        }
                                    }
                                }
                                //if (newsletter.Trim() == Coats.Crafts.Configuration.WebConfiguration.Current.EmailNewsletterYes.Trim())
                                //    oPublicasterServiceRequest.createJsonPublicasterRequest(coatsUserProfile.MAIL, coatsUserProfile.NAME, coatsUserProfile.SURNAME, craftstypes, coatsUserProfile.DISPLAYNAME, IsNewsletter);
                                //else
                                //    oPublicasterServiceRequest.UnSubscripePublicaster(coatsUserProfile.MAIL);
                                //Ending By Ajaya for Double Opt
                                

                                var newUser = CoatsUserProfile.GetProfile(coatsUserProfile.MAIL);

                                //FormsAuthentication.SetAuthCookie(coatsUserProfile.MAIL, false);
                                //WriteFormsCookie(coatsUserProfile.MAIL, coatsUserProfile.DISPLAYNAME, coatsUserProfile.NAME, coatsUserProfile.SURNAME, coatsUserProfile.LONG, coatsUserProfile.LAT);
                                CookieHelper.WriteFormsCookie(coatsUserProfile.MAIL, coatsUserProfile.DISPLAYNAME, coatsUserProfile.NAME, coatsUserProfile.SURNAME, coatsUserProfile.LONG, coatsUserProfile.LAT, "");

                                ViewBag.profileStatus = "saved";

                                if (newUser == null)
                                {
                                    isValid = false;
                                    ModelState.AddModelError(string.Empty, "");
                                    Logger.DebugFormat("Update Profile newUser null");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat("Update Profile exception {0} {1}", ex.Message, ex.InnerException);
                    isValid = false;
                }
            }

            // Profile is using the customerdetails model which is used for registration
            // We need to clear the model errors on displayname and email address as we don't care about the errors for profile
            if (ModelState.ContainsKey("CustomerDetails.DisplayName"))
                ModelState["CustomerDetails.DisplayName"].Errors.Clear();

            if (ModelState.ContainsKey("CustomerDetails.EmailAddress"))
                ModelState["CustomerDetails.EmailAddress"].Errors.Clear();


            if (!isValid)
            {
                var userProfile = GetModel();

                // TODO No ERRORS
                ModelState.AddModelError(string.Empty, "");

                var errors = ModelState
                   .Where(x => x.Value.Errors.Count > 0)
                   .Select(x => new { x.Key, x.Value.Errors })
                   .ToArray();

                foreach (var error in errors)
                {
                    Logger.DebugFormat("Update Profile error {0}", error);
                }

                GetModelData(userProfile);

                Session["feedback"] = Helper.GetResource("ProfileError");
                
                ViewBag.profileStatus = "error";

                return View(userProfile);
            }

            

            var updatedProfile = GetModel();

            Session["feedback"] = Helper.GetResource("ProfileChangesSaved"); //"Profile changes saved.";

            return View(updatedProfile);
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

        // See http://stackoverflow.com/questions/3181500/respond-to-http-head-requests-using-asp-net-mvc
        // This child action is called and the parent verb is passed through.
        // Noticed in the logs that something (probably Akamai) makes HEAD requests to MIC pages - this caused an exception A public action method 'EmailNewsletterSettings' was not found.
        // because originaly this action was contrained ONLY top HttpGet.
        [AcceptVerbs(HttpVerbs.Get| HttpVerbs.Head)]
        public ActionResult EmailNewsletterSettings()
        {
            Logger.Debug("GET request for EmailNewsletterSettings triggered");
            return DefaultEmailNewsletterView();
        }

        /// <summary>
        /// This action is trigger for ANY postback as its a child of the Footer child action which is on EVERY page via the master view.
        /// This means we can't simply assume it was the newsletter form that triggered the postback
        /// </summary>
        /// <param name="emailNewsLetter"></param>
        /// <param name="EmailNewsletterSubmit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailNewsletterSettings(EmailNewsletter emailNewsLetter, string EmailNewsletterSubmit, string redirectUrl)
        {
            Boolean isAjax = Request.IsAjaxRequest();
            Boolean success = false;
            Boolean allowRedirect = false;
            String redirect = String.Empty;
            String message = Helper.GetResource("ProfileNotLoggedIn"); //"Looks like you're not logged in, please login and try again.";
            

            Logger.Debug("POST request for EmailNewsletterSettings triggered");
            Logger.DebugFormat("emailNewsLetter {0} & EmailNewsletterSubmit {1}", (emailNewsLetter == null), EmailNewsletterSubmit);

            // Check that user is even logged in AND the submit button has passed a value
            if (User.Identity.IsAuthenticated && !String.IsNullOrEmpty(EmailNewsletterSubmit))
            {
                // Get the profile and update keywords
                // Use th overload that passes email as it avoids the Memebership call
                var coatsUserProfile = CoatsUserProfile.GetProfile(HttpContext.User.Identity.Name);

                Logger.DebugFormat("coatsUserProfile? {0}", (coatsUserProfile == null));
                if (coatsUserProfile != null)
                {
                    // A complete list of techniques, marked true or false if selected
                    if (emailNewsLetter != null)
                        emailNewsLetter.Techniques.ForEach(t =>
                                                               {
                                                                   // If false (so not selected), find any match in current profile and remove
                                                                   if (!t.Selected)
                                                                   {                          
                                                                       foreach (var match in coatsUserProfile.Keywords.Where(k => k.Key == t.Value).ToList())
                                                                       {
                                                                           Logger.DebugFormat("Removing {0} {1}", match.Key, match.Value);
                                                                           coatsUserProfile.Keywords.Remove(match.Key);
                                                                       }
                                                                   }

                                                                   if (t.Selected && !coatsUserProfile.Keywords.ContainsKey(t.Value))
                                                                   {
                                                                       Logger.DebugFormat("Adding {0}", t.Value);
                                                                       coatsUserProfile.Keywords.Add(t.Value, "tcm");
                                                                   }
                                                               });

                    coatsUserProfile.Save();
                }

                success = true;
                message = Helper.GetResource("PreferencesUpdated"); //Thanks, your preferences have been updated.";

                redirectUrl = !String.IsNullOrEmpty(redirectUrl) ? redirectUrl : _settings.EmailNewsLetterConfirmation;

                Logger.DebugFormat("Redirecting to {0}", redirectUrl);

                if (!isAjax)
                {
                    return Redirect(redirectUrl);
                }
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = success,
                    allowRedirect = allowRedirect,
                    redirect = redirect,
                    message = message
                });
            }

            return DefaultEmailNewsletterView();
        }

        private ActionResult DefaultEmailNewsletterView()
        {
            // Use an empty object as default
            UserProfile userProfile = new UserProfile();
            EmailNewsletter emailNewsLetter = new EmailNewsletter();

            if (User.Identity.IsAuthenticated)
            {
                // Reset based on logged in user
                userProfile = GetModel();
                if (userProfile != null)
                {
                    emailNewsLetter.IsLoggedIn = true;
                    emailNewsLetter.EmailAddress = userProfile.CustomerDetails.EmailAddress;
                }
            }

            // Populate Techniques list
            string listTechniques = string.Format(_settings.CraftListCategory, PublicationId);
            var keywords = _keywordrepository.GetKeywordsList(listTechniques);
            //FacetSection facetTechniques = null;
            Location loc = null;

            if (!emailNewsLetter.IsLoggedIn)
            {
                //if (Session["FACETED_CONTENT"] != null)
                //{
                //    /*
                //     * 
                //     * Check if faceted content object in session is of type facetedcontent. 
                //     * We can't apply this functionality when on the product explorer.
                //     * 
                //     */
                //    var content = Session["FACETED_CONTENT"] as FacetedContent;
                //    if (content != null)
                //    {
                //        FacetedContent savedFacetedContent = content;
                //        try
                //        {
                //            // PLEASE NOTE: At the time of writing, the translation of the section title takes place in the View.
                //            //              Therefore, the section title here is the same as that in FredHopper. i.e. In plain English.
                //            //              If the translation were to move to the Controller, then you will need to implement the
                //            //              commented out code underneath, which uses the Helper.GetResource("techniques").
                //            facetTechniques = savedFacetedContent.FacetMap.FacetSections.SingleOrDefault(f => f.SectionTitle.ToLower() == "techniques");
                //            //facetTechniques = savedFacetedContent.FacetMap.FacetSections.SingleOrDefault(f => f.SectionTitle == Helper.GetResource("techniques"));
                //        }
                //        catch (Exception) { /* Can't find the section, so just let things follow the default behaviour */ }
                //    }
                //}

                // Use the query string to parse the selected techniques, rather than use a session value.
                try
                {
                    string[] fh_params = HttpContext.Request.QueryString["fh_params"].Split('&');
                    string fh_location = fh_params.SingleOrDefault(f => f.StartsWith("fh_location=")).Replace("fh_location=", "");
                    loc = new Location(fh_location);
                }
                catch (Exception) { loc = null; } 

            }

            foreach (var keyword in keywords)
            {
                // Selected "true" by default
                SelectListItem sli = new SelectListItem { Text = keyword.Description, Value = keyword.Id, Selected = true };
                if (emailNewsLetter.IsLoggedIn)
                {
                    if (userProfile != null)
                    {
                        var existing = userProfile.CraftTypeList.SingleOrDefault(c => c.Value == sli.Value);
                        sli.Selected = existing != null && existing.Selected;
                    }
                }
                else if (loc != null)
                {
                    try
                    {
                        // Only set the selected values if there is at least one technique facet selected.
                        var criteria = loc.getCriteria("techniques");
                        if (criteria.size() > 0)
                        {
                            //sli.Selected = criteria.ToString().Contains(string.Format("techniques__{0}", sli.Text.ToLower().Replace(" ", "_")));
                            sli.Selected = criteria.ToString().Contains(string.Format("techniques{0}{1}", FacetedContentHelper.NestedLevelDelimiter, sli.Text.ToLower().Replace(" ", "_")));
                        }
                    }
                    catch (Exception)
                    {
                        // And do what, exactly???  Probably nothing, just carry on regardless...
                    }

                }
                
                emailNewsLetter.Techniques.Add(sli);

                emailNewsLetter.Techniques.Count(t => t.Selected);
            }

            return View("~/Views/Partials/EmailNewsletter.cshtml", emailNewsLetter);
        }


        private UserProfile GetModel()
        {
            UserProfile userProfile = null;

            var user = Membership.GetUser();

            if (user != null)
            {
                CoatsUserProfile ctsUserProfile = ConfigurationManager.AppSettings["SiteIdentifier"] != null ? CoatsUserProfile.GetProfile(user.Email, ConfigurationManager.AppSettings["SiteIdentifier"]) : CoatsUserProfile.GetProfile(user.Email);

                userProfile = MapUserProfile(ctsUserProfile, user.Email);

                // ASH : see comment below
                GetModelData(userProfile);
            }

            // ASH : Moved up inside the braces;
            //GetModelData(userProfile);

            return userProfile;
        }


        private void GetModelData(UserProfile userProfile)
        {
            userProfile.CraftTypeList = GetKeywordsSelectList(userProfile, _keywordrepository.GetKeywordsList(_craftlistidentifier));
            userProfile.EmailNewsletter = GetKeywordsSelectList(userProfile, _keywordrepository.GetKeywordsList(_emailnewsletteridentifier));
            userProfile.ProfileVisible = GetKeywordsSelectList(userProfile, _keywordrepository.GetKeywordsList(_profilevisibleidentifier));
        }


        //need a private method to validate the edit as not all fields are required
        private bool IsEditModelValid(IUserProfile model)
        {
            var isValid = true;

            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.CustomerDetails.FirstName))
                    {
                        isValid = false;
                    }

                    if (string.IsNullOrEmpty(model.CustomerDetails.LastName))
                    {
                        isValid = false;
                    }
                    //if (string.IsNullOrEmpty(model.CustomerDetails.DisplayName))
                    //{
                    //    isValid = false;
                    //}
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception exception)
            {

                Logger.DebugFormat("IsEditModelValid exception >>>> ", exception);
            }

            return isValid;
        }

        private void MapUserProfileForUpdate(UserProfile model, CoatsUserProfile userProfile)
        {
            Logger.DebugFormat("MapUserProfileForUpdate");

            try
            {
                userProfile.NAME = model.CustomerDetails.FirstName;
                userProfile.SURNAME = model.CustomerDetails.LastName;
                userProfile.TELEPHONE = model.CustomerDetails.TelephoneNumber;
                //userProfile.DISPLAYNAME = model.CustomerDetails.DisplayName;
                userProfile.ABOUT = model.CustomerDetails.About;
                userProfile.LONG = model.CustomerDetails.Long;
                userProfile.LAT = model.CustomerDetails.Lat;
                userProfile.Keywords = model.Keywords;
                //userProfile.MAIL = model.CustomerDetails.EmailAddress;
                userProfile.POSTCODE = model.AddressDetails.Postcode;
                userProfile.PASSWORD = model.CustomerDetails.Password;
            }
            catch (Exception ex)
            {
                Logger.DebugFormat("MapUserProfileForUpdate ex {0}", ex.Message);
            }
        }

        private UserProfile MapUserProfile(CoatsUserProfile ctsUserProfile, string userEmail)
        {
            var userProfile = new UserProfile
            {
                CustomerDetails =
                {
                    FirstName = ctsUserProfile.NAME,
                    LastName = ctsUserProfile.SURNAME,
                    TelephoneNumber = ctsUserProfile.TELEPHONE,
                    EmailAddress = userEmail,
                    //VerifyEmailAddress = userEmail,
                    Password = ctsUserProfile.PASSWORD,
                    VerifyPassword = ctsUserProfile.PASSWORD,
                    DisplayName = ctsUserProfile.DISPLAYNAME,
                    About = ctsUserProfile.ABOUT,
                    Long = ctsUserProfile.LONG,
                    Lat = ctsUserProfile.LAT,
                },
                AddressDetails =
                {
                    BuildingNameNumber = ctsUserProfile.BUILDING,
                    City = ctsUserProfile.CITY,
                    Postcode = ctsUserProfile.POSTCODE,
                    Street = ctsUserProfile.STREET
                },
                Keywords = ctsUserProfile.Keywords
            };

            return userProfile;
        }

        private static List<SelectListItem> GetKeywordsSelectList(UserProfile model, IEnumerable<Coats.Crafts.Models.Keyword> keywords)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            List<int> keywordTcmList = new List<int>();

            TridionTcmUri tcm = new TridionTcmUri();

            // We need to convert the list of full TCM IDs into a list of TCM ITEM IDs
            foreach (var key in model.Keywords)
            {
                tcm = UtilityHelper.GetTcmUri(key.Key);
                keywordTcmList.Add(tcm.TcmItemId);
            }

            TridionTcmUri componentItem = new TridionTcmUri();

            foreach (var item in keywords)
            {
                componentItem = UtilityHelper.GetTcmUri(item.Id);

                //if (model.Keywords.ContainsKey(item.Id))
                selectList.Add(keywordTcmList.Contains(componentItem.TcmItemId)
                                   ? new SelectListItem {Text = item.Description, Value = item.Id, Selected = true}
                                   : new SelectListItem {Text = item.Description, Value = item.Id});
            }
            return selectList.OrderBy(x => x.Text).ToList();
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


        private ViewResult FindPostcode(UserProfile model)
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
                    }
                    else
                    {
                        model.RegistrationStatus = "postcode invalid";
                        ModelState.AddModelError(string.Empty, Helper.GetResource("PostcodeError")); //Errors.SendEmailError
                    }
                }
            }

            return View(model);
        }

    }
}
