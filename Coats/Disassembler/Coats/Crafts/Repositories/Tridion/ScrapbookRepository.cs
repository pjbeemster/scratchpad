namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Gateway;
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ScrapbookRepository : IScrapbookRepository
    {
        private IAppSettings _settings;

        public ScrapbookRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public ScrapbookItem GetScrapbookItem(Guid itemID)
        {
            ScrapbookGateway gateway = new ScrapbookGateway();
            return gateway.GetScrapbookItem(itemID);
        }

        public List<ScrapbookItem> GetScrapbookItemsForUser(string userID)
        {
            ScrapbookGateway gateway = new ScrapbookGateway();
            return gateway.GetScrapbookItemsForUser(userID);
        }

        public ScrapbookItem InsertScrapbookItem(string userID, string imageURL, string itemDescription, string itemType, string sourceURL, string sourceDescription)
        {
            ScrapbookItem item = new ScrapbookItem {
                UserID = userID,
                ImageURL = imageURL,
                ItemDescription = itemDescription,
                ItemType = itemType,
                SourceURL = sourceURL,
                SourceDescription = sourceDescription
            };
            ScrapbookGateway gateway = new ScrapbookGateway();
            return gateway.InsertScrapbookItem(item);
        }

        public bool TryDeleteScrapbookItem(Guid itemID, string userID)
        {
            ScrapbookGateway gateway = new ScrapbookGateway();
            return gateway.DeleteScrapbookItem(itemID, userID);
        }

        public ILogger Logger { get; set; }
    }
}

