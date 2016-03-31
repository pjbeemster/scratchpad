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
    public interface IScrapbookRepository
    {
        List<ScrapbookItem> GetScrapbookItemsForUser(string userID);
        ScrapbookItem GetScrapbookItem(Guid itemID);
        bool TryDeleteScrapbookItem(Guid itemID, string userID);
        ScrapbookItem InsertScrapbookItem(string userID, string imageURL, string itemDescription, string itemType, string sourceURL, string sourceDescription);
    }
}
