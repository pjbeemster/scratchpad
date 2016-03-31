namespace Coats.Crafts.Repositories.Interfaces
{
    using Coats.Crafts.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    public interface IStoreLocatorRepository
    {
        IEnumerable<SelectListItem> GetDistanceItems(string selectedItemVal);
        List<GoogleMapsMarker> GetMarkerForPostcode(string postcode, string bias = "");
        string GetStaticMap(GoogleMapsMarker currentLoc, List<StoreLocatorRetailer> retailers);
        List<StoreLocatorRetailer> GetStoreLocatorRetailers(GoogleMapsMarker currLocation, int radius, string filterByBrandname = "");
        IEnumerable<SelectListItem> GetWithinItems(string selectedItemVal = null);
    }
}

