namespace Coats.Crafts.Attributes
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;

    public class OldCustomStringLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _maximumLength;
        public int _minimumLength;
        private readonly string _resourceName;

        public OldCustomStringLengthAttribute(string resourceName, int maxLength, int minLength)
        {
            this._resourceName = resourceName;
            this._maximumLength = maxLength;
            this._minimumLength = minLength;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(this._resourceName))
            {
                string globalResourceObject = HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, this._resourceName) as string;
                return string.Format(globalResourceObject, new object[] { name });
            }
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name });
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationRequiredRule[] { new ModelClientValidationRequiredRule(this.FormatErrorMessage(metadata.DisplayName)) };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string errorMessage = this.FormatErrorMessage(this._resourceName);
            if (value != null)
            {
                string str2 = value.ToString();
                if (str2.Length > this.MaximumLength)
                {
                    return new ValidationResult(errorMessage);
                }
                if (str2.Length < this.MinimumLength)
                {
                    return new ValidationResult(errorMessage);
                }
                return ValidationResult.Success;
            }
            return new ValidationResult(errorMessage);
        }

        public int MaximumLength
        {
            get
            {
                return this._maximumLength;
            }
        }

        public int MinimumLength
        {
            get
            {
                return this._minimumLength;
            }
        }

        public string ResourceName
        {
            get
            {
                return this._resourceName;
            }
        }

        public class ModelClientValidationRequiredRule : ModelClientValidationRule
        {
            public ModelClientValidationRequiredRule(string errorMessage)
            {
                base.ErrorMessage = errorMessage;
                base.ValidationType = "customstringlengthvalidation";
            }
        }
    }
}

