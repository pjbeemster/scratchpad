namespace Coats.Crafts.Repositories.Interfaces
{
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;

    public interface IScrapbookRepository
    {
        ScrapbookItem GetScrapbookItem(Guid itemID);
        List<ScrapbookItem> GetScrapbookItemsForUser(string userID);
        ScrapbookItem InsertScrapbookItem(string userID, string imageURL, string itemDescription, string itemType, string sourceURL, string sourceDescription);
        bool TryDeleteScrapbookItem(Guid itemID, string userID);
    }
}

