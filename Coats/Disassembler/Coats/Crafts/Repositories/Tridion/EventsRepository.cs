namespace Coats.Crafts.Repositories.Tridion
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Gateway;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    public class EventsRepository : IEventsRepository
    {
        private IAppSettings _settings;

        public EventsRepository(IAppSettings settings)
        {
            this._settings = settings;
        }

        public List<CraftsEvent> GetCraftsEventsInArea()
        {
            List<CraftsEvent> list = new List<CraftsEvent>();
            try
            {
                string defaultLong = WebConfiguration.Current.DefaultLong;
                string defaultLat = WebConfiguration.Current.DefaultLat;
                int publicationId = WebConfiguration.Current.PublicationId;
                decimal lat = Convert.ToDecimal(defaultLat, CultureInfo.InvariantCulture);
                decimal lng = Convert.ToDecimal(defaultLong, CultureInfo.InvariantCulture);
                int radius = 0x3e8;
                DateTime today = DateTime.Today;
                DateTime time2 = new DateTime(today.Year, today.Month, 1);
                DateTime startDate = time2.AddMonths(-11);
                DateTime endDate = time2.AddMonths(11);
                int maxResults = 2;
                list = new RetailersGateway().GetCraftsEventsInArea(publicationId, lat, lng, radius, startDate, endDate, maxResults);
            }
            catch (Exception exception)
            {
                this.Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", exception));
            }
            return list;
        }

        public List<CraftsEvent> GetCraftsEventsInArea(decimal lat, decimal lng, int radius, int maxResults)
        {
            this.Logger.Debug("Events repository - GetCraftsEventsInArea");
            this.Logger.DebugFormat("Events repository - GetCraftsEventsInArea -  lat {0}, lng {1}, radius{2}, maxResults {3} ", new object[] { lat.ToString(), lng.ToString(), radius.ToString(), maxResults.ToString() });
            List<CraftsEvent> list = new List<CraftsEvent>();
            try
            {
                int publicationId = WebConfiguration.Current.PublicationId;
                DateTime today = DateTime.Today;
                DateTime time2 = new DateTime(today.Year, today.Month, 1);
                DateTime startDate = time2.AddMonths(-11);
                DateTime endDate = time2.AddMonths(11);
                this.Logger.DebugFormat("Events repository - GetCraftsEventsInArea - Before gateway call", new object[0]);
                list = new RetailersGateway().GetCraftsEventsInArea(publicationId, lat, lng, radius, startDate, endDate, maxResults);
            }
            catch (Exception exception)
            {
                this.Logger.Error(string.Format("{0} {1}", "GetCraftsEventsInArea", exception));
            }
            return list;
        }

        public ILogger Logger { get; set; }
    }
}

