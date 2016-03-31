using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

using Coats.Crafts.Configuration;
using System.Web.Mvc;

namespace Coats.Crafts.Attributes
{
    /// <summary>
    /// RequiredAttribute that pulls the error message from a xml file using LabelExpressionBuilder.GetXmlSetting
    /// </summary>
    /// <remarks>
    /// </remarks>

    public class ResourceRequiredAttribute : RequiredAttribute, IClientValidatable 
    {
        // Fields
        private readonly string _resourceName;

        // Methods
        //public ResourceRequiredAttribute(string resourceName)
        public ResourceRequiredAttribute(string resourceName)
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
                return string.Format(label, new object[] { name });
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, new object[] { name });
            }
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRequiredRule(FormatErrorMessage(_resourceName));
        }

        #endregion
    }

}