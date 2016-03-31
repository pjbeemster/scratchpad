using System;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Coats.Crafts.Gateway
{
    public class RetailersGateway
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

        public List<IntegrationServiceRetailer> GetRetailersAndEventsInArea(int publicationID,decimal lat, decimal lng, int radius, DateTime startDate, DateTime endDate, int maxEvents, int maxRetailers, string filterByBrandName = "")
        {
            List<IntegrationServiceRetailer> retailers = new List<IntegrationServiceRetailer>();

            //Logger.DebugFormat("RetailersGateway - GetRetailersAndEventsInArea -  publicationID {0}, lat {1}, lng {2}, radius {3}, startDate {4}, endDate {5}, maxEvents {6}, maxRetailers {7} ", publicationID, lat, lng, radius, startDate.ToString(), endDate.ToString(), maxEvents, maxRetailers);
            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        if (ConfigurationManager.AppSettings["StoreLocatorUseMilesForDistanceUnit"] == "true")
                        {
                            // retailers = client.GetRetailersAndEventsInArea(lng, lat, radius, startDate, endDate, true, maxEvents, maxRetailers).ToList();
                            retailers = client.GetRetailersAndEventsInArea(publicationID,lng.ToString(), lat.ToString(), radius, DateTime.Now, true, maxEvents, maxRetailers, filterByBrandName).ToList();
                        }
                        else {
                            // retailers = client.GetRetailersAndEventsInArea(lng, lat, radius, startDate, endDate, false, maxEvents, maxRetailers).ToList();
                            retailers = client.GetRetailersAndEventsInArea(publicationID, lng.ToString(), lat.ToString(), radius, DateTime.Now, false, maxEvents, maxRetailers, filterByBrandName).ToList();
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        //sLog.Error(string.Format("{0} {1}", LOGMETHODNAME, ex));
                    }
                }
            }
            catch (Exception ex)
            {
                //sLog.Error(string.Format("{0} {1}", LOGMETHODNAME, ex));
            }


            return retailers;
        }

        public List<IntegrationServiceRetailer> GetRetailersInArea(int publicationID, decimal lat, decimal lng, int radius, int maxResults)
        {
            List<IntegrationServiceRetailer> retailers = new List<IntegrationServiceRetailer>();

            //Logger.DebugFormat("RetailersGateway - GetRetailersInArea -  publicationID {0}, lat {1}, lng {2}, radius {3}, maxResults {3} ", publicationID, lat, lng, radius, maxResults);

            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        if (ConfigurationManager.AppSettings["StoreLocatorUseMilesForDistanceUnit"] == "true")
                        {
                            retailers = client.GetRetailersInArea(publicationID,lng.ToString(), lat.ToString(), radius, true, maxResults).ToList();
                        }
                        else
                        {
                            retailers = client.GetRetailersInArea(publicationID,lng.ToString(), lat.ToString(), radius, false, maxResults).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error(string.Format("{0} {1}", "GetRetailersInArea", ex));
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(string.Format("{0} {1}", "GetRetailersInArea", ex));
            }

            return retailers;
        }

        public List<CraftsEvent> GetCraftsEventsInArea(int publicationID,decimal lat, decimal lng, int radius, DateTime startDate, DateTime endDate, int maxResults)
        {
            List<CraftsEvent> events = new List<CraftsEvent>();

            //Logger.DebugFormat("RetailersGateway - GetCraftsEventsInArea -  publicationID {0}, lat {1}, lng {2}, radius {3}, maxResults {4} ", publicationID, lat, lng, radius, maxResults);

            try
            {
                using (var client = new CoatsCraftsIntegrationServiceContractClient())
                {
                    try
                    {
                        if (ConfigurationManager.AppSettings["StoreLocatorUseMilesForDistanceUnit"] == "true")
                        {
                            // events = client.GetEventsInArea(lng, lat, radius, startDate, endDate, true, maxResults).ToList();
                            events = client.GetEventsInArea(publicationID,lng.ToString(), lat.ToString(), radius, DateTime.Now, true, maxResults).ToList();
                        }
                        else
                        {
                            // events = client.GetEventsInArea(lng, lat, radius, startDate, endDate, false, maxResults).ToList();
                            events = client.GetEventsInArea(publicationID, lng.ToString(), lat.ToString(), radius, DateTime.Now, false, maxResults).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", ex));
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", ex));
            }

            return events;
        }
    }
}
