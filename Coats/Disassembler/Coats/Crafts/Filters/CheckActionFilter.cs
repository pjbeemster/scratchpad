namespace Coats.Crafts.Filters
{
    using Castle.Windsor;
    using Coats.Crafts.Gateway;
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Factories;
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Mvc;

    public class CheckActionFilter : ActionFilterAttribute
    {
        private IComponent GetComponentInfo(string tcm)
        {
            IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
            return applicationInstance.Container.Resolve<IComponentFactory>().GetComponent(tcm);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase httpContext = filterContext.RequestContext.HttpContext;
            NameValueCollection @params = httpContext.Request.Params;
            if (httpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(@params["action"]))
            {
                if (@params["action"] == "addtoscrapbook")
                {
                    ScrapbookItem item = new ScrapbookItem {
                        UserID = httpContext.User.Identity.Name,
                        ImageURL = @params["imageURL"],
                        ItemDescription = @params["description"],
                        ItemType = @params["type"],
                        SourceURL = @params["sourceUrl"],
                        SourceDescription = @params["sourceDescription"]
                    };
                    ScrapbookGateway gateway = new ScrapbookGateway();
                    if ((gateway.InsertScrapbookItem(item) != null) && !string.IsNullOrEmpty(@params["ReturnUrl"]))
                    {
                        filterContext.HttpContext.Session.Add("feedback", Helper.GetResource("Feedback_AddedToScrapbook"));
                        filterContext.Result = new RedirectResult(@params["ReturnUrl"]);
                    }
                    base.OnActionExecuting(filterContext);
                }
                if (@params["action"] == "addtoshoppinglist")
                {
                    string str = (@params["project"] != null) ? @params["project"] : string.Empty;
                    int result = 0;
                    bool flag = int.TryParse(@params["quantity"], out result);
                    IComponent componentInfo = this.GetComponentInfo(@params["tcm-id"]);
                    ShoppingListItem item4 = new ShoppingListItem {
                        AddedDateTime = DateTime.Now,
                        Brand = componentInfo.Fields["brand"].Value,
                        ProductTcmID = @params["tcm-id"],
                        ProductName = componentInfo.Fields["title"].Value,
                        ProjectName = str,
                        Quantity = result,
                        UserID = httpContext.User.Identity.Name
                    };
                    ShoppingListGateway gateway2 = new ShoppingListGateway();
                    if ((gateway2.InsertShoppingListItem(item4) != null) && !string.IsNullOrEmpty(@params["ReturnUrl"]))
                    {
                        filterContext.HttpContext.Session.Add("feedback", Helper.GetResource("Feedback_AddedToShoppingList"));
                        filterContext.Result = new RedirectResult(@params["ReturnUrl"]);
                    }
                    base.OnActionExecuting(filterContext);
                }
                if (@params["action"] == "ReturnUrl")
                {
                    if (!string.IsNullOrEmpty(@params["ReturnUrl"]))
                    {
                        filterContext.Result = new RedirectResult(@params["ReturnUrl"]);
                    }
                    base.OnActionExecuting(filterContext);
                }
                if (@params["action"] == "download")
                {
                    if (!string.IsNullOrEmpty(@params["itemLink"]))
                    {
                        filterContext.Result = new RedirectResult(@params["ReturnUrl"] + "?download=" + @params["itemLink"]);
                    }
                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }
}

