using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Coats.Crafts.Configuration;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace Coats.Crafts.Attributes
{
    /// <summary>
    /// RequiredAttribute that pulls the error message from a xml file using LabelExpressionBuilder.GetXmlSetting
    /// </summary>
    /// <remarks>
    /// </remarks>

    public class CustomEmailAttribute :  EmailAttribute
    {

        // Fields
        private readonly string _resourceName;
        
        // Methods
        public CustomEmailAttribute(string resourceName)
        {
            this._resourceName = resourceName;
        }
        
        public string ResourceName
        {
            get
            {
                return this._resourceName;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(this._resourceName))
            {
                string label = HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, _resourceName) as string;
                return string.Format(label, name);
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, new object[] { name });
            }
        }

    }

}