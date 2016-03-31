namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Data;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    public class StoreLocatorController : TridionControllerBase
    {
        private IStoreLocatorRepository storelocatorrepository;

        public StoreLocatorController(IStoreLocatorRepository storelocatorrepository)
        {
            this.storelocatorrepository = storelocatorrepository;
        }

        private string getAddressFromLocation(GoogleMapsMarker location)
        {
            string str = "";
            if (!string.IsNullOrEmpty(location.AddressLine1))
            {
                str = location.AddressLine1;
            }
            if (!string.IsNullOrEmpty(location.AddressLine2))
            {
                str = str + "," + location.AddressLine2;
            }
            if (!string.IsNullOrEmpty(location.AddressLine3))
            {
                str = str + "," + location.AddressLine3;
            }
            if (!string.IsNullOrEmpty(location.AddressLine4))
            {
                str = str + "," + location.AddressLine4;
            }
            if (!string.IsNullOrEmpty(location.AddressLine5))
            {
                str = str + "," + location.AddressLine5;
            }
            if (!string.IsNullOrEmpty(location.AddressLine6))
            {
                str = str + "," + location.AddressLine6;
            }
            if (!string.IsNullOrEmpty(location.AddressLine7))
            {
                str = str + "," + location.AddressLine7;
            }
            if (str.StartsWith(","))
            {
                str = str.Substring(1, str.Length - 1);
            }
            return str;
        }

        [HttpGet, Level1BrandFilter]
        public ActionResult Index(ComponentPresentation presentation, string Location = "", decimal Latitude = 0, decimal Longitude = 0, string Distance = null, bool clear = false, string Address = "", string NoJS = "", string WithinVal = null)
        {
            if (clear)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.Debug("Clearing session");
                }
                base.RouteData.Values["Level1BrandActivated"] = false;
                base.RouteData.Values["BrandComponent"] = new Field();
                base.RouteData.Values["BrandFilter"] = string.Empty;
                base.RouteData.Values["BrandFacet"] = string.Empty;
                base.RouteData.Values["BrandFacetValue"] = string.Empty;
                base.RouteData.Values["BrandValueForSearch"] = string.Empty;
                base.Session.ClearLevel1BrandFilter();
            }
            bool flag = false;
            bool flag2 = false;
            if ((Latitude != 0M) && (Longitude != 0M))
            {
                flag = true;
            }
            else if (string.IsNullOrEmpty(Location))
            {
                flag2 = true;
            }
            string filterByBrandname = "";
            if (!string.IsNullOrEmpty(base.Session.GetLevel1BrandFilter()))
            {
                filterByBrandname = base.RouteData.Values.GetLevel1BrandSearchValue();
            }
            if (Distance == null)
            {
                Distance = WebConfiguration.Current.StoreLocatorDefaultValue;
            }
            StoreLocatorResults results = new StoreLocatorResults {
                DistanceItems = this.storelocatorrepository.GetDistanceItems(Distance),
                Distance = Convert.ToInt16(Distance)
            };
            if (!string.IsNullOrEmpty(WebConfiguration.Current.StoreLocatorWithinVals))
            {
                if (WithinVal != null)
                {
                    results.WithinItems = this.storelocatorrepository.GetWithinItems(WithinVal);
                }
                else
                {
                    results.WithinItems = this.storelocatorrepository.GetWithinItems(null);
                }
            }
            else
            {
                results.WithinItems = new List<SelectListItem>();
            }
            GoogleMapsMarker marker = null;
            List<GoogleMapsMarker> markerForPostcode = new List<GoogleMapsMarker>();
            if (!string.IsNullOrEmpty(Address))
            {
                base.Session["SelectedAddress"] = Address;
            }
            else if ((string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Location)) && (Location == (base.Session["SelectedAddress"] as string)))
            {
                Address = Location;
            }
            if ((flag && (NoJS != "true")) || (!string.IsNullOrEmpty(Address) && (NoJS == "true")))
            {
                marker = new GoogleMapsMarker("current", Latitude, Longitude);
                if (!string.IsNullOrEmpty(Address))
                {
                    marker = this.setAddressOnMarker(Address, ',', marker);
                }
                results.Latitude = Latitude;
                results.Longitude = Longitude;
            }
            else if (string.IsNullOrEmpty(Location))
            {
                results.Latitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLatitude, CultureInfo.InvariantCulture);
                results.Longitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLongitude, CultureInfo.InvariantCulture);
            }
            else
            {
                results.Location = Location;
                string culture = WebConfiguration.Current.Culture;
                string bias = "";
                if (string.IsNullOrEmpty(WithinVal))
                {
                    bias = culture.Substring(culture.Length - 2);
                }
                else
                {
                    bias = WithinVal;
                }
                markerForPostcode = this.storelocatorrepository.GetMarkerForPostcode(Location, bias);
                int result = 5;
                int.TryParse(WebConfiguration.Current.StoreLocatorMaxDisambigResults, out result);
                switch (markerForPostcode.Count)
                {
                    case 0:
                        results.Error = Helper.GetResource("StoreLocatorPostcodeNotFound");
                        break;

                    case 1:
                        if (markerForPostcode[0].type == "country")
                        {
                            results.Error = Helper.GetResource("StoreLocatorPostcodeNotFound");
                            break;
                        }
                        marker = markerForPostcode[0];
                        results.Latitude = marker.lat;
                        Latitude = marker.lat;
                        results.Longitude = marker.lng;
                        Longitude = marker.lng;
                        break;

                    default:
                        results.Options = markerForPostcode.GetRange(0, (markerForPostcode.Count >= result) ? result : markerForPostcode.Count);
                        results.Latitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLatitude, CultureInfo.InvariantCulture);
                        results.Longitude = Convert.ToDecimal(WebConfiguration.Current.StoreLocatorLongitude, CultureInfo.InvariantCulture);
                        break;
                }
            }
            if (marker != null)
            {
                List<StoreLocatorRetailer> list2 = this.storelocatorrepository.GetStoreLocatorRetailers(marker, Convert.ToInt16(Distance), filterByBrandname);
                if (list2.Count > 0)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    results.Retailers = list2;
                    results.RetailersJSON = serializer.Serialize(list2);
                    results.StaticMapAddress = this.storelocatorrepository.GetStaticMap(marker, list2);
                }
                else
                {
                    results.Error = Helper.GetResource("StoreLocatorNoRetailers");
                }
                results.Location = this.getAddressFromLocation(marker);
                results.Latitude = marker.lat;
                results.Longitude = marker.lng;
            }
            this.SetComponentTypes(results);
            if (base.RouteData.Values.IsLevel1BrandFilterActivated())
            {
                results.Brand = base.RouteData.Values.GetLevel1BrandComponent();
                ((dynamic) base.ViewBag).StoreLocator = true;
            }
            return base.View(results);
        }

        private GoogleMapsMarker setAddressOnMarker(string address, char separator, GoogleMapsMarker marker)
        {
            string[] strArray = address.Split(new char[] { separator });
            int length = strArray.Length;
            if ((length > 0) && (strArray[0] != null))
            {
                marker.AddressLine1 = strArray[0];
            }
            if ((length > 1) && (strArray[1] != null))
            {
                marker.AddressLine2 = strArray[1];
            }
            if ((length > 2) && (strArray[2] != null))
            {
                marker.AddressLine3 = strArray[2];
            }
            if ((length > 3) && (strArray[3] != null))
            {
                marker.AddressLine4 = strArray[3];
            }
            if ((length > 4) && (strArray[4] != null))
            {
                marker.AddressLine5 = strArray[4];
            }
            if ((length > 5) && (strArray[5] != null))
            {
                marker.AddressLine6 = strArray[5];
            }
            if ((length > 6) && (strArray[6] != null))
            {
                marker.AddressLine7 = strArray[6];
            }
            return marker;
        }

        private void SetComponentTypes(StoreLocatorResults results)
        {
            results.ExtComponentSection = new ExtComponentSearchSection();
            results.ExtComponentSection.ComponentTypes = FacetedContentHelper.GetComponentSchemaTypes("StoreLocator");
        }

        public ILogger Logger { get; set; }
    }
}

