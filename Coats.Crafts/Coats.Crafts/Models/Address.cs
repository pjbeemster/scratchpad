using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Coats.Crafts.Models
{
    public class Address
    {
        public string BuildingNameNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

        [CustomStringLength(10, "PostcodeLengthError",0)]
        public string Postcode { get; set; }
    }
}