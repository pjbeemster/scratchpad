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
using Coats.Crafts.ControllerHelpers;

namespace Coats.Crafts.Repositories.Tridion
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;
        ShoppingListGateway gw = new ShoppingListGateway();


        public ShoppingListRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        public ShoppingListItem InsertShoppingListItem(ShoppingListItem item)
        {
            return gw.InsertShoppingListItem(item);
        }

        public bool TryDeleteShoppingListItem(Guid ID, string userID)
        {
            return gw.TryDeleteShoppingListItem(ID, userID);
        }

        public bool TryDeleteShoppingListItemsByProjectName(string projectName, string userID)
        {
            return gw.TryDeleteShoppingListItemsByProjectName(projectName, userID);
        }

        public IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProject(string userID)
        {
            IEnumerable<ShoppingListItem> items = gw.GetShoppingListItems(userID);
            var groupedItems = items.GroupBy(p => p.ProjectName);
            return groupedItems;
        }

        public IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProduct(string userID)
        {
            IEnumerable<ShoppingListItem> items = gw.GetShoppingListItems(userID);
            var groupedItems = items.GroupBy(p => p.ProductTcmID);
            return groupedItems;
        }

        public ShoppingListItem GetShoppingListItem(Guid itemID)
        {
            return gw.GetShoppingListItem(itemID);
        }

        public ShoppingListItem UpdateShoppingListItemQuantity(Guid itemID, string username, int alterQuantityBy)
        {
            return gw.UpdateShoppingListItemQuantity(itemID, username, alterQuantityBy);
        }

        public ShoppingListItem UpdateAndInsertItem(ShoppingListItem item) {
            item.AddedDateTime = DateTime.Now;
            IComponent product = GetComponentInfo(item.ProductTcmID);
            item.ProductName = product.Fields["title"].Value;
            item.Brand = product.Fields["brand"].Value;
            InsertShoppingListItem(item);
            return item;
        }

        public bool SendShoppingListEmail(ShoppingListEmail email)
        {
            bool success = false;
            var util = new EmailUtility();
            try
            {
                util.SendEmail(email, _settings.ShoppingListEmailTemplate, _settings.ShoppingListEmailFrom, email.EmailAddress);
                success = true;
            }
            catch {
                throw;
            }
            return success;
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