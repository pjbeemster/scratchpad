using System;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Gateway;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;
using Castle.Windsor;
using DD4T.ContentModel.Factories;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Filters
{
    public class CheckActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.RequestContext.HttpContext;
            var qParams = httpContext.Request.Params;

            // Check if user is authenticated
            if (httpContext.User.Identity.IsAuthenticated)
            {

                // Check if callback action is defined
                if (!String.IsNullOrEmpty(qParams["action"]))
                {
                    // Handle add to scrapbook callbacks
                    if (qParams["action"] == "addtoscrapbook")
                    {

                        // Build Scrabook Item object
                        ScrapbookItem item = new ScrapbookItem
                        {
                            UserID              = httpContext.User.Identity.Name,
                            ImageURL            = qParams["imageURL"],
                            ItemDescription     = qParams["description"],
                            ItemType            = qParams["type"],
                            SourceURL           = qParams["sourceUrl"],
                            SourceDescription   = qParams["sourceDescription"]
                        };

                        // Fire off insert item method
                        ScrapbookGateway sg = new ScrapbookGateway();
                        var insertedItem = sg.InsertScrapbookItem(item);

                        if (insertedItem != null)
                        {
                            if (!String.IsNullOrEmpty(qParams["ReturnUrl"]))
                            {
                                // Set feedback message via session - retreive in view
                                filterContext.HttpContext.Session.Add("feedback", Helper.GetResource("Feedback_AddedToScrapbook"));
                                filterContext.Result = new RedirectResult(qParams["ReturnUrl"]);
                            }
                        }

                        base.OnActionExecuting(filterContext);
                    }

                    // Handle add to shopping list callbacks
                    if (qParams["action"] == "addtoshoppinglist")
                    {

                        String project = (qParams["project"] != null) ? qParams["project"] : String.Empty;
                        Int32 quantity = 0;

                        Boolean quant = Int32.TryParse(qParams["quantity"], out quantity);

                        IComponent product = GetComponentInfo(qParams["tcm-id"]);

                        // Build Shopping List Item object
                        ShoppingListItem item = new ShoppingListItem
                        {
                            AddedDateTime       = DateTime.Now,
                            Brand               = product.Fields["brand"].Value,
                            ProductTcmID        = qParams["tcm-id"],
                            ProductName         = product.Fields["title"].Value,
                            ProjectName         = project,
                            Quantity            = quantity,
                            UserID              = httpContext.User.Identity.Name
                        };

                        // Fire off insert item method
                        ShoppingListGateway gw = new ShoppingListGateway();
                        var insertedItem = gw.InsertShoppingListItem(item);

                        if (insertedItem != null)
                        {
                            if (!String.IsNullOrEmpty(qParams["ReturnUrl"]))
                            {
                                // Set feedback message via session - retreive in view
                                filterContext.HttpContext.Session.Add("feedback", Helper.GetResource("Feedback_AddedToShoppingList"));
                                filterContext.Result = new RedirectResult(qParams["ReturnUrl"]);
                            }
                        }

                        // Recall method with new filterContext
                        base.OnActionExecuting(filterContext);
                    }

                    // Handle redirects
                    if (qParams["action"] == "ReturnUrl")
                    {
                        if (!String.IsNullOrEmpty(qParams["ReturnUrl"]))
                        {
                            filterContext.Result = new RedirectResult(qParams["ReturnUrl"]);
                        }

                        // Recall method with new filterContext
                        base.OnActionExecuting(filterContext);
                    }

                    // Handle download items callbacks
                    if (qParams["action"] == "download")
                    {
                        if (!String.IsNullOrEmpty(qParams["itemLink"]))
                        {
                            filterContext.Result = new RedirectResult(qParams["ReturnUrl"] + "?download=" + qParams["itemLink"]);
                        }

                        // Recall method with new filterContext
                        base.OnActionExecuting(filterContext);
                    }
                }
            }
        }

        private IComponent GetComponentInfo(string tcm)
        {
            var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;

            var factory = accessor.Container.Resolve<IComponentFactory>();

            IComponent c = factory.GetComponent(tcm);
            return c;
        }
    }
}