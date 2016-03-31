namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    public class FacetSort
    {
        private bool? _activeDiscussionsEnabled;

        public bool ActiveDiscussions { get; set; }

        public bool ActiveDiscussionsEnabled
        {
            get
            {
                return (!this._activeDiscussionsEnabled.HasValue || this._activeDiscussionsEnabled.Value);
            }
            set
            {
                this._activeDiscussionsEnabled = new bool?(value);
            }
        }

        public string ActiveDiscussionsUrl { get; set; }

        public string SortBy { get; set; }

        public IList<SelectListItem> SortByList { get; set; }
    }
}

