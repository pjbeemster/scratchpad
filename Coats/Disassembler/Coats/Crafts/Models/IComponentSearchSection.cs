namespace Coats.Crafts.Models
{
    using System;

    public interface IComponentSearchSection
    {
        FacetSection ComponentTypes { get; set; }

        string SearchTerm { get; set; }
    }
}

