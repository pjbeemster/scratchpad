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
using Coats.Crafts.Resources;

namespace Coats.Crafts.Validators
{
    public class EmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();

                MembershipUser mUser = Membership.GetUser(email);

                if (mUser == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    //return new ValidationResult("Email already exists"); //Helper.GetResource("")
                    return new ValidationResult(Helper.GetResource("EmailAlreadyExists"));
                }
            }
            else
            {
                //return new ValidationResult("This field is mandatory");
                return new ValidationResult(Helper.GetResource("EmailAddressRequired"));                
            }
        }
    }
}
