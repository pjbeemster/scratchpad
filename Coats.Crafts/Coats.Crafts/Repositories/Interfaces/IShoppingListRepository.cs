using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Web.Mvc;
using DD4T.ContentModel;
using Coats.Crafts.Models;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IShoppingListRepository
    {
        ShoppingListItem InsertShoppingListItem(ShoppingListItem item);
        bool TryDeleteShoppingListItem(Guid ID, string userID);
        bool TryDeleteShoppingListItemsByProjectName(string projectName, string userID);
        IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProject(string userID);
        IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProduct(string userID);
        ShoppingListItem GetShoppingListItem(Guid itemID);
        ShoppingListItem UpdateShoppingListItemQuantity(Guid itemID, string username, int alterQuantityBy);
        ShoppingListItem UpdateAndInsertItem(ShoppingListItem item);
        bool SendShoppingListEmail(ShoppingListEmail email);
    }
}
