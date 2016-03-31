namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using DD4T.ContentModel;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class CatsContactForm
    {
        public CatsContactForm()
        {
            this.Address2 = "";
            this.ProductDescription = "";
        }

        [StringLength(40), Required(AllowEmptyStrings=false)]
        public string Address1 { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull=false), StringLength(40)]
        public string Address2 { get; set; }

        [Required(AllowEmptyStrings=false), StringLength(40)]
        public string City { get; set; }

        [StringLength(0xfa0), Required(AllowEmptyStrings=false)]
        public string Comments { get; set; }

        [StringLength(50), Required(AllowEmptyStrings=false)]
        public string Country { get; set; }

        public IComponentPresentation cp { get; set; }

        public DateTime dateSubmitted { get; set; }

        [Required(AllowEmptyStrings=false), StringLength(50)]
        public string EmailAddress { get; set; }

        [StringLength(20), Required(AllowEmptyStrings=false)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings=false), StringLength(20)]
        public string LastName { get; set; }

        [StringLength(40), DisplayFormat(ConvertEmptyStringToNull=false)]
        public string ProductDescription { get; set; }

        [StringLength(50), Required(AllowEmptyStrings=false)]
        public string ProductType { get; set; }

        [StringLength(0x2d), Required(AllowEmptyStrings=false)]
        public string QuestionType { get; set; }

        [Required(AllowEmptyStrings=false), StringLength(2)]
        public string State { get; set; }

        [Required, DataType(DataType.PhoneNumber), CustomRegularExpression("TelephoneNumber", @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$"), StringLength(12)]
        public string TelephoneNumber { get; set; }

        [StringLength(5), Required(AllowEmptyStrings=false)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings=false), StringLength(7)]
        public string ZipCode { get; set; }
    }
}

