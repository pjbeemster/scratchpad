namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class FacetCollection
    {
        public IList<FacetSection> FacetSections { get; set; }

        public string ResetFacetsUrl { get; set; }
    }
}

