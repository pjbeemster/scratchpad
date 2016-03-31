using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Mvc;
using Coats.Crafts.Attributes;
using DataAnnotationsExtensions;

namespace Coats.Crafts.Models
{
    public class Login
    {
        [CustomResourceRequired("EmailAddress")]
        //[Email]
        [CustomEmail("ValidEmail")]
        public string EmailAddress { get; set; }

        [CustomResourceRequired("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}