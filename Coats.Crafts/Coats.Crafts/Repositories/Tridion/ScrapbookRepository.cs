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

namespace Coats.Crafts.Repositories.Tridion
{
    public class ScrapbookRepository : IScrapbookRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;
        

        public ScrapbookRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        public List<ScrapbookItem> GetScrapbookItemsForUser(string userID)
        {
            ScrapbookGateway sg = new ScrapbookGateway();
            return sg.GetScrapbookItemsForUser(userID);
        }

        public ScrapbookItem GetScrapbookItem(Guid itemID)
        {
            ScrapbookGateway sg = new ScrapbookGateway();
            return sg.GetScrapbookItem(itemID);
        }

        public bool TryDeleteScrapbookItem(Guid itemID, string userID)
        {
            ScrapbookGateway sg = new ScrapbookGateway();
            return sg.DeleteScrapbookItem(itemID, userID);
        }

        public ScrapbookItem InsertScrapbookItem(string userID, string imageURL, string itemDescription, string itemType, string sourceURL, string sourceDescription) {
            ScrapbookItem item = new ScrapbookItem();
            item.UserID = userID;
            item.ImageURL = imageURL;
            item.ItemDescription = itemDescription;
            item.ItemType = itemType;
            item.SourceURL = sourceURL;
            item.SourceDescription = sourceDescription;

            ScrapbookGateway sg = new ScrapbookGateway();
            return sg.InsertScrapbookItem(item);
        }
    }
}