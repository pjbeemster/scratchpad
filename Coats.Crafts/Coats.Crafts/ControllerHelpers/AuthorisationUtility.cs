using System.Linq;
using System.Web;
using System.Web.Security;
using Castle.Core.Logging;
using Coats.Crafts.Extensions;
using DD4T.ContentModel;
//using Coats.IndustrialPortal.Configuration;
using System;

using Castle.Windsor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Coats.Crafts.Configuration;

namespace Coats.Crafts.ControllerHelpers
{
    public class AuthorisationUtility
    {
        /// <summary>
        /// method to determone if user has access to component.
        /// The assumption that the call is made indicates that the access needs to be granted.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="accessTitle"> </param>
        /// <returns></returns>
        public static bool GetComponentAccessInfo(IComponent model, out string accessTitle, out string parentKeyword)
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var allowAccess = false;
            var logger = accessor.Container.Resolve<ILogger>();

            accessTitle = string.Empty;
            parentKeyword = string.Empty;

            logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > model? {0}", model != null);
            if (model != null)
            {

                logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > Any Access field? Access {0} access{1}", model.MetadataFields.ContainsKey("Access"), model.MetadataFields.ContainsKey("access"));
                if (model.MetadataFields.ContainsKey("Access") || model.MetadataFields.ContainsKey("access"))
                {
                    IList<IKeyword> keys = (model.MetadataFields["Access"].Keywords != null) ? model.MetadataFields["Access"].Keywords : model.MetadataFields["access"].Keywords;
                    if (keys != null)
                    {
                        foreach (var kw in keys)
                        {
                            accessTitle = kw.Description;
                            parentKeyword = GetParentKw(kw.Path, kw.Description);
                            break;
                        }
                    }

                    logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > accessTitle (after Keyword check) = {0}", accessTitle);

                    //TEMP workaround to check that the value 
                    if (accessTitle == string.Empty)
                    {
                        accessTitle = (!string.IsNullOrEmpty(model.MetadataFields["Access"].Value)) ? model.MetadataFields["Access"].Value : model.MetadataFields["access"].Value;
                        logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > accessTitle (after Field check) = {0}", accessTitle);
                    }

                    var user = Membership.GetUser();
                    logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > user? {0}", user != null);
                    if (user != null)
                    {
                        string[] roles = Roles.GetRolesForUser(user.Email);
                        logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > user roles? {0}", String.Join(", ", roles));

                        if (roles.Length > 0)
                        {
                            var title = accessTitle;
                            logger.DebugFormat("AuthorisationUtility.GetComponentAccessInfo > Checking if {0} appears in roles: {1}", title, String.Join(",", roles));
                            if (roles.Any(role => role == title))
                            {
                                logger.Debug("AuthorisationUtility.GetComponentAccessInfo > role found! Access allowed!");
                                allowAccess = true;
                            }
                        }

                    }
                }
            }

            accessor.Container.Release(logger);

            return allowAccess;

        }

        private static string GetParentKw(string p, string toolName)
        {

            string filePath = @p;
            string[] keywords = filePath.Split('\\');
            int keywordsLength = keywords.Length;

            string pkw = keywords[keywordsLength - 2];
            return pkw;
        }

        public static bool IsUserAuth()
        {
            var isAuth = false;
            var user = Membership.GetUser();

            if (user != null)
            {
                isAuth = true;
            }
            return isAuth;
        }

        public static bool GetPageModelInfo(IPage model)
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            var logger = accessor.Container.Resolve<ILogger>();

            var allowAccess = true;

            logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > model? {0}", model != null);
            if (model != null)
            {
                logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > model.MetadataFields? {0}", model.MetadataFields != null);
                if (model.MetadataFields != null)
                {
                    logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > model.MetadataFields.ContainsKey(\"access\")? {0}", model.MetadataFields.ContainsKey("access"));
                    if (model.MetadataFields.ContainsKey("access"))
                    {
                        var accessTitle = model.MetadataFields["access"].Keywords[0].Title;
                        logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > accessTitle {0}", accessTitle);

                        var user = Membership.GetUser();
                        logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > user? {0}", user != null);
                        if (user != null)
                        {
                            string[] roles = Roles.GetRolesForUser(user.Email);
                            logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > user roles? {0}", String.Join(", ", roles));
                            if (roles.Length > 0)
                            {
                                logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > Checking if {0} appears in roles: {1}", accessTitle, String.Join(",", roles));
                                allowAccess = roles.Any(role => accessTitle == role);
                            }
                        }
                        else
                        {
                            logger.Debug("AuthorisationUtility.GetPageModelInfo > No user, but access level defined, so return false");
                            allowAccess = false;
                        }
                    }

                }
            }

            logger.DebugFormat("AuthorisationUtility.GetPageModelInfo > allowAccess {0}", allowAccess);
            accessor.Container.Release(logger);

            return allowAccess;
        }

        public static string GetReturnUrl(string returnUrl)
        {
            return string.Format("{0}?ReturnUrl={1}", WebConfiguration.Current.Login.AddApplicationRoot(), returnUrl);
        }


    }
}