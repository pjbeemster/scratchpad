namespace Coats.Crafts.Filters
{
    using System;
    using System.Web.Mvc;

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

