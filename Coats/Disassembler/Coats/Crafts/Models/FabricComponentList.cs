namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System.Collections.Generic;

    public class FabricComponentList
    {
        public List<IComponent> Components { get; set; }

        public IList<FacetPagination> Pagination { get; set; }

        public IComponent SelectedComponent { get; set; }
    }
}
