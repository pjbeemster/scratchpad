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

    public class CustomCompareAttribute :  CompareAttribute, IClientValidatable
    {

        // Fields
        private readonly string _resourceName;
        
        // Methods
        public CustomCompareAttribute(string resourceName, string compareAttribute)
            : base(compareAttribute)
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

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    yield return new ModelClientValidationRequiredRule(FormatErrorMessage(_resourceName));
        //    //yield return new ModelClientValidationRule();
        //}



        //// Fields
        //private readonly string _resourceName;
        //private readonly int _maximumLength;
        //public string _compareAttribute;

        //// Methods
        //public CustomCompareAttribute(string resourceName, string compareAttribute)
        //{
        //    this._resourceName = resourceName;
        //    this._compareAttribute = compareAttribute; 
        //}

        //public string ResourceName
        //{
        //    get
        //    {
        //        return this._resourceName;
        //    }
        //}

        //public string CompareAttribute
        //{
        //    get
        //    {
        //        return this._compareAttribute;
        //    }
        //}

        //protected override ValidationResult IsValid
        //(object value, ValidationContext validationContext)
        //{
        //    string error = FormatErrorMessage(_resourceName);

        //    if (value != null)
        //    {
        //        string val = value.ToString();

        //        //if (val.Length > MaximumLength)
        //        //{
        //        //    return new ValidationResult(error);
        //        //}

        //        //if (val.Length < MinimumLength)
        //        //{
        //        //    return new ValidationResult(error);
        //        //}

        //        return ValidationResult.Success;

        //    }
        //    return new ValidationResult(error);
        //}

        //public override string FormatErrorMessage(string name)
        //{
        //    if (!string.IsNullOrEmpty(this._resourceName))
        //    {
        //        string label = HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, _resourceName) as string;
        //        return string.Format(label, new object[] { name });
        //    }
        //    else
        //    {
        //        return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, new object[] { name });
        //    }
        //}


        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    return new[] { new ModelClientValidationRequiredRule(FormatErrorMessage(metadata.DisplayName)) };
        //}


        //public class ModelClientValidationRequiredRule : ModelClientValidationRule
        //{
        //    public ModelClientValidationRequiredRule(string errorMessage)
        //    {
        //        ErrorMessage = errorMessage;
        //        ValidationType = "customcomparevalidation";
        //    }
        //}

    }

}