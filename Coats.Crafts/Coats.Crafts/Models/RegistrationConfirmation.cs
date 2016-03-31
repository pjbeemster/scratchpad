using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Validators;
using System.Web.Mvc;
using System;
using DD4T.ContentModel;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class RegistrationConfirmation
    {
        public RegistrationConfirmation()
        {

        }

        public DateTime dateSubmitted { get; set; }

        public bool IsEmailExist { get; set; }

        public IComponentPresentation cp { get; set; }
    }
}