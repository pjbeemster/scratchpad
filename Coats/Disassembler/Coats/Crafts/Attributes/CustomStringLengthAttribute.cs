namespace Coats.Crafts.Attributes
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;

    public class CustomStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        private readonly int _maximumLength;
        private readonly int _minimumLength;
        private readonly string _resourceName;

        public CustomStringLengthAttribute(int minimumLength, string resourceName, int maximumLength) : base(minimumLength)
        {
            this._resourceName = resourceName;
            this._minimumLength = minimumLength;
            this._maximumLength = maximumLength;
        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(this._resourceName))
            {
                string globalResourceObject = HttpContext.GetGlobalResourceObject(WebConfiguration.Current.ResourceName, this._resourceName) as string;
                try
                {
                    return string.Format(globalResourceObject, new object[] { name, base.MinimumLength });
                }
                catch (FormatException)
                {
                    return globalResourceObject;
                }
            }
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name });
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationStringLengthRule(this.FormatErrorMessage(this._resourceName), this._maximumLength, this._minimumLength);
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

