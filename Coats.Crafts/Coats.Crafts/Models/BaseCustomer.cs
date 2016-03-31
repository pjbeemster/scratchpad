using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Validators;
using System.Web.Mvc;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class BaseCustomer : Login
    {
        [CustomResourceRequired("FirstNameRequired")]
        public string FirstName { get; set; }

        [CustomResourceRequired("LastNameRequired")]
        public string LastName { get; set; }

        public string TelephoneNumber { get; set; }
        public string About { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }

    }
}