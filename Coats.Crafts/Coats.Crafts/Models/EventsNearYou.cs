using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;

namespace Coats.Crafts.Models
{
    public class EventsNearYou
    {
        public string ComponentTitle { get; set; }
        public List<CraftsEvent> Events { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string LinkTitle { get; set; }
        public string LinkURL { get; set; }
        public IComponent LinkComponent { get; set; }
    }
}
