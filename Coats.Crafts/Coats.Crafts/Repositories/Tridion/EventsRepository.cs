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
using System.Globalization;

namespace Coats.Crafts.Repositories.Tridion
{
    public class EventsRepository : IEventsRepository
    {
        public ILogger Logger { get; set; }
        private IAppSettings _settings;

        public EventsRepository(IAppSettings settings)
        {
            _settings = settings;
        }

        // TODO Is this needed?
        public List<CraftsEvent> GetCraftsEventsInArea()
        {
            List<CraftsEvent> events = new List<CraftsEvent>();

            try
            {
                string defaultLong = WebConfiguration.Current.DefaultLong; // + "M"
                string defaultLat = WebConfiguration.Current.DefaultLat; // + "M"
                int publicationID = WebConfiguration.Current.PublicationId;

                decimal lat = Convert.ToDecimal(defaultLat, CultureInfo.InvariantCulture);
                decimal lng = Convert.ToDecimal(defaultLong, CultureInfo.InvariantCulture);
                int radius = 1000;
                DateTime today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                var startDate = month.AddMonths(-11);
                var endDate = month.AddMonths(+11);
                int maxResults = 2;


                RetailersGateway sg = new RetailersGateway();
                events = sg.GetCraftsEventsInArea(publicationID, lat, lng, radius, startDate, endDate, maxResults);

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", ex));
            }

            return events;
        }


        public List<CraftsEvent> GetCraftsEventsInArea(decimal lat, decimal lng, int radius, int maxResults)
        {

            Logger.Debug("Events repository - GetCraftsEventsInArea");

            Logger.DebugFormat("Events repository - GetCraftsEventsInArea -  lat {0}, lng {1}, radius{2}, maxResults {3} ", lat.ToString(), lng.ToString(), radius.ToString(), maxResults.ToString());

            List<CraftsEvent> events = new List<CraftsEvent>();

            try
            {
                int publicationID = WebConfiguration.Current.PublicationId;
                DateTime today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                var startDate = month.AddMonths(-11);
                var endDate = month.AddMonths(+11);
                Logger.DebugFormat("Events repository - GetCraftsEventsInArea - Before gateway call");
                RetailersGateway sg = new RetailersGateway();
                events = sg.GetCraftsEventsInArea(publicationID, lat, lng, radius, startDate, endDate, maxResults);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", ex));
            }

            return events;
        }

    }
}