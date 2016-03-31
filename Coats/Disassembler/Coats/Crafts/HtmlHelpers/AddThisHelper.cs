namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;
    using System.Xml;

    public static class AddThisHelper
    {
        public static HtmlString addThis(this HtmlHelper helper, string label)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XmlElement element = document.SelectSingleNode("/configuration/Coats.Crafts." + label) as XmlElement;
                return new HtmlString(element.InnerText);
            }
            catch (Exception)
            {
                return new HtmlString("");
            }
        }
    }
}

