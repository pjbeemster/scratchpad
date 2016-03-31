namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using Coats.Crafts.Validators;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class Customer : BaseCustomer
    {
        [Remote("IsDisplayNameAvailable", "RemoteValidation", HttpMethod="POST"), CustomRegularExpression("DisplayNameInvalidCharacters", "^[a-zA-Z0-9 ]+$"), CustomResourceRequired("DisplayNameRequired")]
        public string DisplayName { get; set; }

        [EmailValidator, DataType(DataType.EmailAddress), Remote("IsEmailAvailable", "RemoteValidation", HttpMethod="POST"), CustomResourceRequired("EmailAddressRequired"), CustomEmail("ValidEmail")]
        public string EmailAddress { get; set; }

        [CustomStringLength(100, "PasswordLengthError", 6), DataType(DataType.Password), CustomResourceRequired("PasswordRequired")]
        public string Password { get; set; }

        public string SiteUrl { get; set; }

        [DataType(DataType.Password), CustomResourceRequired("VerifyPasswordRequired"), CustomCompare("ConfirmPasswordError", "Password")]
        public string VerifyPassword { get; set; }
    }
}

