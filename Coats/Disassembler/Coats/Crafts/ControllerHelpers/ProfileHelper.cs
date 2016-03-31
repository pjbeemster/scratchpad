namespace Coats.Crafts.ControllerHelpers
{
    using Coats.IndustrialPortal.Providers;
    using System;
    using System.Web;

    public class ProfileHelper
    {
        private const string UserSessionKey = "UserObject";

        private static void CacheUser(CoatsUserProfile theUser)
        {
            if (theUser == null)
            {
                throw new Exception("User to cache not valid");
            }
            HttpContext.Current.Session["UserObject"] = theUser;
        }

        private static CoatsUserProfile GetCachedUser()
        {
            CoatsUserProfile profile = (CoatsUserProfile) HttpContext.Current.Session["UserObject"];
            if (profile == null)
            {
                throw new Exception("Cached user not valid");
            }
            return profile;
        }

        public static CoatsUserProfile GetUser()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return CoatsUserProfile.GetProfile(HttpContext.Current.User.Identity.Name);
            }
            return new CoatsUserProfile();
        }
    }
}

