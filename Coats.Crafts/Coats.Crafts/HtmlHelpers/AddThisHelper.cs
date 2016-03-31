using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Xml;

namespace Coats.Crafts.HtmlHelpers
{
    public static class AddThisHelper
    {

        /// <summary>
        /// Returns the addThis script block which is stored in the web config
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        public static HtmlString addThis(this HtmlHelper helper, string label)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XmlElement node = doc.SelectSingleNode("/configuration/Coats.Crafts." + label) as XmlElement;

                return new HtmlString(node.InnerText);
            }
            catch(Exception ex)
            {
                return new HtmlString("");
            }
        }


    }
}