using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;

namespace MvcCustomValidation.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class MyCustomValidation : ValidationAttribute, IClientValidatable
    {
        public string CommaSeperatedProperties { get; private set; }
        public MyCustomValidation(string commaSeperatedProperties)
            : base("Please select an option XX.")
        {
            if (!string.IsNullOrEmpty(commaSeperatedProperties))
                CommaSeperatedProperties = commaSeperatedProperties;
        }
        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string[] strProperties =
        CommaSeperatedProperties.Split(new char[] { ',' });
                bool bIsAnyChecked = false;
                if (Convert.ToBoolean(value))
                {
                    bIsAnyChecked = true;
                }
                else
                {
                    foreach (string strProperty in strProperties)
                    {
                        var curProperty = validationContext.ObjectInstance.GetType().
                    GetProperty(strProperty);
                        var curPropertyValue = curProperty.GetValue
                (validationContext.ObjectInstance, null);
                        if (Convert.ToBoolean(curPropertyValue))
                        {
                            bIsAnyChecked = true;
                            break;
                        }
                    }
                }
                if (!bIsAnyChecked)
                    return new ValidationResult("Please select an option.");
            }
            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule>
        GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[] { new ModelClientValidationSelectOneRule
            (FormatErrorMessage(metadata.DisplayName), 
        CommaSeperatedProperties.Split(new char[] { ',' })) };
        }
        public class ModelClientValidationSelectOneRule : ModelClientValidationRule
        {
            public ModelClientValidationSelectOneRule
        (string errorMessage, string[] strProperties)
            {
                ErrorMessage = errorMessage;
                ValidationType = "mycustomvalidation";
                for (int intIndex = 0; intIndex < strProperties.Length; intIndex++)
                {
                    ValidationParameters.Add("otherproperty" +
                    intIndex.ToString(), strProperties[intIndex]);
                }
            }
        }
    }
}