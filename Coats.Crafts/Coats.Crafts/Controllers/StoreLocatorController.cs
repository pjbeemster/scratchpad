using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using Coats.Crafts.Models;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Resources;
using Coats.Crafts.Filters;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;

using Castle.Core.Logging;

using DD4T.Mvc.Controllers;
using DD4T.Mvc.Attributes;
using DD4T.ContentModel;
using System.Globalization;

namespace Coats.Crafts.Controllers
{
    public class StoreLocatorController : TridionControllerBase
    {
        private IStoreLocatorRepository storelocatorrepository;

        public ILogger Logger { get; set; }

        public StoreLocatorController(IStoreLocatorRepository storelocatorrepository)
        {
            this.storelocatorrepository = storelocatorrepository;
        }


        // Merge Post functionality into Get request

        [HttpGet, Level1BrandFilter]
        public ActionResult Index(ComponentPresentation presentation, string Location = "", decimal Latitude = 0, decimal Longitude = 0, string Distance = null, bool clear = false, string Address = "", string NoJS = "", string WithinVal = null)
        {
            if (clear)
            {
                if (Logger.IsDebugEnabled)
                    Logger.Debug("Clearing session");

                // Not nice but effective
                RouteData.Values["Level1BrandActivated"] = false;
                RouteData.Values["BrandComponent"] = new Field();
                RouteData.Values["BrandFilter"] = String.Empty;
                RouteData.Values["BrandFacet"] = String.Empty;
                RouteData.Values["BrandFacetValue"] = String.Empty;
                RouteData.Values["BrandValueForSearch"] = String.Empty;

                Session.ClearLevel1BrandFilter();
            }

            bool isLatLng = false;
            bool isInitLoad = false;

            if (Latitude != 0 && Longitude != 0)
            {
                isLatLng = true;
            }
            else if (String.IsNullOrEmpty((Location)))
            {
                //we have no Lat/Lng or location
                isInitLoad = true;
            }
            
            string brandFilter = "";

            if (!String.IsNullOrEmpty(Session.GetLevel1BrandFilter()))
            {
                brandFilter = RouteData.Values.GetLevel1BrandSearchValue();
            }           

            if (Distance == null) {
                Distance = WebConfiguration.Current.StoreLocatorDefaultValue;
            }

            // New Store results object
            StoreLocatorResults results = new StoreLocatorResults();
            results.DistanceItems = storelocatorrepository.GetDistanceItems(Distance);
            results.Distance = Convert.ToInt16(Distance);

            if (!String.IsNullOrEmpty(WebConfiguration.Current.StoreLocatorWithinVals))
            {
                if (WithinVal != null)
                {
                    results.WithinItems = storelocatorrepository.GetWithinItems(WithinVal);
                }
                else
                {
                    results.WithinItems = storelocatorrepository.GetWithinItems();
                }
            }
            else {
                results.WithinItems = new List<SelectListItem>();
            }

            // New Marker result object
            GoogleMapsMarker currentLoc = null;

            // New locations list
            List<GoogleMapsMarker> Locations = new List<GoogleMapsMarker>();

            // CC-1600
            // Persist selected Address data (when user is given multiple address results) and override location if location is available and hasn't changed
            if (!String.IsNullOrEmpty(Address))
            {            
                Session["SelectedAddress"] = Address;
            }
            else if (String.IsNullOrEmpty(Address) && !String.IsNullOrEmpty(Location))
            {
                if (Location == Session["SelectedAddress"] as string)
                {
                    Address = Location;
                }
            }

            if (isLatLng && NoJS != "true" || !String.IsNullOrEmpty(Address) && NoJS == "true") //lat and lng or address passed
            {
                currentLoc = new GoogleMapsMarker("current", Latitude, Longitude);
                if (!String.IsNullOrEmpty(Address)){
                    currentLoc = setAddressOnMarker(Address, ',', currentLoc);
                }
                results.Latitude = Latitude;
                results.Longitude = Longitude; 
            }              
            else if (!String.IsNullOrEmpty(Location)) //Location name only
            {
                results.Location = Location;
                // Get all possible locations from Google
                string culture = WebConfiguration.Current.Culture;
                string regionBias = "";

                if (String.IsNullOrEmpty(WithinVal))
                {
                    regionBias = culture.Substring(culture.Length - 2);
                }
                else {
                    regionBias = WithinVal;
                }

                Locations = storelocatorrepository.GetMarkerForPostcode(Location, regionBias);
                int maxDisambigResults = 5;
                int.TryParse(WebConfiguration.Current.StoreLocatorMaxDisambigResults, out maxDisambigResults);

                // Set up variables depending on location count
                switch (Locations.Count)
                {
                    case 0:
                        results.Error = Helper.GetResource("StoreLocatorPostcodeNotFound");
                        break;
                    case 1:
                        if (Locations[0].type != "country")
                        {
                            currentLoc = Locations[0];
                            results.Latitude = currentLoc.lat;
                            Latitude = currentLoc.lat;
                            results.Longitude = currentLoc.lng;
                            Longitude = currentLoc.lng;
                        }
                        else {
                            results.Error = Helper.GetResource("StoreLocatorPostcodeNotFound");
                        }
                        break;
                    default:
                        results.Options = Locations.GetRange(0, Locations.Count >= maxDisambigResults ? maxDisambigResults : Locations.Count);
                        results.Latitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLatitude, CultureInfo.InvariantCulture);
                        results.Longitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLongitude, CultureInfo.InvariantCulture);
                        break;
                }

            }
            else { //first load
                results.Latitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLatitude, CultureInfo.InvariantCulture);
                results.Longitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLongitude, CultureInfo.InvariantCulture);
            }

            if (currentLoc != null)
            {
                List<StoreLocatorRetailer> retailers = storelocatorrepository.GetStoreLocatorRetailers(currentLoc, Convert.ToInt16(Distance), brandFilter);
                if (retailers.Count > 0)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    results.Retailers = retailers;
                    results.RetailersJSON = js.Serialize(retailers);
                    results.StaticMapAddress = storelocatorrepository.GetStaticMap(currentLoc, retailers);
                }
                else
                {
                    results.Error = Helper.GetResource("StoreLocatorNoRetailers");
                }

                results.Location = getAddressFromLocation(currentLoc);
                results.Latitude = currentLoc.lat;
                results.Longitude = currentLoc.lng;
            }

            SetComponentTypes(results);

            if (RouteData.Values.IsLevel1BrandFilterActivated())
            {
                results.Brand = RouteData.Values.GetLevel1BrandComponent();
                ViewBag.StoreLocator = true;
            }



            return View(results);
        }

        private void SetComponentTypes(StoreLocatorResults results)
        {
            results.ExtComponentSection = new ExtComponentSearchSection();
            results.ExtComponentSection.ComponentTypes = FacetedContentHelper.GetComponentSchemaTypes("StoreLocator");
        }

        private string getAddressFromLocation(GoogleMapsMarker location)
        {
            string address = "";
            if (!string.IsNullOrEmpty(location.AddressLine1))
            {
                address = location.AddressLine1;
            }
            if (!string.IsNullOrEmpty(location.AddressLine2))
            {
                address += "," + location.AddressLine2;
            }
            if (!string.IsNullOrEmpty(location.AddressLine3))
            {
                address += "," + location.AddressLine3;
            }
            if (!string.IsNullOrEmpty(location.AddressLine4))
            {
                address += "," + location.AddressLine4;
            }
            if (!string.IsNullOrEmpty(location.AddressLine5))
            {
                address += "," + location.AddressLine5;
            }
            if (!string.IsNullOrEmpty(location.AddressLine6))
            {
                address += "," + location.AddressLine6;
            }
            if (!string.IsNullOrEmpty(location.AddressLine7))
            {
                address += "," + location.AddressLine7;
            }

            if (address.StartsWith(","))
            {
                address = address.Substring(1, address.Length - 1);
            }

            return address;
        }

        private GoogleMapsMarker setAddressOnMarker(string address, char separator, GoogleMapsMarker marker)
        {
            string[] addressArr = address.Split(separator);
            int arrayLength = addressArr.Length;
            if (arrayLength > 0 && addressArr[0] != null)
            {
                marker.AddressLine1 = addressArr[0];
            }
            if (arrayLength > 1 && addressArr[1] != null)
            {
                marker.AddressLine2 = addressArr[1];
            }
            if (arrayLength > 2 && addressArr[2] != null)
            {
                marker.AddressLine3 = addressArr[2];
            }
            if (arrayLength > 3 && addressArr[3] != null)
            {
                marker.AddressLine4 = addressArr[3];
            }
            if (arrayLength > 4 && addressArr[4] != null)
            {
                marker.AddressLine5 = addressArr[4];
            }
            if (arrayLength > 5 && addressArr[5] != null)
            {
                marker.AddressLine6 = addressArr[5];
            }
            if (arrayLength > 6 && addressArr[6] != null)
            {
                marker.AddressLine7 = addressArr[6];
            }

            return marker;
        }
    }
}
