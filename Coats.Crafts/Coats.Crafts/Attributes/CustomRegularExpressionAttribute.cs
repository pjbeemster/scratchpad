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

    public class CustomRegularExpressionAttribute :  RegularExpressionAttribute, IClientValidatable
    {
        // Fields
        private readonly string _resourceName;
        private readonly string _regularExpressionAttribute;
        
        // Methods

        public CustomRegularExpressionAttribute(string resourceName, string regularExpressionAttribute)
            : base(regularExpressionAttribute)
            //: base(@"^[a-zA-Z0-9 ]+$")
        {
            this._resourceName = resourceName;
            this._regularExpressionAttribute = regularExpressionAttribute;
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

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRegexRule(FormatErrorMessage(_resourceName), _regularExpressionAttribute);
            //yield return new ModelClientValidationRule();
        }


    }

}