namespace Coats.Crafts.Attributes
{
    using Coats.Crafts.Configuration;
    using Foolproof;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;

    public class CustomRequiredIfAttribute : RequiredIfNotRegExMatchAttribute, IClientValidatable
    {
        private readonly string _resourceName;

        public CustomRequiredIfAttribute(string resourceName, string dependentValue, string pattern) : base(dependentValue, pattern)
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

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationRequiredRule[] { new ModelClientValidationRequiredRule(this.FormatErrorMessage(metadata.DisplayName)) };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string str = this.FormatErrorMessage(this._resourceName);
            if (value != null)
            {
                string str2 = value.ToString();
            }
            return ValidationResult.Success;
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
                base.ValidationType = "customrequiredifvalidation";
            }
        }
    }
}

