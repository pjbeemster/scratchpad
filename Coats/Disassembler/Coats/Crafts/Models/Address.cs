namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    public class Address
    {
        public string BuildingNameNumber { get; set; }

        public string City { get; set; }

        [CustomStringLength(10, "PostcodeLengthError", 0)]
        public string Postcode { get; set; }

        public string Street { get; set; }
    }
}

