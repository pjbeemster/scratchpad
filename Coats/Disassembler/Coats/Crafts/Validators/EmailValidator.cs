namespace Coats.Crafts.Validators
{
    using Coats.Crafts.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Security;

    public class EmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (Membership.GetUser(value.ToString()) == null)
                {
                    return ValidationResult.Success;
                }
                return new ValidationResult(Helper.GetResource("EmailAlreadyExists"));
            }
            return new ValidationResult(Helper.GetResource("EmailAddressRequired"));
        }
    }
}

