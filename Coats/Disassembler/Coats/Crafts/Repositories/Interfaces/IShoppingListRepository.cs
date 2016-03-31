namespace Coats.Crafts.Repositories.Interfaces
{
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public interface IShoppingListRepository
    {
        ShoppingListItem GetShoppingListItem(Guid itemID);
        IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProduct(string userID);
        IEnumerable<IGrouping<string, ShoppingListItem>> GetShoppingListItemsByProject(string userID);
        ShoppingListItem InsertShoppingListItem(ShoppingListItem item);
        bool SendShoppingListEmail(ShoppingListEmail email);
        bool TryDeleteShoppingListItem(Guid ID, string userID);
        bool TryDeleteShoppingListItemsByProjectName(string projectName, string userID);
        ShoppingListItem UpdateAndInsertItem(ShoppingListItem item);
        ShoppingListItem UpdateShoppingListItemQuantity(Guid itemID, string username, int alterQuantityBy);
    }
}

