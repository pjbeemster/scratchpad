namespace Coats.Crafts.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class FacetItem : SelectListItem
    {
        private bool _enabled = true;

        public FacetSection childSection { get; set; }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
            }
        }

        public string FHId { get; set; }

        public string Href { get; set; }

        public int NumItems { get; set; }
    }
}

