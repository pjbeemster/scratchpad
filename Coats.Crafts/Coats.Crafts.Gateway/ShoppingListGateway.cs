using System;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
//using log4net;

namespace Coats.Crafts.Gateway
{
    public class ShoppingListGateway
    {
        //private static readonly ILog sLog = LogManager.GetLogger(typeof(EmailGateway));
        private static readonly object sPadlock = new object();
        private static volatile ShoppingListGateway _sInstance;

        public static ShoppingListGateway Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    lock (sPadlock)
                    {
                        if (_sInstance == null)
                        {
                            _sInstance = new ShoppingListGateway();
                        }
                    }
                }
                return _sInstance;
            }
        }

        public ShoppingListItem InsertShoppingListItem(ShoppingListItem item)
        {
            
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        item = client.InsertShoppingListItem(item);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return item;
        }

        public bool TryDeleteShoppingListItem(Guid ID, string userID) {
            bool success = false;
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        success = client.TryDeleteShoppingListItem(ID, userID);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return success;
        }

        public bool TryDeleteShoppingListItemsByProjectName(string projectName, string userID)
        {
            bool success = false;
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        success = client.TryDeleteShoppingListItemsByProjectName(projectName, userID);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return success;
        }

        public IEnumerable<ShoppingListItem> GetShoppingListItems(string userID)
        {
            IEnumerable<ShoppingListItem> items;
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        items = client.GetShoppingListItems(userID).ToList();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return items;
        }

        public ShoppingListItem GetShoppingListItem(Guid itemID)
        {
            ShoppingListItem item = new ShoppingListItem();
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        item = client.GetShoppingListItem(itemID);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return item;
        }

        public ShoppingListItem UpdateShoppingListItemQuantity(Guid itemID, string username, int alterQuantityBy)
        {
            ShoppingListItem item = new ShoppingListItem();
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        item = client.UpdateShoppingListItemQuantity(itemID, username, alterQuantityBy);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return item;
        }
    }
}
