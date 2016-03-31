namespace Coats.Crafts.Models
{
    using System;
    using System.Runtime.CompilerServices;

    public class ComponentSearchSection : IComponentSearchSection
    {
        public FacetSection ComponentTypes { get; set; }

        public string SearchTerm { get; set; }
    }
}

