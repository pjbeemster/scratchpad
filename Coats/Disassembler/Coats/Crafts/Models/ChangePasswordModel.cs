namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class ChangePasswordModel
    {
        [DataType(DataType.Password), CustomCompare("ConfirmPasswordError", "NewPassword")]
        public string ConfirmPassword { get; set; }

        [Required, DataType(DataType.Password), CustomStringLength(100, "PasswordLengthError", 6)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }
    }
}

