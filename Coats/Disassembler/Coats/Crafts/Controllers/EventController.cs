namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class EventController : Controller
    {
        private IEventsRepository eventsrepository;

        public EventController(IEventsRepository eventsrepository)
        {
            this.eventsrepository = eventsrepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            Events model = new Events {
                Description = "Event"
            };
            return base.View(model);
        }

        public ILogger Logger { get; set; }

        public IAppSettings Settings { get; set; }
    }
}

