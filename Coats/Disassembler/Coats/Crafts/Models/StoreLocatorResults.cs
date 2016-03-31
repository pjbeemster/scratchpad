namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class StoreLocatorResults
    {
        private Field _brand = new Field();

        public Field Brand
        {
            get
            {
                return this._brand;
            }
            set
            {
                this._brand = value;
            }
        }

        [Display(Name="Distance"), CustomResourceRequired("DistanceRequired")]
        public int Distance { get; set; }

        public IEnumerable<SelectListItem> DistanceItems { get; set; }

        public string Error { get; set; }

        public ExtComponentSearchSection ExtComponentSection { get; set; }

        public decimal Latitude { get; set; }

        [Display(Name="Location"), CustomResourceRequired("LocationRequired")]
        public string Location { get; set; }

        public decimal Longitude { get; set; }

        public List<GoogleMapsMarker> Options { get; set; }

        public List<StoreLocatorRetailer> Retailers { get; set; }

        public string RetailersJSON { get; set; }

        public string StaticMapAddress { get; set; }

        public IEnumerable<SelectListItem> WithinItems { get; set; }

        [Display(Name="Within")]
        public string WithinVal { get; set; }
    }
}

