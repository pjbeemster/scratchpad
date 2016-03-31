using System.Web.Security;
using Coats.IndustrialPortal.Providers;
using System.Web;
using System;

namespace Coats.Crafts.ControllerHelpers
{

    public class ProfileHelper
    {
        private const string UserSessionKey = "UserObject";

        public static CoatsUserProfile GetUser()
        {
            // Can only get a profile for a logged in user
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                // Name should contain the "username" in our case is email which is part of the FormsAuthentication cookie
                return CoatsUserProfile.GetProfile(HttpContext.Current.User.Identity.Name);

            //var mUser = Membership.GetUser();

            //CoatsUserProfile user = new CoatsUserProfile();

            //if (mUser != null)
            //{
            //    try
            //    {
            //        return GetCachedUser();
            //    }
            //    catch (Exception)
            //    {
            //        user = CoatsUserProfile.GetProfile(mUser.Email);
            //        CacheUser(user);
            //        return user;
            //    }
            //}

            return new CoatsUserProfile();
        }


        private static CoatsUserProfile GetCachedUser()
        {
            CoatsUserProfile theUser = (CoatsUserProfile)HttpContext.Current.Session[UserSessionKey];

            if (theUser == null)
                throw new Exception("Cached user not valid");
            return theUser;
        }


        private static void CacheUser(CoatsUserProfile theUser)
        {
            if (theUser == null)
                throw new Exception("User to cache not valid");

            HttpContext.Current.Session[UserSessionKey] = theUser;
        }

    }
}