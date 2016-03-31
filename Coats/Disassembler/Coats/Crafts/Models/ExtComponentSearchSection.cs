namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ExtComponentSearchSection : ComponentSearchSection, IComponentSearchSection
    {
        public List<FacetItem> ChooseCategoryList { get; set; }

        public string ChosenCategory { get; set; }

        public bool HasSelectedItems
        {
            get
            {
                try
                {
                    return (this.ChooseCategoryList.Count<FacetItem>(c => c.Selected) > 0);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}

