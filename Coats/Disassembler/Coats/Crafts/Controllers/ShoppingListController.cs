namespace Coats.Crafts.Controllers
{
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Models;
    using Coats.Crafts.Repositories.Interfaces;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using DD4T.Mvc.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public class ShoppingListController : TridionControllerBase
    {
        private IShoppingListRepository shoppinglistrepository;

        public ShoppingListController(IShoppingListRepository shoppinglistrepository)
        {
            this.shoppinglistrepository = shoppinglistrepository;
        }

        [HttpPost, Authorize]
        public ActionResult AddItem(string id = "", int quantity = 1, string project = "", string redirectUrl = "")
        {
            string resource = Helper.GetResource("Feedback_AddedToShoppingList");
            ShoppingListItem item = new ShoppingListItem();
            Guid result = new Guid();
            Guid.TryParse(id, out result);
            item.ProductTcmID = id;
            item.UserID = base.User.Identity.Name;
            item.Quantity = quantity;
            item.ProjectName = project;
            ShoppingListItem item2 = this.shoppinglistrepository.UpdateAndInsertItem(item);
            if (item == null)
            {
                resource = Helper.GetResource("Feedback_NotAddedToShoppingList");
            }
            if (base.Request.IsAjaxRequest())
            {
                var data = new {
                    success = item != null,
                    item = (item != null) ? item : null,
                    feedback = resource,
                    redirect = redirectUrl
                };
                return base.Json(data);
            }
            base.Session.Add("feedback", resource);
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return this.Redirect(redirectUrl);
            }
            return this.Redirect("/shoppinglist");
        }

        [HttpPost]
        public ActionResult ChangeQuantity(string ID, string quantityButton, string returnUrl = "")
        {
            int alterQuantityBy = 1;
            if (quantityButton == "decrease")
            {
                alterQuantityBy = -1;
            }
            Guid result = new Guid();
            Guid.TryParse(ID, out result);
            ShoppingListItem item = this.shoppinglistrepository.UpdateShoppingListItemQuantity(result, base.User.Identity.Name, alterQuantityBy);
            if (base.Request.IsAjaxRequest())
            {
                var data = new {
                    success = item != null,
                    data = (item != null) ? item : null
                };
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Redirect(returnUrl);
        }

        [HttpDelete]
        public ActionResult DeleteItem(string ID, string returnUrl = "")
        {
            bool flag = false;
            if (base.User.Identity.IsAuthenticated)
            {
                Guid guid;
                if (Guid.TryParse(ID, out guid))
                {
                    flag = this.shoppinglistrepository.TryDeleteShoppingListItem(guid, base.User.Identity.Name);
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

        [HttpDelete]
        public ActionResult DeleteProject(string projectName, string returnUrl = "")
        {
            bool flag = false;
            if (base.User.Identity.IsAuthenticated)
            {
                flag = this.shoppinglistrepository.TryDeleteShoppingListItemsByProjectName(projectName, base.User.Identity.Name);
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

        [Authorize, HttpPost]
        public ActionResult EmailShoppingList(ComponentPresentation componentPresentation, string EmailButton)
        {
            ShoppingListEmail email = new ShoppingListEmail {
                items = this.shoppinglistrepository.GetShoppingListItemsByProduct(base.User.Identity.Name),
                itemHtml = "\n\n"
            };
            string projectName = string.Empty;
            foreach (IGrouping<string, ShoppingListItem> grouping in email.items)
            {
                int num = 0;
                foreach (ShoppingListItem item in grouping)
                {
                    num += item.Quantity;
                }
                if (num != 0)
                {
                    string itemHtml;
                    if (projectName != grouping.First<ShoppingListItem>().ProjectName)
                    {
                        itemHtml = email.itemHtml;
                        email.itemHtml = itemHtml + Helper.GetResource("ShoppingListProject") + ": " + grouping.First<ShoppingListItem>().ProjectName + "\n\n";
                    }
                    itemHtml = email.itemHtml;
                    email.itemHtml = itemHtml + Helper.GetResource("ShoppingListProduct") + ": " + grouping.First<ShoppingListItem>().ProductName + "\n";
                    itemHtml = email.itemHtml;
                    email.itemHtml = itemHtml + Helper.GetResource("ShoppingListBrand") + ": " + grouping.First<ShoppingListItem>().Brand + "\n";
                    object obj2 = email.itemHtml;
                    email.itemHtml = string.Concat(new object[] { obj2, Helper.GetResource("ShoppingListQuantity"), ": ", num, "\n" });
                    email.itemHtml = email.itemHtml + "\n";
                }
                projectName = grouping.First<ShoppingListItem>().ProjectName;
            }
            email.itemHtml = email.itemHtml + "\n";
            email.EmailAddress = base.User.Identity.Name;
            bool flag = this.shoppinglistrepository.SendShoppingListEmail(email);
            ShoppingList model = new ShoppingList {
                componentPresentation = componentPresentation,
                items = this.shoppinglistrepository.GetShoppingListItemsByProject(base.User.Identity.Name)
            };
            if (base.Request.IsAjaxRequest())
            {
                var data = new {
                    success = flag
                };
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            return base.View(model);
        }

        [Authorize]
        public ActionResult Index(ComponentPresentation componentPresentation)
        {
            ShoppingList model = new ShoppingList {
                componentPresentation = componentPresentation,
                items = this.shoppinglistrepository.GetShoppingListItemsByProject(base.User.Identity.Name)
            };
            return base.View(model);
        }

        [HttpPost]
        public ActionResult ShoppingListInsertItems(List<ShoppingListItem> items)
        {
            if (base.User.Identity.IsAuthenticated)
            {
                foreach (ShoppingListItem item in items)
                {
                    item.UserID = base.User.Identity.Name;
                    this.shoppinglistrepository.UpdateAndInsertItem(item);
                }
            }
            return this.Redirect("/ShoppingList/shoppinglist.html");
        }

        [HttpGet, Authorize]
        public ActionResult ShoppingListPrint()
        {
            ShoppingList model = new ShoppingList();
            if (base.User.Identity.IsAuthenticated)
            {
                model.items = this.shoppinglistrepository.GetShoppingListItemsByProject(base.User.Identity.Name);
                return base.View(model);
            }
            return base.View(model);
        }
    }
}

