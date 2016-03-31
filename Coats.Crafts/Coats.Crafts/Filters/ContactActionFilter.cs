using System;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Gateway;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;
using Castle.Windsor;
using DD4T.ContentModel.Factories;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Filters
{
    public class ContactActionFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData["ViewData"] = filterContext.Controller.ViewData;
            filterContext.Controller.TempData["contactform"] = filterContext.Controller.TempData["contactform"];

            base.OnActionExecuting(filterContext);
        }

    }
}