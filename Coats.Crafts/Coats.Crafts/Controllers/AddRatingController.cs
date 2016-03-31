using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coats.Crafts.Controllers
{
    public class AddRatingController : Controller
    {
        //
        // GET: /AddRating/

        public ActionResult Index()
        {
            return Redirect(Request.Url.AbsolutePath);
            //return View();
        }

    }
}
