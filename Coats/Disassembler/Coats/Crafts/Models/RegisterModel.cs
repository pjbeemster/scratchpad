namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class RegisterModel
    {
        [DataType(DataType.Password), CustomCompare("ConfirmPasswordError", "Password")]
        public string ConfirmPassword { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, CustomStringLength(100, "PasswordLengthError", 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}

