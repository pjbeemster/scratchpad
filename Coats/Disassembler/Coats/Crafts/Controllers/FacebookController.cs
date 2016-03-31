namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Models;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    public class FacebookController : Controller
    {
        public ActionResult Index(string facebookID, string orientation)
        {
            PostModel model = new PostModel();
            return base.View(model.GetAll(facebookID, orientation).ToList<PostModel>());
        }
    }
}

