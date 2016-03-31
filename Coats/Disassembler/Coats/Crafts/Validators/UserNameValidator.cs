namespace Coats.Crafts.Validators
{
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserNameValidator : ValidationAttribute
    {
        private IRegistrationRepository _registrationrepository;

        public UserNameValidator(IRegistrationRepository registrationrepository)
        {
            this._registrationrepository = registrationrepository;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string displayname = value.ToString();
                if (this._registrationrepository.checkDisplayNameExists(displayname))
                {
                    return new ValidationResult("Email already exists");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("This field is mandatory");
        }
    }
}

