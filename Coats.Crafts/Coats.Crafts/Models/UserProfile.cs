using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Coats.Crafts.HtmlHelpers;
using System.Collections.Generic;
using Coats.Crafts.Interfaces;
using System.Web.Mvc;
using Coats.Crafts.Resources;
using System;
using Foolproof;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class UserProfile : IUserProfile
    {
        public UserProfile()
        {
            CustomerDetails = new Customer();
            AddressDetails = new Address();
        }

        public Customer CustomerDetails { get; set; }
        public Address AddressDetails { get; set; }
        public List<SelectListItem> CraftTypeList { get; set; }

        public string AddressBookId { get; set; }
        public string AdministratorEmail { get; set; }

        public string RegistrationStatus { get; set; }

        public string PostCodeMapUrl { get; set; }

        public Dictionary<string, string> Keywords { get; set; }

        public List<SelectListItem> EmailNewsletter { get; set; }
        public List<SelectListItem> ProfileVisible { get; set; }

        //Required if NewPassword field is not empty
        //[RequiredIfNotRegExMatch("NewPassword", "^$", ErrorMessage = "Please enter your current password")]
        [CustomRequiredIf("CurrentPasswordRequired", "NewPassword", "^$")]
        [CustomStringLength(100, "PasswordLengthError", 6)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [CustomStringLength(100, "NewPasswordLengthError", 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [CustomCompare("VerifyNewPasswordError", "NewPassword")]
        public string VerifyNewPassword { get; set; }

        public string returnUrl { get; set; }
    }
}