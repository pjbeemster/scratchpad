namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class Login
    {
        [CustomEmail("ValidEmail"), CustomResourceRequired("EmailAddress")]
        public string EmailAddress { get; set; }

        [CustomResourceRequired("Password"), DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}

