namespace Coats.Crafts
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Coats.Crafts.Attributes;
    using Coats.Crafts.Plumbing;
    using DataAnnotationsExtensions.ClientValidation.Adapters;
    using LowercaseRoutesMVC;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    public class MvcApplication : HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer container;

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = base.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if ((cookie != null) && (cookie.Value != string.Empty))
            {
                FormsAuthenticationTicket ticket;
                try
                {
                    ticket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch
                {
                    return;
                }
                if ((HttpContext.Current.User != null) && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = HttpContext.Current.User.Identity as FormsIdentity;
                    if (identity != null)
                    {
                        string[] strArray = ticket.UserData.Split(new char[] { '|' });
                        CraftsPrincipal principal = new CraftsPrincipal(identity) {
                            DISPLAYNAME = strArray[0],
                            Firstname = strArray[1],
                            Lastname = strArray[2],
                            Longitude = strArray[3],
                            Latitude = strArray[4],
                            COUNTRY = strArray[5]
                        };
                        base.Context.User = principal;
                    }
                }
            }
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        protected void Application_PreSendRequestHeaders()
        {
            base.Response.Headers.Remove("Server");
            base.Response.Headers.Remove("X-AspNet-Version");
            base.Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);
            BootstrapContainer();
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(CustomEmailAttribute), typeof(EmailAttributeAdapter));
        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            container.Install(new IWindsorInstaller[] { FromAssembly.This() });
            container.Install(new IWindsorInstaller[] { Configuration.FromAppConfig() });
            WindsorControllerFactory controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom == "facet")
            {
                string str = context.Request.Url.ToString();
                if (!str.ToLower().Contains("fh_params") && ((context.Request.Form != null) && context.Request.Form.AllKeys.Contains<string>("ComponentSection.SearchTerm")))
                {
                    str = str + "|" + context.Request.Form["ComponentSection.SearchTerm"].ToString();
                }
                return str;
            }
            return base.GetVaryByCustomString(context, custom);
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            Bundle bundle = new StyleBundle("~/css/boostrap").Include(new string[] { "~/css/bootstrap*" });
            Bundle bundle2 = new StyleBundle("~/css/commonstyles").Include(new string[] { "~/css/rateit.css", "~/css/customScroller.css", "~/css/style.css", "~/css/structure.css", "~/css/iconography.css", "~/css/anchor.css", "~/css/freespirit.css", "~/css/rowan.css", "~/css/redheart.css", "~/css/schachenmayr.css" });
            Bundle bundle3 = new StyleBundle("~/css/printshopping").Include(new string[] { "~/css/style.css", "~/css/structure.css", "~/css/printshopping.css" });
            string cdnPath = "http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js";
            Bundle bundle4 = new ScriptBundle("~/jquery", cdnPath).Include(new string[] { "~/js/vendor/jquery-{version}.js" });
            Bundle bundle5 = new ScriptBundle("~/modernizr").Include(new string[] { "~/js/vendor/modernizr-{version}.js" });
            Bundle bundle6 = new ScriptBundle("~/commonscripts").Include(new string[] { 
                "~/js/vendor/jquery-ui-{version}.js", "~/js/vendor/jquery.validate*", "~/js/vendor/jquery.validate.unobtrusive*", "~/js/vendor/jquery.isotope*", "~/js/vendor/jquery.rateit*", "~/js/vendor/jquery.customScroller*", "~/js/vendor/mvcfoolproof.unobtrusive*", "~/js/vendor/jquery.scrolldepth*", "~/js/tracking.js", "~/js/bootstrap*", "~/js/helper.*", "~/js/main.js", "~/js/plugins.js", "~/js/richmarker-compiled.js", "~/js/infobox.js", "~/js/vendor/jquery.tablesorter.js",
                "~/js/vendor/jquery-migrate-{version}.js", "~/js/native.history.js"
            });
            bundles.Add(bundle);
            bundles.Add(bundle2);
            bundles.Add(bundle3);
            bundles.Add(bundle4);
            bundles.Add(bundle5);
            bundles.Add(bundle6);
            bundles.UseCdn = true;
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = "(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*allgif}", new { allgif = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*allpng}", new { allpng = @".*\.png(/.*)?" });
            routes.IgnoreRoute("{*allhtc}", new { allhtc = @".*\.htc(/.*)?" });
            routes.IgnoreRoute("{*allmp3}", new { allmp3 = @".*\.mp3(/.*)?" });
            routes.IgnoreRoute("{*allpdf}", new { allpdf = @".*\.pdf(/.*)?" });
            routes.MapRouteLowercase("remoteValidationEmail", "RemoteValidation/IsEmailAvailable", new { 
                controller = "RemoteValidation",
                action = "IsEmailAvailable"
            });
            routes.MapRouteLowercase("remoteValidationUser", "RemoteValidation/IsValidUser", new { 
                controller = "RemoteValidation",
                action = "IsValidUser"
            });
            routes.MapRouteLowercase("remoteValidationDisplayName", "RemoteValidation/IsDisplayNameAvailable", new { 
                controller = "RemoteValidation",
                action = "IsDisplayNameAvailable"
            });
            routes.MapRouteLowercase("Resources", "resources/{*resxFileName}", new { 
                controller = "Resources",
                action = "GetResourcesJavaScript"
            });
            routes.MapRouteLowercase("GoogleSiteMap", "googlesitemap.xml", new { 
                controller = "GoogleSiteMap",
                action = "Index"
            });
            routes.MapRouteLowercase("ajaxPassword", "ajax/PasswordReminder", new { 
                controller = "PasswordReminder",
                action = "Index"
            });
            routes.MapRouteLowercase("Password", "PasswordReminder/index", new { 
                controller = "PasswordReminder",
                action = "Index"
            });
            routes.MapRouteLowercase("AjaxEmailShopping", "ajax/shoppinglist", new { 
                controller = "ShoppingList",
                action = "Index"
            }, new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) });
            routes.MapRouteLowercase("AjaxNewsletterUpdate", "ajax/profile/emailnewslettersettings", new { 
                controller = "Profile",
                action = "EmailNewsletterSettings"
            }, new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) });
            routes.MapRouteLowercase("Ratings", "comments/AddRating", new { 
                controller = "Comments",
                action = "AddRating"
            });
            routes.MapRouteLowercase("CountrySelector", "CountrySelector/RedirectCountry", new { 
                controller = "CountrySelector",
                action = "RedirectCountry"
            });
            routes.MapRouteLowercase("Comments", "comments/AddComment", new { 
                controller = "Comments",
                action = "AddComment"
            });
            routes.MapRouteLowercase("ReturnComments", "comments/Comments", new { 
                controller = "Comments",
                action = "Comments"
            });
            routes.MapRouteLowercase("UserComments", "comments/Comments", new { 
                controller = "Comments",
                action = "GetCommentsByUser"
            });
            routes.MapRouteLowercase("ShoppingListInsertItems", "ShoppingList/ShoppingListInsertItems", new { 
                controller = "ShoppingList",
                action = "ShoppingListInsertItems"
            });
            routes.MapRouteLowercase("ShoppingListAddItem", "ShoppingList/AddItem", new { 
                controller = "ShoppingList",
                action = "AddItem"
            });
            routes.MapRouteLowercase("ShoppingListDeleteItem", "shoppinglist/DeleteItem", new { 
                controller = "ShoppingList",
                action = "Deleteitem"
            });
            routes.MapRouteLowercase("ShoppingListEmail", "shoppinglist/EmailShoppingList", new { 
                controller = "ShoppingList",
                action = "EmailShoppingList"
            });
            routes.MapRouteLowercase("ShoppingListDeleteProject", "shoppinglist/DeleteProject", new { 
                controller = "ShoppingList",
                action = "DeleteProject"
            });
            routes.MapRouteLowercase("ShoppingListChangeQuantity", "shoppinglist/ChangeQuantity", new { 
                controller = "ShoppingList",
                action = "ChangeQuantity"
            });
            routes.MapRouteLowercase("Login", "login/index", new { 
                controller = "Login",
                action = "Index"
            });
            routes.MapRouteLowercase("Profile", "profile/index", new { 
                controller = "Profile",
                action = "index"
            });
            routes.MapRouteLowercase("EditProfile", "profile/editprofile", new { 
                controller = "Profile",
                action = "Edit"
            });
            routes.MapRouteLowercase("Neils Test", "this/is/a/test", new { 
                controller = "Hybrid",
                action = "Post"
            });
            routes.MapRouteLowercase("Logout", "logout/", new { 
                controller = "Logout",
                action = "Index"
            });
            routes.MapRouteLowercase("Register", "registration/index", new { 
                controller = "registration",
                action = "Index"
            });
            routes.MapRouteLowercase("ajaxRegister", "ajax/registration/index", new { 
                controller = "registration",
                action = "Index"
            });
            routes.MapRouteLowercase("PostedComments", "comments/post", new { 
                controller = "comments",
                action = "Post"
            });
            routes.MapRouteLowercase("DeleteScrapbook", "scrapbook/delete", new { 
                controller = "Scrapbook",
                action = "Delete"
            });
            routes.MapRouteLowercase("AddItemScrapbook", "scrapbook/AddItem", new { 
                controller = "Scrapbook",
                action = "AddItem"
            });
            routes.MapRouteLowercase("FacetedContent", "facetedcontent/", new { 
                controller = "FacetedContent",
                action = "Index"
            });
            routes.MapRouteLowercase("Events", "events/index", new { 
                controller = "Events",
                action = "Index"
            });
            routes.MapRouteLowercase("Footer", "Footer/index", new { 
                controller = "Footer",
                action = "Index"
            });
            routes.MapRouteLowercase("EmailNewsletterSignup", "Registration/EmailNewsletterSignup", new { 
                controller = "Registration",
                action = "EmailNewsletterSignup"
            });
            routes.MapRouteLowercase("EmailNewsletterSettings", "Profile/EmailNewsletterSettings", new { 
                controller = "Profile",
                action = "EmailNewsletterSettings"
            });
            routes.MapRouteLowercase("Cats", "CATS/index", new { 
                controller = "CATS",
                action = "Index"
            });
            routes.MapRouteLowercase("TridionPageXml", "xml/{*PageId}", new { 
                controller = "TridionPage",
                action = "Xml"
            }, new { pageId = "^(.*)?$" });
            routes.MapRouteLowercase("TridionPage", "{*PageId}", new { 
                controller = "TridionPage",
                action = "Page"
            }, new { pageId = "^(.*)?$" });
            routes.MapRouteLowercase("Default", "{controller}/{action}/{id}", new { 
                controller = "Home",
                action = "Index",
                id = UrlParameter.Optional
            });
        }

        IWindsorContainer IContainerAccessor.Container
        {
            get
            {
                return container;
            }
        }

        public class CraftsPrincipal : GenericPrincipal
        {
            public CraftsPrincipal(IIdentity identity) : base(identity, new string[0])
            {
            }

            public string COUNTRY { get; set; }

            public string DISPLAYNAME { get; set; }

            public string Firstname { get; set; }

            public string Lastname { get; set; }

            public string LAT
            {
                get
                {
                    return this.Latitude;
                }
            }

            public string Latitude { get; set; }

            public string LONG
            {
                get
                {
                    return this.Longitude;
                }
            }

            public string Longitude { get; set; }

            public string NAME
            {
                get
                {
                    return this.Firstname;
                }
            }

            public string UserName
            {
                get
                {
                    return base.Identity.Name;
                }
            }
        }
    }
}

