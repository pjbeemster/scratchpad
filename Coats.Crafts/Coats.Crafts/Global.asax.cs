using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Configuration;

using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.MicroKernel.Registration;
using Castle.Facilities.TypedFactory;

using Coats.Crafts.Plumbing;
using Coats.Crafts.Configuration;

using DataAnnotationsExtensions;
using DataAnnotationsExtensions.ClientValidation.Adapters;

using LowercaseRoutesMVC;
using System.Collections.Specialized;
using Coats.Crafts.Attributes;

namespace Coats.Crafts
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.IgnoreRoute("{*allgif}", new { allgif = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*allpng}", new { allpng = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*allhtc}", new { allhtc = @".*\.htc(/.*)?" });
            routes.IgnoreRoute("{*allmp3}", new { allmp3 = @".*\.mp3(/.*)?" });
            routes.IgnoreRoute("{*allpdf}", new { allpdf = @".*\.pdf(/.*)?" });

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "remoteValidationEmail", // Route name
                "RemoteValidation/IsEmailAvailable", // URL with parameters
                new { controller = "RemoteValidation", action = "IsEmailAvailable" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "remoteValidationUser", // Route name
                "RemoteValidation/IsValidUser", // URL with parameters
                new { controller = "RemoteValidation", action = "IsValidUser" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "remoteValidationDisplayName", // Route name
                "RemoteValidation/IsDisplayNameAvailable", // URL with parameters
                new { controller = "RemoteValidation", action = "IsDisplayNameAvailable" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "Resources",
                "resources/{*resxFileName}",
                new
                {
                    controller = "Resources",
                    action = "GetResourcesJavaScript"
                }
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "GoogleSiteMap",
                "googlesitemap.xml",
                new
                {
                    controller = "GoogleSiteMap",
                    action = "Index"
                }
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "ajaxPassword", // Route name
               "ajax/PasswordReminder", // URL with parameters
               new { controller = "PasswordReminder", action = "Index" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Password", // Route name
               "PasswordReminder/index", // URL with parameters
               new { controller = "PasswordReminder", action = "Index" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "AjaxEmailShopping", // Route name
               "ajax/shoppinglist", // URL with parameters
               new { controller = "ShoppingList", action = "Index" }, // Parameter defaults,
               new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) }
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "AjaxNewsletterUpdate", // Route name
               "ajax/profile/emailnewslettersettings", // URL with parameters
               new { controller = "Profile", action = "EmailNewsletterSettings" }, // Parameter defaults,
               new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) }
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Ratings", // Route name
               "comments/AddRating", // URL with parameters
               new { controller = "Comments", action = "AddRating" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "CountrySelector", // Route name
                "CountrySelector/RedirectCountry", // URL with parameters
                new { controller = "CountrySelector", action = "RedirectCountry" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Comments", // Route name
               "comments/AddComment", // URL with parameters
               new { controller = "Comments", action = "AddComment" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "ReturnComments", // Route name
               "comments/Comments", // URL with parameters
               new { controller = "Comments", action = "Comments" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "UserComments", // Route name
               "comments/Comments", // URL with parameters
               new { controller = "Comments", action = "GetCommentsByUser" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListInsertItems", // Route name
                "ShoppingList/ShoppingListInsertItems", // URL with parameters
                new { controller = "ShoppingList", action = "ShoppingListInsertItems" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListAddItem", // Route name
                "ShoppingList/AddItem", // URL with parameters
                new { controller = "ShoppingList", action = "AddItem" } // Parameter defaults
            );

            //routes.MapRoute(
            //    "ShoppingListPrint", // Route name
            //    "ShoppingList/ShoppingListPrint", // URL with parameters
            //    new { controller = "ShoppingList", action = "Print" } // Parameter defaults
            //);

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListDeleteItem", // Route name
                "shoppinglist/DeleteItem", // URL with parameters
                new { controller = "ShoppingList", action = "Deleteitem" } // Parameter defaults
            );
            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListEmail", // Route name
                "shoppinglist/EmailShoppingList", // URL with parameters
                new { controller = "ShoppingList", action = "EmailShoppingList" } // Parameter defaults
            );
            

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListDeleteProject", // Route name
                "shoppinglist/DeleteProject", // URL with parameters
                new { controller = "ShoppingList", action = "DeleteProject" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ShoppingListChangeQuantity", // Route name
                "shoppinglist/ChangeQuantity", // URL with parameters
                new { controller = "ShoppingList", action = "ChangeQuantity" } // Parameter defaults
            );


            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Login", // Route name
               "login/index", // URL with parameters
               new { controller = "Login", action = "Index" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Profile", // Route name
               "profile/index", // URL with parameters
               new { controller = "Profile", action = "index" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "EditProfile", // Route name
               "profile/editprofile", // URL with parameters
               //new { controller = "Hybrid", action = "Post" } // Parameter defaults
               new { controller = "Profile", action = "Edit" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Neils Test", // Route name
               "this/is/a/test", // URL with parameters
               new { controller = "Hybrid", action = "Post" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "Logout", // Route name
                "logout/", // URL with parameters
                new { controller = "Logout", action = "Index" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "Register", // Route name
                "registration/index", // URL with parameters
                new { controller = "registration", action = "Index" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "ajaxRegister", // Route name
                "ajax/registration/index", // URL with parameters
                new { controller = "registration", action = "Index" } // Parameter defaults
            );


            routes.MapRouteLowercase( // changed from routes.MapRoute
                "PostedComments", // Route name
                "comments/post", // URL with parameters
                new { controller = "comments", action = "Post" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "DeleteScrapbook", // Route name
                "scrapbook/delete", // URL with parameters
                new { controller = "Scrapbook", action = "Delete" } // Parameter defaults
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "AddItemScrapbook", // Route name
                "scrapbook/AddItem", // URL with parameters
                new { controller = "Scrapbook", action = "AddItem" } // Parameter defaults
            );
            
            routes.MapRouteLowercase( // changed from routes.MapRoute
                "FacetedContent", // Route name
                "facetedcontent/", // URL with parameters
                new { controller = "FacetedContent", action = "Index" } // Controller is actually "FacetedContent"
            );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Events", // Route name
               "events/index", // URL with parameters
               new { controller = "Events", action = "Index" } // Parameter defaults
           );


            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Footer", // Route name
               "Footer/index", // URL with parameters
               new { controller = "Footer", action = "Index" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "EmailNewsletterSignup", // Route name
               "Registration/EmailNewsletterSignup", // URL with parameters
               new { controller = "Registration", action = "EmailNewsletterSignup" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "EmailNewsletterSettings", // Route name
               "Profile/EmailNewsletterSettings", // URL with parameters
               new { controller = "Profile", action = "EmailNewsletterSettings" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "Cats", // Route name
               "CATS/index", // URL with parameters
               new { controller = "CATS", action = "Index" } // Parameter defaults
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "TridionPageXml", // Route name
               "xml/{*PageId}",
               new { controller = "TridionPage", action = "Xml" }, //Parameter defaults
               new { pageId = @"^(.*)?$" } //Parameter constraints
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
               "TridionPage", // Route name
               "{*PageId}",
               new { controller = "TridionPage", action = "Page" }, //Parameter defaults
               new { pageId = @"^(.*)?$" } //Parameter constraints
           );

            routes.MapRouteLowercase( // changed from routes.MapRoute
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bootstrap bundle5
            var bootstrapBundle = new StyleBundle("~/css/boostrap")
                                        .Include("~/css/bootstrap*");

            // Css bundle
            var cssBundle = new StyleBundle("~/css/commonstyles")
                                .Include(
                                    // "~/css/normalize.css"),
                                    "~/css/rateit.css",
                                    "~/css/customScroller.css",
                                    "~/css/style.css",
                                    "~/css/structure.css",
                                    "~/css/iconography.css",
                                    "~/css/anchor.css",
                                    "~/css/freespirit.css",
                                    "~/css/rowan.css",
                                    "~/css/redheart.css",
                                    "~/css/schachenmayr.css");

            // Print Css bundle
            var PrintShoppingCSSBundle = new StyleBundle("~/css/printshopping")
                                .Include(
                                    "~/css/style.css",
                                    "~/css/structure.css",
                                    "~/css/printshopping.css");

            // jQuery bundle using CDN
            var jqueryCdnPath = "http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js";
            var jqueryBundle = new ScriptBundle("~/jquery", jqueryCdnPath)
                                .Include("~/js/vendor/jquery-{version}.js");

            // modernizr bundle
            var modernizr = new ScriptBundle("~/modernizr")
                                .Include("~/js/vendor/modernizr-{version}.js");
                                
            // JS bundle for remaining scripts
            var jsBundle = new ScriptBundle("~/commonscripts")
                            .Include("~/js/vendor/jquery-ui-{version}.js",
                                    "~/js/vendor/jquery.validate*",
                                    "~/js/vendor/jquery.validate.unobtrusive*",
                                    "~/js/vendor/jquery.isotope*",
                                    "~/js/vendor/jquery.rateit*",
                                    "~/js/vendor/jquery.customScroller*",
                                    "~/js/vendor/mvcfoolproof.unobtrusive*",
                                    "~/js/vendor/jquery.scrolldepth*",
                                    "~/js/tracking.js",
                                    "~/js/bootstrap*",
                                    "~/js/helper.*",
                                    "~/js/main.js",
                                    "~/js/plugins.js",
                                    "~/js/richmarker-compiled.js",
                                    "~/js/infobox.js",
                                    "~/js/vendor/jquery.tablesorter.js",
                                    "~/js/vendor/jquery-migrate-{version}.js",
                                    "~/js/native.history.js"
                                    );

            bundles.Add(bootstrapBundle);
            bundles.Add(cssBundle);
            bundles.Add(PrintShoppingCSSBundle);
            bundles.Add(jqueryBundle);
            bundles.Add(modernizr);
            bundles.Add(jsBundle);
            
            bundles.UseCdn = true;
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_Start()
        {        
            // Don't send the MVC info in the header
            MvcHandler.DisableMvcResponseHeader = true;

            // Clears all previously registered view engines - so stops .NET always look for the WebFormViewEngine all the time when we only use Razor
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();

            // Register stuff
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);   

            // Initiate Windsor
            BootstrapContainer();
            DataAnnotationsModelValidatorProvider.RegisterAdapter(
        typeof(CustomEmailAttribute),
        typeof(EmailAttributeAdapter));

        }

        /// <summary>
        /// This is used in conjunction with the output cache attribute.
        /// This can be set up in the web.config under system.web/caching/outputCacheSettings/outputCacheProfiles.
        /// You can then decorate a controller action, or, indeed, a whole controller class with the OutputCache
        /// attribute, e.g. [OutputCache(CacheProfile = "FacetedContent")], where "FacetedContent" is the key to
        /// the web.config settings.
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <param name="custom">The string value of the varyByCustom attribute parameter</param>
        /// <returns></returns>
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "facet") // varyByCustom parameter
            {
                // First, get the URL as the basis for the key.
                string url = context.Request.Url.ToString();

                // Next, check if the url contains "fh_params".
                // The reason for this is because any search terms will have been applied in the query string.
                if(!url.ToLower().Contains("fh_params"))
                {
                    // Next, check if there is a search term, and apply it to the cache key string.
                    if (context.Request.Form != null && context.Request.Form.AllKeys.Contains("ComponentSection.SearchTerm"))
                    {
                        url = url + "|" + context.Request.Form["ComponentSection.SearchTerm"].ToString();
                    }
                }
                return url;
            }

            return base.GetVaryByCustomString(context, custom);
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        public class CraftsPrincipal : GenericPrincipal
        {
            public CraftsPrincipal(IIdentity identity) : base(identity, new string[] { }) { }
            public string DISPLAYNAME { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Longitude { get; set; }
            public string Latitude { get; set; }
            public string UserName { get { return base.Identity.Name;  } }
            public string LAT { get { return Latitude; } }
            public string LONG { get {return Longitude;  }}
            public string NAME { get { return Firstname; } }
            public string COUNTRY { get; set; }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty(WebConfiguration.Current.FakeRoles))
            //{
            //    GenericPrincipal userPrincipal =
            //                        new GenericPrincipal(
            //                            new GenericIdentity("username"), 
            //                            WebConfiguration.Current.FakeRoles.Split(",".ToCharArray()));

            //    Context.User = userPrincipal;
            //}


            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == string.Empty)
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }


            if (HttpContext.Current.User != null)
            {
                // see if this user is authenticated, any authenticated cookie (ticket) exists for this user
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // see if the authentication is done using FormsAuthentication
                    var identity = HttpContext.Current.User.Identity as FormsIdentity;

                    if (identity != null)
                    {

                        ////Get the roles stored as UserData into ticket
                        //string[] roles = { };
                        ////Create general prrincipal and assign it to current request
                        //HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(identity, roles);

                        // retrieve roles from UserData
                        //string[] roles = authTicket.UserData.Split(';');

                        //if (Context.User != null)
                        //    Context.User = new GenericPrincipal(Context.User.Identity, roles);

                        string[] props =  authTicket.UserData.Split('|');

                        CraftsPrincipal user = new CraftsPrincipal(identity)
                        {
                            DISPLAYNAME = props[0],
                            Firstname = props[1],
                            Lastname = props[2],
                            Longitude = props[3],
                            Latitude = props[4],
                            COUNTRY = props[5]
                        };

                        Context.User = user;
                    }
                }
            }

        }

        private static IWindsorContainer container;

        private static void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            container.Install(FromAssembly.This());
            
            container.Install(
                    Castle.Windsor.Installer.Configuration.FromAppConfig());


            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

  
        #region IContainerAccessor Members

        IWindsorContainer IContainerAccessor.Container
        {
            get { return container; }
        }

        #endregion
    }


}