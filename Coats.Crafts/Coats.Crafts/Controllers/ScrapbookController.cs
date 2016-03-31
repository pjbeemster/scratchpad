using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DD4T.Mvc.Controllers;
using DD4T.Mvc.Attributes;
using System.Linq;

using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using Coats.Crafts.Models;
using System.Configuration;
using Coats.Crafts.HtmlHelpers;
using DD4T.ContentModel;
using Castle.Windsor;
using System.Web.Security;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Controllers
{
    public class ScrapbookController : TridionControllerBase
    {
        private IScrapbookRepository scrapbookrepository;

        public ScrapbookController(IScrapbookRepository scrapbookrepository)
        {
            this.scrapbookrepository = scrapbookrepository;
        }

        public ActionResult Index()
        {
            Scrapbook results = new Scrapbook();
            if (User.Identity.IsAuthenticated)
            {
                results.items = scrapbookrepository.GetScrapbookItemsForUser(User.Identity.Name).OrderBy(x =>x.ItemDescription).ToList();
                return View(results);
            }

            //icky but not sure how else to do this from child
            string redirectURL = ConfigurationManager.AppSettings["Registration"] + "?returnUrl=" + Request.Url.PathAndQuery;
            Response.Redirect(redirectURL);
            return null;
        }

        [HttpDelete]
        public ActionResult Delete(string ID, string returnUrl = "")
        {
            bool success = false;

            if (User.Identity.IsAuthenticated)
            {
                Guid itemID;
                if (Guid.TryParse(ID, out itemID))
                {
                    success = scrapbookrepository.TryDeleteScrapbookItem(itemID, User.Identity.Name);
                }
                if (Request.IsAjaxRequest())
                {
                    var json = new
                    {
                        success = success
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        [HttpPost]
        public ActionResult AddItem(Scrapbook model)
        {
            if (ModelState.IsValid)
            {
                // Positive feedback by default
                var feedback = Helper.GetResource("Feedback_AddedToScrapbook");

                if (!User.Identity.IsAuthenticated)
                {
                    return Redirect(model.returnUrl);
                }

                var imageUrl = model.imageUrl != null ? model.imageUrl : String.Empty;
                var description = model.description != null ? model.description : String.Empty;
                var sourceDescription = model.sourceDescription != null ? model.sourceDescription : model.sourceUrl;

                var insertedItem = scrapbookrepository.InsertScrapbookItem(User.Identity.Name, imageUrl, description, model.type, model.sourceUrl, sourceDescription);

                if (insertedItem == null)
                {
                    feedback = Helper.GetResource("Feedback_NotAddedToScrapbook");
                }

                if (Request.IsAjaxRequest())
                {
                    // build and return data via json
                    var json = new
                    {
                        success = (insertedItem != null),
                        item = (insertedItem != null) ? insertedItem : null,
                        feedback = feedback,
                        redirect = model.returnUrl
                    };
                    return Json(json);
                }
                else
                {
                    // Add data to session
                    Session.Add("feedback", feedback);
                    return Redirect(model.returnUrl);
                }
            }
            return Redirect(model.returnUrl);
        }
    }
}
