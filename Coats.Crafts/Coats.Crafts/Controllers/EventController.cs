using System.Web.Mvc;
using Castle.Core.Logging;
using Coats.Crafts.Models;
using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;
using Coats.Crafts.Repositories.Interfaces;

namespace Coats.Crafts.Controllers
{
    public class EventController : Controller
    {
        public ILogger Logger { get; set; }

        private IEventsRepository eventsrepository;
        public IAppSettings Settings { get; set; }

        public EventController(IEventsRepository eventsrepository)
        {
            this.eventsrepository = eventsrepository;
        }


        [HttpGet]
        public ActionResult Index()
        {
            Events model = new Events();
            model.Description = "Event";
            return View(model);
        }
    }

}
