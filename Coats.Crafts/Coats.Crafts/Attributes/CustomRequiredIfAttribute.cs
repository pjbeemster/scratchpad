using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Coats.Crafts.Configuration;
using System.Web.Mvc;
using Foolproof;

namespace Coats.Crafts.Attributes
{
    /// <summary>
    /// RequiredAttribute that pulls the error message from a xml file using LabelExpressionBuilder.GetXmlSetting
    /// </summary>
    /// <remarks>
    /// </remarks>

    public class CustomRequiredIfAttribute :  RequiredIfNotRegExMatchAttribute, IClientValidatable
    {
        // Fields
        private readonly string _resourceName;
        
        // Methods
        public CustomRequiredIfAttribute(string resourceName, string dependentValue, string pattern)
            : base(dependentValue,pattern)
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

        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            string error = FormatErrorMessage(_resourceName);

            if (value != null)
            {
                string val = value.ToString();

                //if (val.Length > MaximumLength)
                //{
                //    return new ValidationResult(error);
                //}

                //if (val.Length < MinimumLength)
                //{
                //    return new ValidationResult(error);
                //}
            }
            return ValidationResult.Success;
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


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationRequiredRule(FormatErrorMessage(metadata.DisplayName)) };
        }


        public class ModelClientValidationRequiredRule : ModelClientValidationRule
        {
            public ModelClientValidationRequiredRule(string errorMessage)
            {
                ErrorMessage = errorMessage;
                ValidationType = "customrequiredifvalidation";
            }
        }

    }

}