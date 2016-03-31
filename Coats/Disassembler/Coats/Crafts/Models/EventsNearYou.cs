namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class EventsNearYou
    {
        public string ComponentTitle { get; set; }

        public List<CraftsEvent> Events { get; set; }

        public string Lat { get; set; }

        public IComponent LinkComponent { get; set; }

        public string LinkTitle { get; set; }

        public string LinkURL { get; set; }

        public string Lng { get; set; }
    }
}

