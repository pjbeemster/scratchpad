namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public class ScrapbookController : TridionControllerBase
    {
        private IScrapbookRepository scrapbookrepository;

        public ScrapbookController(IScrapbookRepository scrapbookrepository)
        {
            this.scrapbookrepository = scrapbookrepository;
        }

        [HttpPost]
        public ActionResult AddItem(Scrapbook model)
        {
            if (base.ModelState.IsValid)
            {
                string resource = Helper.GetResource("Feedback_AddedToScrapbook");
                if (base.User.Identity.IsAuthenticated)
                {
                    string imageURL = (model.imageUrl != null) ? model.imageUrl : string.Empty;
                    string itemDescription = (model.description != null) ? model.description : string.Empty;
                    string sourceDescription = (model.sourceDescription != null) ? model.sourceDescription : model.sourceUrl;
                    ScrapbookItem item = this.scrapbookrepository.InsertScrapbookItem(base.User.Identity.Name, imageURL, itemDescription, model.type, model.sourceUrl, sourceDescription);
                    if (item == null)
                    {
                        resource = Helper.GetResource("Feedback_NotAddedToScrapbook");
                    }
                    if (base.Request.IsAjaxRequest())
                    {
                        var data = new {
                            success = item != null,
                            item = (item != null) ? item : null,
                            feedback = resource,
                            redirect = model.returnUrl
                        };
                        return base.Json(data);
                    }
                    base.Session.Add("feedback", resource);
                }
                return this.Redirect(model.returnUrl);
            }
            return this.Redirect(model.returnUrl);
        }

        [HttpDelete]
        public ActionResult Delete(string ID, string returnUrl = "")
        {
            bool flag = false;
            if (base.User.Identity.IsAuthenticated)
            {
                Guid guid;
                if (Guid.TryParse(ID, out guid))
                {
                    flag = this.scrapbookrepository.TryDeleteScrapbookItem(guid, base.User.Identity.Name);
                }
                if (base.Request.IsAjaxRequest())
                {
                    var data = new {
                        success = flag
                    };
                    return base.Json(data, JsonRequestBehavior.AllowGet);
                }
                return this.Redirect(returnUrl);
            }
            return this.Redirect(returnUrl);
        }

        public ActionResult Index()
        {
            Scrapbook model = new Scrapbook();
            if (base.User.Identity.IsAuthenticated)
            {
                model.items = (from x in this.scrapbookrepository.GetScrapbookItemsForUser(base.User.Identity.Name)
                    orderby x.ItemDescription
                    select x).ToList<ScrapbookItem>();
                return base.View(model);
            }
            string url = ConfigurationManager.AppSettings["Registration"] + "?returnUrl=" + base.Request.Url.PathAndQuery;
            base.Response.Redirect(url);
            return null;
        }
    }
}

