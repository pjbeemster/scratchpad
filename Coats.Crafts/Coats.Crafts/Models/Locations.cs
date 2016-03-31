using System.Collections.Generic;
using Coats.Crafts.Interfaces;

namespace Coats.Crafts.Models
{
    public class Locations : ICoatsData
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string Uri { get; set; }

        public string AdminEmail { get; set; }  //Used for website registrations

        //JAW (15/05/2012) - Added additional email addresses to the class
        public string ContactUsEmail { get; set; }
        public string RequestASampleEmail { get; set; }
        public string WebinarFeedbackEmail { get; set; }

        public string AddressBookId { get; set; }

        public List<Locations> Children { get; set; }

        public string Iso { get; set; }
    }

}