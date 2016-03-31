namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;

    public class RegistrationConfirmation
    {
        public IComponentPresentation cp { get; set; }

        public DateTime dateSubmitted { get; set; }

        public bool IsEmailExist { get; set; }
    }
}

