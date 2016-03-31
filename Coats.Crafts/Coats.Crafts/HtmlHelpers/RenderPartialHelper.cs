using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Coats.Crafts.HtmlHelpers
{
    public static class RenderPartialHelper
    {
        /// <summary>
        /// Custom extension overload to enable the prefixing of each id and name attribute of all rendered HTML tags.
        /// Used to enable binding of complex models (nested models) back to the property of container model.
        /// Just specify the name of the property after the model object.
        /// e.g.
        /// @{Html.RenderPartial("SomePartialView", Model.ComponentTypes, "ComponentTypes");}
        /// </summary>
        /// <param name="htmlHelper">Extension binding type</param>
        /// <param name="partialViewName">The name of the partial view to render</param>
        /// <param name="model">The model to pass into the partial view</param>
        /// <param name="htmlFieldPrefix">The name of the property in the container model. e.g. for Model.ComponentTypes pass in "ComponentTypes"</param>
        public static void RenderPartial(this HtmlHelper htmlHelper, string partialViewName, object model, string htmlFieldPrefix)
        {
            // Store the existing HtmlFieldPrefix so we can return it back later
            string originalHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            // Set the HtmlFieldPrefix
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            // Call the nice MVC RenderPartial
            htmlHelper.RenderPartial(partialViewName, model);
            // Restore the HtmlFieldPrefix to the original
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = originalHtmlFieldPrefix;
        }
    }
}