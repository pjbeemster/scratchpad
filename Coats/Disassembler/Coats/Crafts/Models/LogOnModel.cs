namespace Coats.Crafts.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class LogOnModel
    {
        [DataType(DataType.Password), Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}

