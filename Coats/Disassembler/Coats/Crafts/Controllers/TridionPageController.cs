namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ConfigurationHelpers;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Filters;
    using Coats.Crafts.HtmlHelpers;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using DD4T.ContentModel.Exceptions;
    using DD4T.Mvc.Attributes;
    using DD4T.Mvc.Controllers;
    using DD4T.Utils;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;

    public class TridionPageController : TridionControllerBase
    {
        private Regex reDefaultPage = new Regex(@".*/[^/\.]*(/?)$");
        private string UrlPath = string.Empty;

        private string FacetKeywords(IPage model)
        {
            Action<KeyValuePair<string, IField>> action = null;
            StringBuilder facetKeywords = new StringBuilder();
            if (model.ComponentPresentations.Count > 0)
            {
                IComponent component = model.ComponentPresentations[0].Component;
                if (component.MetadataFields.ContainsKey("facets"))
                {
                    IComponent component2 = component.MetadataFields["facets"].LinkedComponentValues[0];
                    if (component2.Fields.Count > 0)
                    {
                        facetKeywords.Append(" - ");
                    }
                    if (action == null)
                    {
                        action = delegate (KeyValuePair<string, IField> f) {
                            f.Value.Keywords.ToList<IKeyword>().ForEach(delegate (IKeyword k) {
                                if (!string.IsNullOrEmpty(k.Description))
                                {
                                    facetKeywords.AppendFormat("{0}, ", k.Description);
                                }
                            });
                        };
                    }
                    component2.Fields.ToList<KeyValuePair<string, IField>>().ForEach(action);
                }
            }
            return facetKeywords.ToString();
        }

        public ContentResult GetPlainContent(string url, string contentType, Encoding encoding)
        {
            ContentResult result2;
            try
            {
                this.UrlPath = url;
                string str = base.PageFactory.FindPageContent(url);
                result2 = new ContentResult {
                    ContentType = contentType,
                    Content = str,
                    ContentEncoding = encoding
                };
            }
            catch (PageNotFoundException)
            {
                throw new HttpException(0x194, Helper.GetResource("PageCannotBeFound"));
            }
            catch (SecurityException exception)
            {
                throw new HttpException(0x193, exception.Message);
            }
            return result2;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                string appErrorPath;
                int httpCode = 500;
                HttpException exception = filterContext.Exception as HttpException;
                if (exception != null)
                {
                    httpCode = exception.GetHttpCode();
                }
                filterContext.ExceptionHandled = true;
                if (httpCode == 0x194)
                {
                    appErrorPath = WebConfiguration.Current.App404Path;
                    this.Logger.Debug("TridionPageController Exception: ", filterContext.Exception);
                }
                else
                {
                    appErrorPath = WebConfiguration.Current.AppErrorPath;
                    this.Logger.ErrorFormat("TridionPageController Exception: {0}", new object[] { filterContext.RequestContext.HttpContext.Request.Url.ToString() });
                    this.Logger.Error("TridionPageController Exception: ", filterContext.Exception);
                }
                IPage modelForPage = base.GetModelForPage(appErrorPath);
                ViewResult result = new ViewResult {
                    ViewName = "~/Views/TridionPage/ContentPage.cshtml",
                    MasterName = "~/Views/Shared/_Layout.cshtml"
                };
                ViewDataDictionary dictionary = new ViewDataDictionary {
                    Model = modelForPage
                };
                result.ViewData = dictionary;
                filterContext.Result = result;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = httpCode;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

        [CheckActionFilter, CheckCountryFilter, UsesTridionPage(PageUrl="/theme.config", ThrowExceptionIfPageNotFound=false)]
        public override ActionResult Page(string pageId)
        {
            if (base.Request.QueryString["resetFilters"] == null)
            {
                if (base.Request.ServerVariables["HTTP_X_BRAND_FILTER"] != null)
                {
                    base.HttpContext.Request.ServerVariables.Remove("IIS_WasUrlRewritten");
                    base.Session.SetLevel1BrandFilter(base.Request.ServerVariables["HTTP_X_BRAND_FILTER"]);
                }
            }
            else
            {
                base.Session.ClearLevel1BrandFilter();
            }
            pageId = this.ParseUrl(pageId);
            this.UrlPath = pageId;
            IPage modelForPage = base.GetModelForPage(pageId);
            if (modelForPage == null)
            {
                throw new HttpException(0x194, "Page cannot be found");
            }
            this.SetPageHeaderInfo(modelForPage);
            ((dynamic) base.ViewBag).Renderer = base.ComponentPresentationRenderer;
            return base.GetView(modelForPage);
        }

        private string PageTitles(IPage model)
        {
            string title = model.Title;
            if (model.ComponentPresentations.Count > 0)
            {
                string str5;
                title = model.ComponentPresentations[0].Component.Fields.ContainsKey("title") ? model.ComponentPresentations[0].Component.Fields["title"].Value : title;
                if (model.ComponentPresentations[0].Component.Schema.Title == "")
                {
                    return title;
                }
                string setting = model.ComponentPresentations[0].Component.Schema.Title;
                if (setting.Equals(WebConfiguration.Current.ContentTypeProject))
                {
                    str5 = title;
                    return (str5 + " - " + WebConfiguration.Current.ProductFree + " " + this.FacetKeywords(model));
                }
                if (setting.Equals(WebConfiguration.Current.ContentTypeDesigner))
                {
                    return (title + " : " + WebConfiguration.Current.PageTitleProfileAppend);
                }
                if (setting.Equals(WebConfiguration.Current.ContentTypeBrand))
                {
                    return (title + " : " + WebConfiguration.Current.PageTitleBrandAppend);
                }
                if (WebConfiguration.Current.ContentTypeProduct.Match(setting))
                {
                    string str3 = model.ComponentPresentations[0].Component.Fields.ContainsKey("product_group") ? model.ComponentPresentations[0].Component.Fields["product_group"].Value : "";
                    str5 = title;
                    title = str5 + " : " + str3 + " " + WebConfiguration.Current.PageTitleBrandAppend;
                }
            }
            return title;
        }

        private string ParseUrl(string pageId)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                pageId = DD4T.Utils.ConfigurationHelper.DefaultPage;
                return pageId;
            }
            if (this.reDefaultPage.IsMatch("/" + pageId))
            {
                if (pageId.EndsWith("/"))
                {
                    pageId = pageId + DD4T.Utils.ConfigurationHelper.DefaultPage;
                    return pageId;
                }
                pageId = pageId + "/" + DD4T.Utils.ConfigurationHelper.DefaultPage;
            }
            return pageId;
        }

        public void SetCookie()
        {
            if (base.HttpContext.Request.Cookies.Get("CoatsCraftsCookie") == null)
            {
                HttpCookie cookie = new HttpCookie("CoatsCraftsCookie") {
                    Value = DateTime.Now.ToString(),
                    Expires = DateTime.Now.AddMonths(1)
                };
                base.HttpContext.Response.Cookies.Add(cookie);
            }
        }

        private void SetPageHeaderInfo(IPage model)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            if (model.MetadataFields.ContainsKey("seo"))
            {
                IComponent component = model.MetadataFields["seo"].LinkedComponentValues[0];
                string str = component.Fields.ContainsKey("page_title") ? component.Fields["page_title"].Value : string.Empty;
                if (string.IsNullOrEmpty(str))
                {
                    builder.Append(this.PageTitles(model));
                }
                else
                {
                    builder.Append(str);
                }
                ((dynamic) base.ViewBag).Description = component.Fields.ContainsKey("description") ? component.Fields["description"].Value : string.Empty;
                ((dynamic) base.ViewBag).CanonicalLink = component.Fields.ContainsKey("canonical_link") ? component.Fields["canonical_link"].Value : string.Empty;
                builder2.Append(component.Fields.ContainsKey("head_tags") ? component.Fields["head_tags"].Value : string.Empty);
                if (!this.UrlPath.StartsWith("/"))
                {
                    this.UrlPath = "/" + this.UrlPath;
                }
                string[] allPublishedPageUrlsWithSame = base.PageFactory.GetAllPublishedPageUrlsWithSame(this.UrlPath);
                for (int i = 0; i < allPublishedPageUrlsWithSame.Length; i++)
                {
                    string[] strArray2 = allPublishedPageUrlsWithSame[i].Split("/".ToCharArray());
                    string str2 = strArray2[1] + strArray2[2];
                    string str3 = allPublishedPageUrlsWithSame[i].Replace("/" + strArray2[1] + "/" + strArray2[2], "");
                    str3 = str3.Substring(0, str3.LastIndexOf("/"));
                    string str4 = ConfigurationManager.AppSettings[str2];
                    if ((str4 != null) && (str4.Length > 0))
                    {
                        string str5 = str4.Substring(str4.IndexOf(".com") + 5).ToString().TrimEnd("/".ToCharArray());
                        if (str5.Contains(ConfigurationManager.AppSettings["Default"].ToLower()))
                        {
                            builder2.Append("<link rel=\"x-default\" ");
                        }
                        else
                        {
                            builder2.Append("<link rel=\"alternate\" ");
                        }
                        builder2.Append("hreflang=\"" + str5 + "\" ");
                        builder2.Append("href= \"" + str4 + str3.Trim() + "\" ");
                        builder2.Append("/> ");
                        builder2.AppendLine();
                    }
                }
                ((dynamic) base.ViewBag).HeadTags = builder2.ToString();
            }
            else
            {
                builder.Append(this.PageTitles(model));
            }
            builder.Append(WebConfiguration.Current.CoatsStrapline);
            ((dynamic) base.ViewBag).Title = builder.ToString().TrimEnd(", ".ToCharArray());
            if (model.ComponentPresentations.Count > 0)
            {
                IField image = model.ComponentPresentations[0].Component.Fields.ContainsKey("banner") ? model.ComponentPresentations[0].Component.Fields["banner"] : null;
                if (image != null)
                {
                    ((dynamic) base.ViewBag).OGImageTag = image.GetImagePhysicalPath();
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Xml(string pageId)
        {
            LoggerService.Information(">>PageController.Xml (url={0})", new object[] { pageId });
            pageId = this.ParseUrl(pageId);
            if (!pageId.StartsWith("/"))
            {
                pageId = "/" + pageId;
            }
            return this.GetPlainContent(pageId, "text/xml", Encoding.UTF8);
        }

        public ILogger Logger { get; set; }
    }
}

