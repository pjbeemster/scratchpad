using System;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
//using log4net;

namespace Coats.Crafts.Gateway
{
    public class ScrapbookGateway
    {
        //private static readonly ILog sLog = LogManager.GetLogger(typeof(EmailGateway));
        private static readonly object sPadlock = new object();
        private static volatile RetailersGateway _sInstance;

        public static RetailersGateway Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    lock (sPadlock)
                    {
                        if (_sInstance == null)
                        {
                            _sInstance = new RetailersGateway();
                        }
                    }
                }
                return _sInstance;
            }
        }

        public ScrapbookItem InsertScrapbookItem(ScrapbookItem item)
        {
            
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        item = client.InsertScrapbookItem(item);
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

        public bool DeleteScrapbookItem(Guid itemID, string userID)
        {
            bool success = false;
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        success = client.TryDeleteScrapbookItem(itemID, userID);
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

        public ScrapbookItem GetScrapbookItem(Guid itemID)
        {
            ScrapbookItem item = new ScrapbookItem();
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        item = client.GetScrapbookItem(itemID);
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

        public List<ScrapbookItem> GetScrapbookItemsForUser(string userID)
        {
            List<ScrapbookItem> items = new List<ScrapbookItem>();
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        items = client.GetScrapbookItemsByUser(userID).ToList();
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
    }
}
