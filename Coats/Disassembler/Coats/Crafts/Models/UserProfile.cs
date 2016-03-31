namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using Coats.Crafts.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    public class UserProfile : IUserProfile
    {
        public UserProfile()
        {
            this.CustomerDetails = new Customer();
            this.AddressDetails = new Address();
        }

        public string AddressBookId { get; set; }

        public Address AddressDetails { get; set; }

        public string AdministratorEmail { get; set; }

        public List<SelectListItem> CraftTypeList { get; set; }

        [DataType(DataType.Password), CustomStringLength(100, "PasswordLengthError", 6), CustomRequiredIf("CurrentPasswordRequired", "NewPassword", "^$")]
        public string CurrentPassword { get; set; }

        public Customer CustomerDetails { get; set; }

        public List<SelectListItem> EmailNewsletter { get; set; }

        public Dictionary<string, string> Keywords { get; set; }

        [CustomStringLength(100, "NewPasswordLengthError", 6), DataType(DataType.Password)]
        public string NewPassword { get; set; }

        public List<Newsletter> NewsletterList { get; set; }

        public string PostCodeMapUrl { get; set; }

        public List<SelectListItem> ProfileVisible { get; set; }

        public string RegistrationStatus { get; set; }

        public string returnUrl { get; set; }

        [DataType(DataType.Password), CustomCompare("VerifyNewPasswordError", "NewPassword")]
        public string VerifyNewPassword { get; set; }
    }
}

