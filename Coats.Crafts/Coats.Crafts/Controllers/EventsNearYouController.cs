using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Core.Logging;
using Coats.Crafts.Models;
using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;
using Coats.Crafts.Repositories.Interfaces;
using Coats.IndustrialPortal.Providers;
using Coats.Crafts.ControllerHelpers;
using DD4T.ContentModel;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using System.Diagnostics;

namespace Coats.Crafts.Controllers
{
    public class EventsNearYouController : Controller
    {
        private readonly IEventsRepository eventsrepository;
        private readonly IAppSettings settings;

        public ILogger Logger { get; set; }
        public bool GeoDataAvailable
        {
            get
            {
                bool geoDataAvailable = false;
                if (HttpContext.Items.Contains("GeoInfoAvailable"))
                {
                    if (HttpContext.Items["GeoInfoAvailable"] is bool)
                    {
                        geoDataAvailable = (bool)HttpContext.Items["GeoInfoAvailable"];
                    }
                }
                return geoDataAvailable;
            }
        }
        public decimal Longitude
        {
            get
            {
                decimal longitude = 0.0M;
                if (HttpContext.Items.Contains("CurrentRequestLongitude"))
                {
                    if (HttpContext.Items["CurrentRequestLongitude"] is decimal)
                    {
                        longitude = (decimal) HttpContext.Items["CurrentRequestLongitude"];
                    }
                }
                return longitude;                 
            }
        }
        public decimal Latitude
        {
            get
            {
                decimal latitude = 0.0M;
                if (HttpContext.Items.Contains("CurrentRequestLatitude"))
                {
                    if (HttpContext.Items["CurrentRequestLatitude"] is decimal)
                    {
                        latitude = (decimal)HttpContext.Items["CurrentRequestLatitude"];
                    }
                }
                return latitude;
            }
        }

        public EventsNearYouController(IEventsRepository eventsrepository, IAppSettings settings)
        {
            this.eventsrepository = eventsrepository;
            this.settings = settings;
        }

        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            Stopwatch sw = new Stopwatch();

            if (Logger.IsDebugEnabled)
            {
                sw.Start();
                Logger.Debug("EventsNearYou enter >>>>>");
            }


            // View model
            EventsNearYou eventsnearyou = new EventsNearYou();
            eventsnearyou.ComponentTitle = componentPresentation.Component.Fields["title"].Value;
            eventsnearyou.Lat = Latitude.ToString();
            eventsnearyou.Lng = Longitude.ToString();

            try
            {
                IField links = componentPresentation.Component.Fields["link"];
                IFieldSet link = links.EmbeddedValues[0];
                if (link.ContainsKey("linkTitle"))
                {
                    eventsnearyou.LinkTitle = link["linkTitle"].Value;
                }
                if (link.ContainsKey("linkURL"))
                {
                    eventsnearyou.LinkURL = link["linkURL"].Value.ToUpper().StartsWith("HTTP") ? link["linkURL"].Value : link["linkURL"].Value.AddApplicationRoot();
                }
                if (link.ContainsKey("linkComponent"))
                {
                    var comp = link["linkComponent"].LinkedComponentValues[0];
                    eventsnearyou.LinkComponent = comp;
                }

            }
            catch
            {
                eventsnearyou.LinkTitle = string.Empty;
                eventsnearyou.LinkURL = string.Empty;
            }

            // Initialise events list
            List<CraftsEvent> events = new List<CraftsEvent>();

            // Is there a loggedin user ...
            if (User.Identity.IsAuthenticated)
            {
                // Gte their profile and try and get thrit long/lat
                //CoatsUserProfile user = ProfileHelper.GetUser();

                var user = (Coats.Crafts.MvcApplication.CraftsPrincipal)HttpContext.User;

                decimal lat;
                if (!Decimal.TryParse(user.LAT, out lat))
                    //defaultLat = WebConfiguration.Current.DefaultLat + "M";
                    //lat = Convert.ToDecimal(defaultLat);
                    lat = 0.0M;

                decimal lng;
                if (!Decimal.TryParse(user.LONG, out lng))
                    //defaultLong = WebConfiguration.Current.DefaultLong + "M";
                    //lng = Convert.ToDecimal(defaultLong);
                    lng = 0.0M;

                // If there are values, use them
                if (lat != 0 && lng != 0)
                {
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Getting events for user {0} who has long {1} and lat {2}", user.UserName, lng, lat);

                    events = eventsrepository.GetCraftsEventsInArea(
                        lat, lng, settings.EventsNearYouRadius, settings.EventsNearYouMaxResults);
                }
            }

            // Any events based on the user check above?
            if (events.Count == 0)
            {
                // Is there any geo data available about the request?
                if (GeoDataAvailable)
                {
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("No user available, but request has geo data available, getting events for  long {0} and lat {1}", Longitude, Latitude);

                    events = eventsrepository.GetCraftsEventsInArea(
                            Latitude, Longitude, settings.EventsNearYouRadius, settings.EventsNearYouMaxResults);
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.Debug("No user available and no request geo data either, just getting some events!");

                    events = eventsrepository.GetCraftsEventsInArea();                    
                }
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Events: {0}", events.Count);

            eventsnearyou.Events = events;

            if (Logger.IsDebugEnabled)
                Logger.Debug("<<<<< EventsNearYou finish");

            if (Logger.IsDebugEnabled)
            {
                sw.Stop();
                string timer = sw.Elapsed.ToString();
                Logger.DebugFormat("EventsNearYouController.Index time elapsed:" + timer);
            }
            return View(eventsnearyou);
        }
    }
}
