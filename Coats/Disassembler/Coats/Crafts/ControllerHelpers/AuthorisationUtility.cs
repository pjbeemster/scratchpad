namespace Coats.Crafts.ControllerHelpers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.Security;

    public class AuthorisationUtility
    {
        public static bool GetComponentAccessInfo(IComponent model, out string accessTitle, out string parentKeyword)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            bool flag = false;
            ILogger instance = applicationInstance.Container.Resolve<ILogger>();
            accessTitle = string.Empty;
            parentKeyword = string.Empty;
            instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > model? {0}", new object[] { model != null });
            if (model != null)
            {
                instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > Any Access field? Access {0} access{1}", new object[] { model.MetadataFields.ContainsKey("Access"), model.MetadataFields.ContainsKey("access") });
                if (model.MetadataFields.ContainsKey("Access") || model.MetadataFields.ContainsKey("access"))
                {
                    IList<IKeyword> list = (model.MetadataFields["Access"].Keywords != null) ? model.MetadataFields["Access"].Keywords : model.MetadataFields["access"].Keywords;
                    if (list != null)
                    {
                        foreach (IKeyword keyword in list)
                        {
                            accessTitle = keyword.Description;
                            parentKeyword = GetParentKw(keyword.Path, keyword.Description);
                            break;
                        }
                    }
                    instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > accessTitle (after Keyword check) = {0}", new object[] { accessTitle });
                    if (accessTitle == string.Empty)
                    {
                        accessTitle = !string.IsNullOrEmpty(model.MetadataFields["Access"].Value) ? model.MetadataFields["Access"].Value : model.MetadataFields["access"].Value;
                        instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > accessTitle (after Field check) = {0}", new object[] { accessTitle });
                    }
                    MembershipUser user = Membership.GetUser();
                    instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > user? {0}", new object[] { user != null });
                    if (user != null)
                    {
                        string[] rolesForUser = Roles.GetRolesForUser(user.Email);
                        instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > user roles? {0}", new object[] { string.Join(", ", rolesForUser) });
                        if (rolesForUser.Length > 0)
                        {
                            string title = accessTitle;
                            instance.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > Checking if {0} appears in roles: {1}", new object[] { title, string.Join(",", rolesForUser) });
                            if (rolesForUser.Any<string>(role => role == title))
                            {
                                instance.Debug("AuthorisationUtility.GetComponentAccessInfo > role found! Access allowed!");
                                flag = true;
                            }
                        }
                    }
                }
            }
            applicationInstance.Container.Release(instance);
            return flag;
        }

        public static bool GetPageModelInfo(IPage model)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            ILogger instance = applicationInstance.Container.Resolve<ILogger>();
            bool flag = true;
            instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > model? {0}", new object[] { model != null });
            if (model != null)
            {
                instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > model.MetadataFields? {0}", new object[] { model.MetadataFields != null });
                if (model.MetadataFields != null)
                {
                    instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > model.MetadataFields.ContainsKey(\"access\")? {0}", new object[] { model.MetadataFields.ContainsKey("access") });
                    if (model.MetadataFields.ContainsKey("access"))
                    {
                        Func<string, bool> predicate = null;
                        string accessTitle = model.MetadataFields["access"].Keywords[0].Title;
                        instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > accessTitle {0}", new object[] { accessTitle });
                        MembershipUser user = Membership.GetUser();
                        instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > user? {0}", new object[] { user != null });
                        if (user != null)
                        {
                            string[] rolesForUser = Roles.GetRolesForUser(user.Email);
                            instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > user roles? {0}", new object[] { string.Join(", ", rolesForUser) });
                            if (rolesForUser.Length > 0)
                            {
                                instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > Checking if {0} appears in roles: {1}", new object[] { accessTitle, string.Join(",", rolesForUser) });
                                if (predicate == null)
                                {
                                    predicate = role => accessTitle == role;
                                }
                                flag = rolesForUser.Any<string>(predicate);
                            }
                        }
                        else
                        {
                            instance.Debug("AuthorisationUtility.GetPageModelInfo > No user, but access level defined, so return false");
                            flag = false;
                        }
                    }
                }
            }
            instance.DebugFormat("AuthorisationUtility.GetPageModelInfo > allowAccess {0}", new object[] { flag });
            applicationInstance.Container.Release(instance);
            return flag;
        }

        private static string GetParentKw(string p, string toolName)
        {
            string[] strArray = p.Split(new char[] { '\\' });
            int length = strArray.Length;
            return strArray[length - 2];
        }

        public static string GetReturnUrl(string returnUrl)
        {
            return string.Format("{0}?ReturnUrl={1}", WebConfiguration.Current.Login.AddApplicationRoot(), returnUrl);
        }

        public static bool IsUserAuth()
        {
            bool flag = false;
            if (Membership.GetUser() != null)
            {
                flag = true;
            }
            return flag;
        }
    }
}

