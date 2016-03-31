namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Data;
    using Coats.Crafts.Gateway;
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.HtmlHelpers;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.Mvc;

    public class StoreLocatorRepository : IStoreLocatorRepository
    {
        private IAppSettings _settings;

        public StoreLocatorRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        private StoreLocatorRetailer AddExtendedInfoForStoreLocatorRetailer(StoreLocatorRetailer retailer)
        {
            int publicationId = retailer.RetailerFromWCF.PublicationId;
            int itemId = retailer.RetailerFromWCF.ItemId;
            IComponent componentInfo = this.GetComponentInfo(retailer.Tcm);
            if (componentInfo != null)
            {
                if (componentInfo.Fields.ContainsKey("body"))
                {
                    string str = componentInfo.Fields["body"].Value.ResolveRichText().ToHtmlString();
                    retailer.Description = str;
                }
                else
                {
                    retailer.Description = "";
                }
                if (componentInfo.Fields.ContainsKey("banner"))
                {
                    List<RetailerExtendedInfoImage> list2;
                    RetailerExtendedInfoImage image;
                    if (componentInfo.Fields["banner"].LinkedComponentValues.First<IComponent>().Fields.ContainsKey("imageTextCollection"))
                    {
                        IList<IFieldSet> embeddedValues = componentInfo.Fields["banner"].LinkedComponentValues.First<IComponent>().Fields["imageTextCollection"].EmbeddedValues;
                        if (embeddedValues.Count > 0)
                        {
                            list2 = new List<RetailerExtendedInfoImage>();
                            foreach (IFieldSet set in embeddedValues)
                            {
                                image = new RetailerExtendedInfoImage {
                                    imageURL = set.ContainsKey("image") ? set["image"].LinkedComponentValues[0].Multimedia.Url : string.Empty,
                                    imageTitle = set.ContainsKey("title") ? set["title"].Value.ToString() : string.Empty
                                };
                                list2.Add(image);
                            }
                            retailer.Images = list2;
                        }
                    }
                    else
                    {
                        IComponent component2 = componentInfo.Fields["banner"].LinkedComponentValues[0];
                        if (component2.Multimedia != null)
                        {
                            list2 = new List<RetailerExtendedInfoImage>();
                            image = new RetailerExtendedInfoImage {
                                imageURL = component2.Multimedia.Url,
                                imageTitle = component2.MetadataFields.ContainsKey("alt") ? component2.MetadataFields["alt"].Value : string.Empty
                            };
                            list2.Add(image);
                            retailer.Images = list2;
                        }
                    }
                }
                if (componentInfo.Fields.ContainsKey("promotions"))
                {
                    IList<IComponent> linkedComponentValues = componentInfo.Fields["promotions"].LinkedComponentValues;
                    if (linkedComponentValues.Count <= 0)
                    {
                        return retailer;
                    }
                    List<RetailerExtendedInfoPromo> list4 = new List<RetailerExtendedInfoPromo>();
                    foreach (IComponent component3 in linkedComponentValues)
                    {
                        IFieldSet fields = component3.Fields;
                        RetailerExtendedInfoPromo item = new RetailerExtendedInfoPromo {
                            title = fields.ContainsKey("title") ? fields["title"].Value.ToString() : string.Empty,
                            description = fields.ContainsKey("summary") ? fields["summary"].Value.ToString() : string.Empty,
                            img = fields.ContainsKey("image") ? fields["image"] : null
                        };
                        if (fields.ContainsKey("link"))
                        {
                            IFieldSet set3 = fields["link"].EmbeddedValues[0];
                            if (set3.ContainsKey("linkComponent"))
                            {
                                if (set3["linkComponent"].LinkedComponentValues[0].Multimedia != null)
                                {
                                    item.buttonURL = set3["linkComponent"].LinkedComponentValues[0].Multimedia.Url;
                                }
                                else
                                {
                                    item.buttonURL = set3["linkComponent"].LinkedComponentValues[0].Url;
                                }
                            }
                            item.buttonText = set3.ContainsKey("linkTitle") ? set3["linkTitle"].Value.ToString() : string.Empty;
                            item.buttonTarget = set3.ContainsKey("boolNewWindow") ? set3["boolNewWindow"].Value : string.Empty;
                            item.buttonLinkType = set3.ContainsKey("linkComponent") ? set3["linkComponent"].LinkedComponentValues[0].ComponentType.ToString() : string.Empty;
                        }
                        list4.Add(item);
                    }
                    retailer.Promos = list4;
                }
            }
            return retailer;
        }

        private IComponent GetComponentInfo(string tcm)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IComponentFactory>().GetComponent(tcm);
        }

        private object GetDefaultValue(Type type)
        {
            if (!(type.IsValueType && (Nullable.GetUnderlyingType(type) == null)))
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        public IEnumerable<SelectListItem> GetDistanceItems(string selectedItemVal)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (this._settings.StoreLocatorDistanceValues != string.Empty)
            {
                string[] strArray = this._settings.StoreLocatorDistanceValues.Split(new char[] { '|' });
                foreach (string str in strArray)
                {
                    SelectListItem item = new SelectListItem {
                        Text = string.Format(Helper.GetResource("StoreLocatorRadiusDropdownFormat"), str),
                        Value = str,
                        Selected = selectedItemVal == str
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public List<GoogleMapsMarker> GetMarkerForPostcode(string postcode, string bias = "")
        {
            WebResponse response;
            Exception exception;
            List<GoogleMapsMarker> list = new List<GoogleMapsMarker>();
            string requestUriString = this._settings.StoreLocatorGoogleGeoCodeBaseAddress + postcode;
            if (!string.IsNullOrEmpty(bias))
            {
                string str11 = requestUriString;
                requestUriString = str11 + "&region=" + bias + "&components=country:" + bias;
            }
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestUriString);
                request.Method = "GET";
                response = request.GetResponse();
            }
            catch (Exception exception1)
            {
                exception = exception1;
                this.Logger.Error("StoreLocatorRepository: Could not get Response from Geocoding service: " + exception);
                throw;
            }
            JObject obj2 = null;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    try
                    {
                        obj2 = JObject.Parse(reader.ReadToEnd());
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        this.Logger.Error("StoreLocatorRepository: Could not parse JSON returned from Google Geocode: " + exception);
                        throw;
                    }
                }
            }
            if (obj2 != null)
            {
                string str2 = obj2.SelectToken("status").ToString();
                JToken token = obj2.SelectToken("results");
                string str12 = str2;
                if ((str12 != null) && (str12 == "OK"))
                {
                    foreach (JToken token2 in (IEnumerable<JToken>) token)
                    {
                        JToken parent = token2.SelectToken("types");
                        string safeToken = this.GetSafeToken<string>(parent, "[0]");
                        JToken token4 = token2.SelectToken("address_components");
                        string str4 = this.GetSafeToken<string>(token4, "[0].long_name");
                        string str5 = this.GetSafeToken<string>(token4, "[1].long_name");
                        string str6 = this.GetSafeToken<string>(token4, "[2].long_name");
                        string str7 = this.GetSafeToken<string>(token4, "[3].long_name");
                        string str8 = this.GetSafeToken<string>(token4, "[4].long_name");
                        string str9 = this.GetSafeToken<string>(token4, "[5].long_name");
                        string str10 = this.GetSafeToken<string>(token4, "[6].long_name");
                        JToken token5 = token2.SelectToken("geometry");
                        decimal lat = this.GetSafeToken<decimal>(token5, "location.lat");
                        decimal lng = this.GetSafeToken<decimal>(token5, "location.lng");
                        decimal defaultValue = (decimal) this.GetDefaultValue(typeof(decimal));
                        if ((lat != defaultValue) && (lng != defaultValue))
                        {
                            list.Add(new GoogleMapsMarker(postcode, lat, lng, str4, str5, str6, str7, str8, str9, str10, safeToken));
                        }
                        else
                        {
                            this.Logger.Error("StoreLocatorRepository: No latitude/longitude found");
                            return null;
                        }
                    }
                }
            }
            return list;
        }

        private T GetSafeToken<T>(JToken parent, string key)
        {
            try
            {
                return parent.SelectToken(key).Value<T>();
            }
            catch (Exception)
            {
                return (T) this.GetDefaultValue(typeof(T));
            }
        }

        public string GetStaticMap(GoogleMapsMarker currentLoc, List<StoreLocatorRetailer> retailers)
        {
            char[] chArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string str = !string.IsNullOrEmpty(this._settings.GoogleMapsAPIKey) ? ("&key=" + this._settings.GoogleMapsAPIKey) : "";
            string str2 = this._settings.StoreLocatorGoogleMapsBaseAddress + str + "&center=" + currentLoc.lat.ToString() + "," + currentLoc.lng.ToString();
            for (int i = 0; i < retailers.Count; i++)
            {
                char ch = ' ';
                if (i <= (chArray.Length - 1))
                {
                    ch = chArray[i];
                }
                object obj2 = str2;
                str2 = string.Concat(new object[] { obj2, "&markers=label:", ch, "%7C", retailers[i].RetailerFromWCF.Latitude, ",", retailers[i].RetailerFromWCF.Longitude });
            }
            return str2;
        }

        public List<StoreLocatorRetailer> GetStoreLocatorRetailers(GoogleMapsMarker currentLocation, int radius, string filterByBrandname = "")
        {
            int result = 50;
            int num2 = 10;
            int num3 = 5;
            int.TryParse(this._settings.StoreLocatorShowEventsInNextDays, out num2);
            int.TryParse(this._settings.StoreLocatorMaxResults, out result);
            int.TryParse(this._settings.StoreLocatorMaxEventsPerRetailer, out num3);
            RetailersGateway gateway = new RetailersGateway();
            int publicationId = WebConfiguration.Current.PublicationId;
            List<IntegrationServiceRetailer> list = gateway.GetRetailersAndEventsInArea(publicationId, currentLocation.lat, currentLocation.lng, radius, DateTime.Now, DateTime.Now.AddDays((double) num2), num3, result, filterByBrandname);
            List<StoreLocatorRetailer> list2 = new List<StoreLocatorRetailer>();
            foreach (IntegrationServiceRetailer retailer in list)
            {
                StoreLocatorRetailer retailer2 = new StoreLocatorRetailer {
                    RetailerFromWCF = retailer
                };
                retailer2 = this.AddExtendedInfoForStoreLocatorRetailer(retailer2);
                retailer2.isPromotedRetailer = false;
                list2.Add(retailer2);
            }
            StoreLocatorRetailer retailer3 = (from a in list2
                where a.RetailerFromWCF.HasPromotion
                select a).FirstOrDefault<StoreLocatorRetailer>();
            if (retailer3 != null)
            {
                retailer3.isPromotedRetailer = true;
            }
            return list2;
        }

        public IEnumerable<SelectListItem> GetWithinItems(string selectedItemVal = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(this._settings.StoreLocatorWithinVals))
            {
                string[] strArray = this._settings.StoreLocatorWithinVals.Split(new char[] { '|' });
                int num = 0;
                foreach (string str in strArray)
                {
                    SelectListItem item = new SelectListItem();
                    string str2 = string.IsNullOrEmpty(Helper.GetResource("StoreLocatorWithin_" + str)) ? str : Helper.GetResource("StoreLocatorWithin_" + str);
                    item.Text = str2;
                    item.Value = str;
                    if (!string.IsNullOrEmpty(selectedItemVal))
                    {
                        item.Selected = selectedItemVal == str;
                    }
                    else
                    {
                        item.Selected = num == 0;
                    }
                    list.Add(item);
                    num++;
                }
            }
            return list;
        }

        public ILogger Logger { get; set; }
    }
}

