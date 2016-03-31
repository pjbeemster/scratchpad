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
    /// CustomStringLengthAttribute that pulls the error message from a xml file using LabelExpressionBuilder.GetXmlSetting
    /// </summary>
    /// <remarks>
    /// </remarks>

    public class CustomStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        // Fields
        private readonly string _resourceName;
        private readonly int _maximumLength;
        private readonly int _minimumLength;
        
        // Methods
        public CustomStringLengthAttribute(int minimumLength, string resourceName, int maximumLength)
            : base(minimumLength)
        {
            this._resourceName = resourceName;
            this._minimumLength = minimumLength;
            this._maximumLength = maximumLength;
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
                object[] args = new object[] { name, MinimumLength };
                try
                {
                    return string.Format(label, args);
                }
                catch (FormatException)
                {
                    return label;
                }
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, new object[] { name });
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationStringLengthRule(FormatErrorMessage(_resourceName),_maximumLength,_minimumLength);
        }

    }

}