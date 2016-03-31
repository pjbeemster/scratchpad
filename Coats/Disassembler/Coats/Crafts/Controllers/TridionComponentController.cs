namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using DD4T.ContentModel;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class TridionComponentController : TridionControllerBase
    {
        public ActionResult Component(ComponentPresentation componentPresentation)
        {
            return this.ComponentPresentation(componentPresentation.Component.Id);
        }

        public ILogger Logger { get; set; }
    }
}

