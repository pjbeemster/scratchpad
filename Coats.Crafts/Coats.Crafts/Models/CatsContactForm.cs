using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Validators;
using System.Web.Mvc;
using System;
using DD4T.ContentModel;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class CatsContactForm
    {
        /// <summary>
        /// Use constrcutor to set defaults for nullable fields
        /// </summary>
        public CatsContactForm()
        {
            Address2 = "";
            ProductDescription = "";
        }

        [Required(AllowEmptyStrings = false), StringLength(5)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(20)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(20)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(50)]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(40)]
        public string Address1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull=false), StringLength(40)]
        public string Address2 { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(40)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(2)]
        public string State { get; set; }

        [Required, StringLength(12)]
        [DataType(DataType.PhoneNumber)]
        [CustomRegularExpression("TelephoneNumber", @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid number")]
        public string TelephoneNumber { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(7)]
        public string ZipCode { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(50)]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(45)]
        public string QuestionType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false), StringLength(40)]
        public string ProductDescription { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(50)]
        public string ProductType { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(4000)]
        public string Comments { get; set; }

        public DateTime dateSubmitted { get; set; }

        public IComponentPresentation cp { get; set; }
    }
}