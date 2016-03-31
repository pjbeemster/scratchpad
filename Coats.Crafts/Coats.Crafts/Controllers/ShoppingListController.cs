using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DD4T.Mvc.Controllers;
using DD4T.Mvc.Attributes;

using Coats.Crafts.Repositories.Interfaces;
using Coats.Crafts.Data;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using Coats.Crafts.Models;
using System.Configuration;
using Coats.Crafts.HtmlHelpers;
using DD4T.ContentModel;
using Castle.Windsor;
using System.Web.Security;
using System.Collections;
using System.Linq;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Controllers
{
    public class ShoppingListController : TridionControllerBase
    {
        private IShoppingListRepository shoppinglistrepository;

        public ShoppingListController(IShoppingListRepository shoppinglistrepository)
        {
            this.shoppinglistrepository = shoppinglistrepository;
        }

        [Authorize]
        public ActionResult Index(ComponentPresentation componentPresentation)
        {
            ShoppingList results = new ShoppingList();
            results.componentPresentation = componentPresentation;
            results.items = shoppinglistrepository.GetShoppingListItemsByProject(User.Identity.Name);

            return View(results);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EmailShoppingList(ComponentPresentation componentPresentation, string EmailButton)
        {
            ShoppingListEmail email = new ShoppingListEmail();
            email.items = shoppinglistrepository.GetShoppingListItemsByProduct(User.Identity.Name);

            email.itemHtml = "\n\n";

            string projectName = string.Empty;

            foreach (var s in email.items)
            {
                int totalQuantity = 0;
                foreach (var productEntry in s)
                {
                    totalQuantity += productEntry.Quantity;
                }

                if (totalQuantity != 0)
                {
                    if (projectName != s.First().ProjectName)
                    {
                        email.itemHtml += Helper.GetResource("ShoppingListProject") + ": " + s.First().ProjectName + "\n\n";
                    }

                    email.itemHtml += Helper.GetResource("ShoppingListProduct") + ": " + s.First().ProductName + "\n";
                    email.itemHtml += Helper.GetResource("ShoppingListBrand") + ": " + s.First().Brand + "\n";
                    email.itemHtml += Helper.GetResource("ShoppingListQuantity") + ": " + totalQuantity + "\n";
                    email.itemHtml += "\n";
                }

                projectName = s.First().ProjectName;
            }

            email.itemHtml += "\n";
            email.EmailAddress = User.Identity.Name;
            //END ITEM HTML BUILD UP

            bool success = shoppinglistrepository.SendShoppingListEmail(email);

            ShoppingList results = new ShoppingList();
            results.componentPresentation = componentPresentation;
            results.items = shoppinglistrepository.GetShoppingListItemsByProject(User.Identity.Name);

            if (Request.IsAjaxRequest())
            {
                var json = new
                {
                    success = success
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //no ajax
                return View(results);
            }
            
        }

        [HttpDelete]
        public ActionResult DeleteItem(string ID, string returnUrl = "")
        {
            bool success = false;

            if (User.Identity.IsAuthenticated)
            {
                Guid itemID;
                if (Guid.TryParse(ID, out itemID))
                {
                    success = shoppinglistrepository.TryDeleteShoppingListItem(itemID, User.Identity.Name);
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

        [HttpDelete]
        public ActionResult DeleteProject(string projectName, string returnUrl = "")
        {
            bool success = false;

            if (User.Identity.IsAuthenticated)
            {
                success = shoppinglistrepository.TryDeleteShoppingListItemsByProjectName(projectName, User.Identity.Name);

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
        public ActionResult ChangeQuantity(string ID, string quantityButton, string returnUrl = "")
        {
            int changeByVal = 1;

            if (quantityButton == "decrease")
            {
                changeByVal = -1;
            }


            Guid itemID = new Guid();
            Guid.TryParse(ID, out itemID);

            ShoppingListItem item = shoppinglistrepository.UpdateShoppingListItemQuantity(itemID, User.Identity.Name, changeByVal);

            if (Request.IsAjaxRequest())
            {
                var json = new
                {
                    success = (item != null),
                    data = (item != null) ? item : null
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //no ajax
                return Redirect(returnUrl);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddItem(string id = "", int quantity = 1, string project = "", string redirectUrl = "")
        {
            // Positive feedback by default
            var feedback = Helper.GetResource("Feedback_AddedToShoppingList");

            var insertedItem = new ShoppingListItem();

            Guid itemID = new Guid();
            Guid.TryParse(id, out itemID);

            insertedItem.ProductTcmID = id;
            insertedItem.UserID = User.Identity.Name;
            insertedItem.Quantity = quantity;
            insertedItem.ProjectName = project;

            var request = shoppinglistrepository.UpdateAndInsertItem(insertedItem);

            if (insertedItem == null)
            {
                feedback = Helper.GetResource("Feedback_NotAddedToShoppingList");
            }

            if (Request.IsAjaxRequest())
            {
                // build and return data via json
                var json = new
                {
                    success = (insertedItem != null),
                    item = (insertedItem != null) ? insertedItem : null,
                    feedback = feedback,
                    redirect = redirectUrl

                };
                return Json(json);
            }
            else
            {
                // Add data to session
                Session.Add("feedback", feedback);

                if (!String.IsNullOrEmpty(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
                else
                {
                    return Redirect("/shoppinglist");
                }
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult ShoppingListPrint()
        {
            ShoppingList results = new ShoppingList();
            if (User.Identity.IsAuthenticated)
            {
                results.items = shoppinglistrepository.GetShoppingListItemsByProject(User.Identity.Name);
                return View(results);
            }

            return View(results);
        }

        [HttpPost]
        public ActionResult ShoppingListInsertItems(List<ShoppingListItem> items)
        {
            if (User.Identity.IsAuthenticated)
            {
                foreach (var item in items)
                {
                    item.UserID = User.Identity.Name;
                    shoppinglistrepository.UpdateAndInsertItem(item);
                }
            }
            return Redirect("/ShoppingList/shoppinglist.html");
        }

    }
}
