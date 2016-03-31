namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    public class BaseCustomer : Login
    {
        public string About { get; set; }

        [CustomResourceRequired("FirstNameRequired")]
        public string FirstName { get; set; }

        [CustomResourceRequired("LastNameRequired")]
        public string LastName { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }

        public string TelephoneNumber { get; set; }
    }
}

