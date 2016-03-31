using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

using Coats.IndustrialPortal.Gateway;
using Coats.IndustrialPortal.Constants;
using Coats.IndustrialPortal.Providers;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Repositories.Tridion;
using Coats.Crafts.Configuration;

namespace Coats.Crafts.Validators
{
    public class UserNameValidator : ValidationAttribute
    {
        private IRegistrationRepository _registrationrepository;

        public UserNameValidator(IRegistrationRepository registrationrepository)
        {
            _registrationrepository = registrationrepository;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string displayname = value.ToString();

                if (_registrationrepository.checkDisplayNameExists(displayname) == true)
                {
                    return new ValidationResult("Email already exists");
                } else {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("This field is mandatory");
            }
        }


    }
}
