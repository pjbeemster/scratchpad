namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class PaginatedComponentList
    {
        public List<Component> Components { get; set; }

        public IList<FacetPagination> Pagination { get; set; }
    }
}

