namespace Coats.Crafts.Models
{
    using Coats.Crafts.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Locations : ICoatsData
    {
        public string AddressBookId { get; set; }

        public string AdminEmail { get; set; }

        public List<Locations> Children { get; set; }

        public string ContactUsEmail { get; set; }

        public string Id { get; set; }

        public string Iso { get; set; }

        public string Name { get; set; }

        public string RequestASampleEmail { get; set; }

        public string Uri { get; set; }

        public string WebinarFeedbackEmail { get; set; }
    }
}

