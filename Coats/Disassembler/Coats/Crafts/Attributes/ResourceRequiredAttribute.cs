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

    public class ResourceRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private readonly string _resourceName;

        public ResourceRequiredAttribute(string resourceName)
        {
            this._resourceName = resourceName;
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
            yield return new System.Web.Mvc.ModelClientValidationRequiredRule(this.FormatErrorMessage(this._resourceName));
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

