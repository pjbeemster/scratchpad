namespace Coats.Crafts.Models
{
    using Coats.Crafts.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class Email : IEmail
    {
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
}

