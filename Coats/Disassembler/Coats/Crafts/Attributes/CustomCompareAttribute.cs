namespace Coats.Crafts.Attributes
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;

    public class CustomCompareAttribute : CompareAttribute, IClientValidatable
    {
        private readonly string _resourceName;

        public CustomCompareAttribute(string resourceName, string compareAttribute) : base(compareAttribute)
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

