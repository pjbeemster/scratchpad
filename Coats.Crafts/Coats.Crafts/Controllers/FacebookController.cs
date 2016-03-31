using System.Linq;
using System.Web.Mvc;

using Coats.Crafts.Models;

namespace Coats.Crafts.Controllers
{
    public class FacebookController : Controller
    {
        public ActionResult Index(string facebookID, string orientation)
        {
            // This now needs to call the FacebookController to get the facebook details
            PostModel pm = new PostModel();
            return View(pm.GetAll(facebookID, orientation).ToList());
        }
    }
}