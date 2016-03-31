namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SegregatedContentItem
    {
        public List<Component> Components { get; set; }

        public string LocationString { get; set; }

        public string Title { get; set; }

        public int ViewSize { get; set; }
    }
}

