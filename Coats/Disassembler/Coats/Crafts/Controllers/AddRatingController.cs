namespace Coats.Crafts.Controllers
{
    using System.Web.Mvc;

    public class AddRatingController : Controller
    {
        public ActionResult Index()
        {
            return this.Redirect(base.Request.Url.AbsolutePath);
        }
    }
}

