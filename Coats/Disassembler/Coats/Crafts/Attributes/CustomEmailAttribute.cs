namespace Coats.Crafts.Attributes
{
    using Coats.Crafts.Configuration;
    using DataAnnotationsExtensions;
    using System;
    using System.Globalization;
    using System.Web;

    public class CustomEmailAttribute : EmailAttribute
    {
        private readonly string _resourceName;

        public CustomEmailAttribute(string resourceName)
        {
            this._resourceName = resourceName;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(this._resourceName))
            {
                string globalResourceObject = HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, this._resourceName) as string;
                return string.Format(globalResourceObject, name);
            }
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name });
        }

        public string ResourceName
        {
            get
            {
                return this._resourceName;
            }
        }
    }
}

