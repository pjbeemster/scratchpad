using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DD4T.ContentModel;
using DD4T.Mvc.Controllers;

using Castle.Core.Logging;
using DD4T.ContentModel.Exceptions;
using System.Text;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Models;


namespace Coats.Crafts.Controllers
{
    public class TridionComponentController : TridionControllerBase
    {
        public ILogger Logger { get; set; }

        //
        // GET: /TridionComponent/
        public ActionResult Component(ComponentPresentation componentPresentation)
        {
            return ComponentPresentation(componentPresentation.Component.Id);
        }
    }
}
