namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using DD4T.ContentModel;
    using Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class EventsNearYouController : Controller
    {
        private readonly IEventsRepository eventsrepository;
        private readonly IAppSettings settings;

        public EventsNearYouController(IEventsRepository eventsrepository, IAppSettings settings)
        {
            this.eventsrepository = eventsrepository;
            this.settings = settings;
        }

        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            Stopwatch stopwatch = new Stopwatch();
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Start();
                this.Logger.Debug("EventsNearYou enter >>>>>");
            }
            EventsNearYou model = new EventsNearYou {
                ComponentTitle = componentPresentation.Component.Fields["title"].Value,
                Lat = this.Latitude.ToString(),
                Lng = this.Longitude.ToString()
            };
            try
            {
                IField field = componentPresentation.Component.Fields["link"];
                IFieldSet set = field.EmbeddedValues[0];
                if (set.ContainsKey("linkTitle"))
                {
                    model.LinkTitle = set["linkTitle"].Value;
                }
                if (set.ContainsKey("linkURL"))
                {
                    model.LinkURL = set["linkURL"].Value.ToUpper().StartsWith("HTTP") ? set["linkURL"].Value : set["linkURL"].Value.AddApplicationRoot();
                }
                if (set.ContainsKey("linkComponent"))
                {
                    IComponent component = set["linkComponent"].LinkedComponentValues[0];
                    model.LinkComponent = component;
                }
            }
            catch
            {
                model.LinkTitle = string.Empty;
                model.LinkURL = string.Empty;
            }
            List<CraftsEvent> craftsEventsInArea = new List<CraftsEvent>();
            if (base.User.Identity.IsAuthenticated)
            {
                decimal num;
                decimal num2;
                MvcApplication.CraftsPrincipal user = (MvcApplication.CraftsPrincipal) base.HttpContext.User;
                if (!decimal.TryParse(user.LAT, out num))
                {
                    num = 0.0M;
                }
                if (!decimal.TryParse(user.LONG, out num2))
                {
                    num2 = 0.0M;
                }
                if ((num != 0M) && (num2 != 0M))
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.DebugFormat("Getting events for user {0} who has long {1} and lat {2}", new object[] { user.UserName, num2, num });
                    }
                    craftsEventsInArea = this.eventsrepository.GetCraftsEventsInArea(num, num2, this.settings.EventsNearYouRadius, this.settings.EventsNearYouMaxResults);
                }
            }
            if (craftsEventsInArea.Count == 0)
            {
                if (this.GeoDataAvailable)
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.DebugFormat("No user available, but request has geo data available, getting events for  long {0} and lat {1}", new object[] { this.Longitude, this.Latitude });
                    }
                    craftsEventsInArea = this.eventsrepository.GetCraftsEventsInArea(this.Latitude, this.Longitude, this.settings.EventsNearYouRadius, this.settings.EventsNearYouMaxResults);
                }
                else
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.Debug("No user available and no request geo data either, just getting some events!");
                    }
                    craftsEventsInArea = this.eventsrepository.GetCraftsEventsInArea();
                }
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.DebugFormat("Events: {0}", new object[] { craftsEventsInArea.Count });
            }
            model.Events = craftsEventsInArea;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.Debug("<<<<< EventsNearYou finish");
            }
            if (this.Logger.IsDebugEnabled)
            {
                stopwatch.Stop();
                string str = stopwatch.Elapsed.ToString();
                this.Logger.DebugFormat("EventsNearYouController.Index time elapsed:" + str, new object[0]);
            }
            return base.View(model);
        }

        public bool GeoDataAvailable
        {
            get
            {
                bool flag = false;
                if (base.HttpContext.Items.Contains("GeoInfoAvailable") && (base.HttpContext.Items["GeoInfoAvailable"] is bool))
                {
                    flag = (bool) base.HttpContext.Items["GeoInfoAvailable"];
                }
                return flag;
            }
        }

        public decimal Latitude
        {
            get
            {
                decimal num = 0.0M;
                if (base.HttpContext.Items.Contains("CurrentRequestLatitude") && (base.HttpContext.Items["CurrentRequestLatitude"] is decimal))
                {
                    num = (decimal) base.HttpContext.Items["CurrentRequestLatitude"];
                }
                return num;
            }
        }

        public ILogger Logger { get; set; }

        public decimal Longitude
        {
            get
            {
                decimal num = 0.0M;
                if (base.HttpContext.Items.Contains("CurrentRequestLongitude") && (base.HttpContext.Items["CurrentRequestLongitude"] is decimal))
                {
                    num = (decimal) base.HttpContext.Items["CurrentRequestLongitude"];
                }
                return num;
            }
        }
    }
}

