namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Coats.Crafts.Gateway;
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web;

    public class ShoppingListRepository : IShoppingListRepository
    {
        private IAppSettings _settings;
        private ShoppingListGateway gw = new ShoppingListGateway();

        public ShoppingListRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        private IComponent GetComponentInfo(string tcm)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IComponentFactory>().GetComponent(tcm);
        }

        public ShoppingListItem GetShoppingListItem(Guid itemID)
        {
            return this.gw.GetShoppingListItem(itemID);
        }

        public IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProduct(string userID)
        {
            return (from p in this.gw.GetShoppingListItems(userID) group p by p.ProductTcmID);
        }

        public IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProject(string userID)
        {
            return (from p in this.gw.GetShoppingListItems(userID) group p by p.ProjectName);
        }

        public ShoppingListItem InsertShoppingListItem(ShoppingListItem item)
        {
            return this.gw.InsertShoppingListItem(item);
        }

        public bool SendShoppingListEmail(ShoppingListEmail email)
        {
            bool flag = false;
            EmailUtility utility = new EmailUtility();
            try
            {
                utility.SendEmail(email, this._settings.ShoppingListEmailTemplate, this._settings.ShoppingListEmailFrom, email.EmailAddress);
                flag = true;
            }
            catch
            {
                throw;
            }
            return flag;
        }

        public bool TryDeleteShoppingListItem(Guid ID, string userID)
        {
            return this.gw.TryDeleteShoppingListItem(ID, userID);
        }

        public bool TryDeleteShoppingListItemsByProjectName(string projectName, string userID)
        {
            return this.gw.TryDeleteShoppingListItemsByProjectName(projectName, userID);
        }

        public ShoppingListItem UpdateAndInsertItem(ShoppingListItem item)
        {
            item.AddedDateTime = DateTime.Now;
            IComponent componentInfo = this.GetComponentInfo(item.ProductTcmID);
            item.ProductName = componentInfo.Fields["title"].Value;
            item.Brand = componentInfo.Fields["brand"].Value;
            this.InsertShoppingListItem(item);
            return item;
        }

        public ShoppingListItem UpdateShoppingListItemQuantity(Guid itemID, string username, int alterQuantityBy)
        {
            return this.gw.UpdateShoppingListItemQuantity(itemID, username, alterQuantityBy);
        }

        public ILogger Logger { get; set; }
    }
}

