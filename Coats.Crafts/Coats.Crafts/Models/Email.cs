using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Coats.Crafts.HtmlHelpers;
using System.Collections.Generic;
using Coats.Crafts.Interfaces;
using System.Web.Mvc;
using Coats.Crafts.Resources;
using Coats.Crafts.Validators;
using DataAnnotationsExtensions;

namespace Coats.Crafts.Models
{
    public class Email : IEmail
    {
        //[Required(ErrorMessage = "Email is required", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        //[Email]
        public string EmailAddress { get; set; }
    }
}