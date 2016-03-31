using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Attributes;
using System.Web.Mvc;
using Coats.Crafts.Validators;
using DataAnnotationsExtensions;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Resources;
using System.ComponentModel;

namespace Coats.Crafts.Models
{
    public class Customer : BaseCustomer
    {
        [CustomResourceRequired("EmailAddressRequired")]
        [DataType(DataType.EmailAddress)]
        [EmailValidator]
        [CustomEmail("ValidEmail")]
        //[Email]
        [Remote("IsEmailAvailable", "RemoteValidation", HttpMethod = "POST")]
        public string EmailAddress { get; set; }

        [CustomResourceRequired("PasswordRequired")]
        [CustomStringLength(100, "PasswordLengthError", 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [CustomResourceRequired("VerifyPasswordRequired")]
        [DataType(DataType.Password)]
        [CustomCompare("ConfirmPasswordError", "Password")]
        public string VerifyPassword { get; set; }

        [Remote("IsDisplayNameAvailable", "RemoteValidation", HttpMethod = "POST")]
        [CustomResourceRequired("DisplayNameRequired")]
        [CustomRegularExpression("DisplayNameInvalidCharacters", @"^[a-zA-Z0-9 ]+$")]
        public string DisplayName { get; set; }
        public string SiteUrl { get; set; }
    }
}
