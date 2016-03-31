namespace Coats.Crafts.App_Start
{
    using DataAnnotationsExtensions.ClientValidation;
    using System;

    public static class RegisterClientValidationExtensions
    {
        public static void Start()
        {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();
        }
    }
}

