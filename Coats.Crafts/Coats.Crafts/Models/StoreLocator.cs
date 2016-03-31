using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;
using Coats.Crafts.Data;
using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class StoreLocator
    {

    }

    public class StoreLocatorResults {

        public List<GoogleMapsMarker> Options { get; set; }
        public List<StoreLocatorRetailer> Retailers { get; set; }
        public IEnumerable<SelectListItem> DistanceItems { get; set; }
        public IEnumerable<SelectListItem> WithinItems { get; set; }

        [CustomResourceRequired("LocationRequired")]
        //[Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [CustomResourceRequired("DistanceRequired")]
        //[Required(ErrorMessage = "Distance is required")]
        [Display(Name = "Distance")]
        public int Distance { get; set; }

        [Display(Name = "Within")]
        public string WithinVal { get; set; }

        public string RetailersJSON { get; set; }
        public string StaticMapAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get;set; }
        public string Error { get; set; }

        public ExtComponentSearchSection ExtComponentSection { get; set; }

        private Field _brand = new Field();
        public Field Brand
        {
            get
            {
                return _brand;
            }
            set
            {
                _brand = value;
            }
        }
    }
}