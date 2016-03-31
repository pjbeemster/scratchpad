namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SegregatedContent
    {
        public ComponentSearchSection ComponentSection { get; set; }

        public Dictionary<string, SegregatedContentItem> SegregatedContentItems { get; set; }
    }
}

