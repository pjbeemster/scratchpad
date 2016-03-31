namespace MvcCustomValidation.Validations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true, Inherited=false)]
    public class MyCustomValidation : ValidationAttribute, IClientValidatable
    {
        public MyCustomValidation(string commaSeperatedProperties) : base("Please select an option XX.")
        {
            if (!string.IsNullOrEmpty(commaSeperatedProperties))
            {
                this.CommaSeperatedProperties = commaSeperatedProperties;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationSelectOneRule[] { new ModelClientValidationSelectOneRule(this.FormatErrorMessage(metadata.DisplayName), this.CommaSeperatedProperties.Split(new char[] { ',' })) };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string[] strArray = this.CommaSeperatedProperties.Split(new char[] { ',' });
                bool flag = false;
                if (Convert.ToBoolean(value))
                {
                    flag = true;
                }
                else
                {
                    foreach (string str in strArray)
                    {
                        if (Convert.ToBoolean(validationContext.ObjectInstance.GetType().GetProperty(str).GetValue(validationContext.ObjectInstance, null)))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    return new ValidationResult("Please select an option.");
                }
            }
            return ValidationResult.Success;
        }

        public string CommaSeperatedProperties { get; private set; }

        public class ModelClientValidationSelectOneRule : ModelClientValidationRule
        {
            public ModelClientValidationSelectOneRule(string errorMessage, string[] strProperties)
            {
                base.ErrorMessage = errorMessage;
                base.ValidationType = "mycustomvalidation";
                for (int i = 0; i < strProperties.Length; i++)
                {
                    base.ValidationParameters.Add("otherproperty" + i.ToString(), strProperties[i]);
                }
            }
        }
    }
}

