using System.Collections.Generic;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Models;


namespace Coats.Crafts.Repositories.Interfaces
{
    public interface ILocationRespository
    {
        IList<Locations> GetLocations(string identifier);
        string GetLocationName(string identifier);
        string GetLocationAdmin(string identifier, string action);
        string GetLocationPath(string locationKeywordTcmId, string seperator = "/");

        Locations GetLocationFromKeywordTcmId(string locationKeywordTcmId);

        List<GroupedSelectListItem> GetLocationGroupedSelectListItem();
        List<GroupedSelectListItem> GetLocationGroupedSelectListItem(string selectedLocation);
    }
}