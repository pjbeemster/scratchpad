using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using DD4T.ContentModel;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IStoreLocatorRepository
    {
        List<GoogleMapsMarker> GetMarkerForPostcode(string postcode, string bias = "");
        List<StoreLocatorRetailer> GetStoreLocatorRetailers(GoogleMapsMarker currLocation, int radius, string filterByBrandname = "");
        string GetStaticMap(GoogleMapsMarker currentLoc, List<StoreLocatorRetailer> retailers);
        IEnumerable<SelectListItem> GetDistanceItems(string selectedItemVal);
        IEnumerable<SelectListItem> GetWithinItems(string selectedItemVal = null);
        //RetailerExtendedInfo GetExtendedInfo(List<Retailer> retailers);
    }
}
