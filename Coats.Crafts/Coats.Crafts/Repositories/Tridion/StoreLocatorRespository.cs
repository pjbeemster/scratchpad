using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Logging;
using Coats.Crafts.Data;
using Tridion.ContentDelivery.UGC.WebService;
using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using Coats.Crafts.Gateway;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Coats.Crafts.Configuration;
using Coats.Crafts.Models;
using DD4T.ContentModel;
using DD4T.Factories;
using DD4T.ContentModel.Factories;
using DD4T.Providers.WCFServices.CoatsTridionComponentServiceRef;
using Castle.Windsor;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Repositories.Tridion
{
    public class StoreLocatorRepository : IStoreLocatorRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;


        public StoreLocatorRepository(IAppSettings settings)
        {
            _settings = settings;
        }


        public string GetStaticMap(GoogleMapsMarker currentLoc, List<StoreLocatorRetailer> retailers)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            string mapsKeyParam = !String.IsNullOrEmpty(_settings.GoogleMapsAPIKey) ? "&key=" + _settings.GoogleMapsAPIKey : "";

            string mapReqAddress = _settings.StoreLocatorGoogleMapsBaseAddress + mapsKeyParam + "&center=" + currentLoc.lat.ToString() + "," + currentLoc.lng.ToString();

            for (int i = 0; i < retailers.Count; i++)
            {
                char markerLetter = ' ';
                if (!(i > alpha.Length - 1))
                {
                    markerLetter = alpha[i];
                }
                mapReqAddress += "&markers=label:" + markerLetter + "%7C" + retailers[i].RetailerFromWCF.Latitude + "," + retailers[i].RetailerFromWCF.Longitude;
            }

            //Add current location marker - NO LONGER REQUIRED
            //mapReqAddress += "&markers=color:blue|" + currentLoc.lat.ToString() + "," + currentLoc.lng.ToString();

            return mapReqAddress;
        }

        //public List<IntegrationServiceRetailer> GetRetailers(GoogleMapsMarker currLocation, int radius)
        //{

        //    //go get retailers
        //    RetailersGateway gw = new RetailersGateway();
        //    List<IntegrationServiceRetailer> retailers = gw.GetRetailersAndEventsInArea(currLocation.lat, currLocation.lng, radius);

        //    //create some json with markers for map
        //    var markers = retailers.Select(r => new GoogleMapsMarker(r.RetailerName, r.Latitude, r.Longitude));

        //    return retailers;
        //}

        public List<GoogleMapsMarker> GetMarkerForPostcode(string postcode, string bias = "")
        {
            var MarkerList = new List<GoogleMapsMarker>();
            string geoCodingServiceAddress = _settings.StoreLocatorGoogleGeoCodeBaseAddress + postcode;

            if (!String.IsNullOrEmpty(bias))
            { 
                geoCodingServiceAddress += "&region=" + bias + "&components=country:" + bias;
            }

            WebResponse response;

            //create geocode request
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(geoCodingServiceAddress);
                req.Method = "GET";
                response = req.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.Error("StoreLocatorRepository: Could not get Response from Geocoding service: " + ex);
                throw;
            }

            JObject result = null;

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                try
                {
                    result = JObject.Parse(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    Logger.Error("StoreLocatorRepository: Could not parse JSON returned from Google Geocode: " + ex);
                    throw;
                }
            }

            if (result != null)
            {
                string resultStatus = result.SelectToken("status").ToString();

                var results = result.SelectToken("results");

                switch (resultStatus)
                {
                    case "OK":

                        foreach (var res in results)
                        {
                            JToken typeComponent = res.SelectToken("types");
                            string resultType = GetSafeToken<string>(typeComponent, "[0]"); // result type

                            // Get the address component
                            JToken addressComponents = res.SelectToken("address_components");

                            // Break down elements and cast to strings
                            //
                            // ASH: Just added some token null checking
                            //string AddressLine1 = (string)addressComponents.SelectToken("[0].long_name"); // Street
                            //string AddressLine2 = (string)addressComponents.SelectToken("[1].long_name"); // Route
                            //string AddressLine3 = (string)addressComponents.SelectToken("[2].long_name"); // Locality
                            //string AddressLine4 = (string)addressComponents.SelectToken("[3].long_name"); // Administrative area 1
                            //string AddressLine5 = (string)addressComponents.SelectToken("[4].long_name"); // Administrative area 2
                            //string AddressLine6 = (string)addressComponents.SelectToken("[5].long_name"); // Country
                            //string AddressLine7 = (string)addressComponents.SelectToken("[6].long_name"); // Postcode
                            //
                            string AddressLine1 = GetSafeToken<string>(addressComponents, "[0].long_name"); // Street
                            string AddressLine2 = GetSafeToken<string>(addressComponents, "[1].long_name"); // Route
                            string AddressLine3 = GetSafeToken<string>(addressComponents, "[2].long_name"); // Locality
                            string AddressLine4 = GetSafeToken<string>(addressComponents, "[3].long_name"); // Administrative area 1
                            string AddressLine5 = GetSafeToken<string>(addressComponents, "[4].long_name"); // Administrative area 2
                            string AddressLine6 = GetSafeToken<string>(addressComponents, "[5].long_name"); // Country
                            string AddressLine7 = GetSafeToken<string>(addressComponents, "[6].long_name"); // Postcode

                            // Get the geometry component
                            //
                            // ASH: Done this loads of times myself - repeatedly using the first item in the collection.
                            //      Changed to get the geometry object from the current result (res).
                            //JToken geometry = result.SelectToken("results[0].geometry");
                            //
                            JToken geometry = res.SelectToken("geometry");

                            // Break down and cast to decimals
                            //
                            // ASH: Just added some token null checking
                            //decimal lat = (decimal)geometry.SelectToken("location.lat");
                            //decimal lng = (decimal)geometry.SelectToken("location.lng");
                            //
                            decimal lat = GetSafeToken<decimal>(geometry, "location.lat");
                            decimal lng = GetSafeToken<decimal>(geometry, "location.lng");

                            decimal dft = (decimal)GetDefaultValue(typeof(decimal));

                            //if (lat != null && lng != null)
                            if (lat != dft && lng != dft)
                            {
                                MarkerList.Add(
                                    new GoogleMapsMarker(
                                        postcode,
                                        lat,
                                        lng,
                                        AddressLine1,
                                        AddressLine2,
                                        AddressLine3,
                                        AddressLine4,
                                        AddressLine5,
                                        AddressLine6,
                                        AddressLine7,
                                        resultType
                                    )
                                );
                            }
                            else
                            {
                                Logger.Error("StoreLocatorRepository: No latitude/longitude found");
                                return null;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return MarkerList;
        }

        private T GetSafeToken<T>(JToken parent, string key)
        {
            try 
            {
                return parent.SelectToken(key).Value<T>();
            }
            catch(Exception) 
            { 
                return (T)GetDefaultValue(typeof(T)); 
            }
        }

        private object GetDefaultValue(Type type)
        {
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        public IEnumerable<SelectListItem> GetDistanceItems(string selectedItemVal)
        {

            List<SelectListItem> items = new List<SelectListItem>();
            if (_settings.StoreLocatorDistanceValues != string.Empty)
            {
                var valsStr = _settings.StoreLocatorDistanceValues.Split('|');

                foreach (string val in valsStr)
                {
                    SelectListItem item = new SelectListItem();

                    item.Text = String.Format(Helper.GetResource("StoreLocatorRadiusDropdownFormat"), val);
                    item.Value = val;
                    item.Selected = selectedItemVal == val;

                    items.Add(item);
                }
            }

            return items;

        }

        public IEnumerable<SelectListItem> GetWithinItems(string selectedItemVal = null)
        {

            List<SelectListItem> items = new List<SelectListItem>();
            if (!String.IsNullOrEmpty(_settings.StoreLocatorWithinVals))
            {
                var valsStr = _settings.StoreLocatorWithinVals.Split('|');

                int loopCount = 0;
                foreach (string val in valsStr)
                {
                    SelectListItem item = new SelectListItem();

                    //Try to get item name from labels (StoreLocatorWithin_ + countrycode) if not, set name to value
                    string itemName = String.IsNullOrEmpty(Helper.GetResource("StoreLocatorWithin_" + val)) ? val : Helper.GetResource("StoreLocatorWithin_" + val);

                    item.Text = itemName;
                    item.Value = val;

                    if (!String.IsNullOrEmpty(selectedItemVal))
                    {
                        item.Selected = selectedItemVal == val;
                    }
                    else {
                        //no selected item val passed in, so make first item selected
                        item.Selected = loopCount == 0;
                    }

                    items.Add(item);
                    loopCount++;
                }
            }

            return items;

        }

        private StoreLocatorRetailer AddExtendedInfoForStoreLocatorRetailer(StoreLocatorRetailer retailer)
        {
            
            if ((retailer.RetailerFromWCF.PublicationId != null) && (retailer.RetailerFromWCF.ItemId != null))
            {
                IComponent comp = GetComponentInfo(retailer.Tcm);
                if (comp != null)
                {
                    if (comp.Fields.ContainsKey("body"))
                    {
                        string desc = comp.Fields["body"].Value.ResolveRichText().ToHtmlString();
                        retailer.Description = desc;
                    }
                    else
                    {
                        retailer.Description = "";
                    }
                    
                    //Images
                    if (comp.Fields.ContainsKey("banner"))
                    {
                        if (comp.Fields["banner"].LinkedComponentValues.First().Fields.ContainsKey("imageTextCollection"))
                        {
                            var images = comp.Fields["banner"].LinkedComponentValues.First().Fields["imageTextCollection"].EmbeddedValues;
                            if (images.Count > 0)
                            {
                                List<RetailerExtendedInfoImage> imageList = new List<RetailerExtendedInfoImage>();
                                foreach (var image in images)
                                {
                                    RetailerExtendedInfoImage reiImage = new RetailerExtendedInfoImage();
                                    reiImage.imageURL = image.ContainsKey("image") ? image["image"].LinkedComponentValues[0].Multimedia.Url : string.Empty;
                                    reiImage.imageTitle = image.ContainsKey("title") ? image["title"].Value.ToString() : string.Empty;
                                    imageList.Add(reiImage);
                                }
                                retailer.Images = imageList;
                            }
                        }
                        else
                        {
                            var image = comp.Fields["banner"].LinkedComponentValues[0];

                            if (image.Multimedia != null)
                            {
                                List<RetailerExtendedInfoImage> imageList = new List<RetailerExtendedInfoImage>();
                                RetailerExtendedInfoImage reiImage = new RetailerExtendedInfoImage();
                                reiImage.imageURL = image.Multimedia.Url;
                                reiImage.imageTitle = image.MetadataFields.ContainsKey("alt") ? image.MetadataFields["alt"].Value : string.Empty;
                                imageList.Add(reiImage);
                                retailer.Images = imageList;
                            }
                        }
                            
                    }
                    //Promos
                    if (comp.Fields.ContainsKey("promotions"))
                    {
                        var promos = comp.Fields["promotions"].LinkedComponentValues;
                        if (promos.Count > 0)
                        {
                            List<RetailerExtendedInfoPromo> promoList = new List<RetailerExtendedInfoPromo>();
                            foreach (var promo in promos)
                            {
                                var promoFields = promo.Fields;

                                RetailerExtendedInfoPromo reiPromo = new RetailerExtendedInfoPromo();
                                reiPromo.title = promoFields.ContainsKey("title") ? promoFields["title"].Value.ToString() : String.Empty;
                                reiPromo.description = promoFields.ContainsKey("summary") ? promoFields["summary"].Value.ToString() : String.Empty;
                                reiPromo.img = promoFields.ContainsKey("image") ? promoFields["image"] : null;

                                if (promoFields.ContainsKey("link"))
                                {
                                    IFieldSet cta = promoFields["link"].EmbeddedValues[0];

                                    if (cta.ContainsKey("linkComponent"))
                                    {
                                        if (cta["linkComponent"].LinkedComponentValues[0].Multimedia != null)
                                        {
                                            reiPromo.buttonURL = cta["linkComponent"].LinkedComponentValues[0].Multimedia.Url;
                                        }
                                        else
                                        {
                                            reiPromo.buttonURL = cta["linkComponent"].LinkedComponentValues[0].Url;
                                        }
                                    }

                                    reiPromo.buttonText = cta.ContainsKey("linkTitle") ? cta["linkTitle"].Value.ToString() : string.Empty;
                                    reiPromo.buttonTarget = cta.ContainsKey("boolNewWindow") ? cta["boolNewWindow"].Value : string.Empty;
                                    reiPromo.buttonLinkType = cta.ContainsKey("linkComponent") ? cta["linkComponent"].LinkedComponentValues[0].ComponentType.ToString() : string.Empty;
                                }
                                promoList.Add(reiPromo);
                            }
                            retailer.Promos = promoList;
                        }
                    }
                }
            }

            return retailer;
        }

        public List<StoreLocatorRetailer> GetStoreLocatorRetailers(GoogleMapsMarker currentLocation, int radius, string filterByBrandname = "")
        {
            
            //default vals, these are overidden by web.config vals
            int maxRetailers = 50;
            int eventsWithinDays = 10;
            int maxEventsPerRetailer = 5;
            int.TryParse(_settings.StoreLocatorShowEventsInNextDays, out eventsWithinDays);
            int.TryParse(_settings.StoreLocatorMaxResults, out maxRetailers);
            int.TryParse(_settings.StoreLocatorMaxEventsPerRetailer, out maxEventsPerRetailer);

            //go get retailers
            RetailersGateway gw = new RetailersGateway();
            int publicationID = WebConfiguration.Current.PublicationId;
            List<IntegrationServiceRetailer> retailers = gw.GetRetailersAndEventsInArea(publicationID, currentLocation.lat, currentLocation.lng, radius, DateTime.Now, DateTime.Now.AddDays(eventsWithinDays), maxEventsPerRetailer, maxRetailers, filterByBrandname);
            List<StoreLocatorRetailer> slRetailers = new List<StoreLocatorRetailer>();

            foreach (IntegrationServiceRetailer r in retailers)
            {
                StoreLocatorRetailer slRetailer = new StoreLocatorRetailer();
                slRetailer.RetailerFromWCF = r;
                slRetailer = AddExtendedInfoForStoreLocatorRetailer(slRetailer);
                slRetailer.isPromotedRetailer = false;
                slRetailers.Add(slRetailer);
            }

            // Get first retailer in results that has promotions
            var promotedRetailer = slRetailers.Where(a => a.RetailerFromWCF.HasPromotion == true).FirstOrDefault();
            if (promotedRetailer != null)
                promotedRetailer.isPromotedRetailer = true;

            return slRetailers;
        }

        private IComponent GetComponentInfo(string tcm)
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var factory = accessor.Container.Resolve<IComponentFactory>();

            IComponent c = factory.GetComponent(tcm);
            return c;
        }


    }
}