using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Castle.Core.Logging;

using Coats.Crafts.Extensions;
using Coats.Crafts.Filters;
using Coats.Crafts.Configuration;
using Coats.Crafts.ConfigurationHelpers;
using Coats.Crafts.Resources;

using DD4T.ContentModel;
using DD4T.ContentModel.Exceptions;
using DD4T.Mvc.Attributes;
using DD4T.Mvc.Controllers;
using DD4T.Utils;

namespace Coats.Crafts.Controllers
{
    
    public class TridionPageController : TridionControllerBase
    {
        public ILogger Logger { get; set; }
        private string UrlPath = string.Empty;
        private Regex reDefaultPage = new Regex(@".*/[^/\.]*(/?)$");

        [CheckActionFilter]
        [CheckCountryFilter]
        [UsesTridionPage(PageUrl = "/theme.config", ThrowExceptionIfPageNotFound = false)]
        public override ActionResult Page(string pageId)
        {
            if (Request.QueryString["resetFilters"] == null)
            {
                // check for server variable to set up brand filter
                if (Request.ServerVariables["HTTP_X_BRAND_FILTER"] != null)
                {
                    // See... http://stackoverflow.com/questions/3826299/asp-net-mvc-urlhelper-generateurl-exception-cannot-use-a-leading-to-exit-ab
                    HttpContext.Request.ServerVariables.Remove("IIS_WasUrlRewritten");

                    // set brand session
                    Session.SetLevel1BrandFilter(Request.ServerVariables["HTTP_X_BRAND_FILTER"]);
                    //Session["BRAND_FILTER"] = Request.ServerVariables["HTTP_X_BRAND_FILTER"];
                }
            }
            else
            {
                Session.ClearLevel1BrandFilter();
                //Session["BRAND_FILTER"] = null;
            }

            //// Check if this page is being hit from a a search result item (e.g. faceted content).
            //// This will be denoted by the Query String "rts" (return to search).
            //// This is to decide whether to inject the "< Search results" breadcrumb link.
            //if (Request.QueryString.AllKeys.Contains("rts"))
            //{
            //    try
            //    {
            //        if (Request.QueryString["rts"].ToLower() == "true")
            //        {
            //            ViewBag.ReturnToSearch = true;
            //        }
            //    }
            //    catch (Exception) { }
            //}

            pageId = ParseUrl(pageId);

            UrlPath = pageId;

            var model = base.GetModelForPage(pageId);
            if (model == null) { throw new HttpException(404, "Page cannot be found"); }


            SetPageHeaderInfo(model);
            ViewBag.Renderer = base.ComponentPresentationRenderer;


            return base.GetView(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Xml(string pageId)
        {
            LoggerService.Information(">>PageController.Xml (url={0})", pageId);

            pageId = ParseUrl(pageId);
            if (!pageId.StartsWith("/"))
                pageId = "/" + pageId;

            return GetPlainContent(pageId, "text/xml", Encoding.UTF8);
        }

        public ContentResult GetPlainContent(string url, string contentType, Encoding encoding)
        {
            
            try
            {
                UrlPath = url;
                string raw = base.PageFactory.FindPageContent(url);
                return new ContentResult
                {
                    ContentType = contentType,
                    Content = raw,
                    ContentEncoding = encoding
                };
            }
            catch (PageNotFoundException)
            {
                //throw new HttpException(404, "Page cannot be found");
                throw new HttpException(404, Helper.GetResource("PageCannotBeFound"));
                
            }
            catch (SecurityException se)
            {
                throw new HttpException(403, se.Message);
            }
        }

        public void SetCookie()
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get("CoatsCraftsCookie");
            if (cookie == null)
            {
                // Create cookie.
                cookie = new HttpCookie("CoatsCraftsCookie");
                cookie.Value = DateTime.Now.ToString();
                cookie.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Response.Cookies.Add(cookie);
            }
    
        }

        private string ParseUrl(string pageId)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                pageId = DD4T.Utils.ConfigurationHelper.DefaultPage;
            }
            else
            {
                if (reDefaultPage.IsMatch("/" + pageId))
                {
                    if (pageId.EndsWith("/"))
                    {
                        pageId += DD4T.Utils.ConfigurationHelper.DefaultPage;
                    }
                    else
                    {
                        pageId += "/" + DD4T.Utils.ConfigurationHelper.DefaultPage;
                    }
                }
            }
            return pageId;
        }

        private void SetPageHeaderInfo(IPage model)
        {
            StringBuilder title = new StringBuilder();
            StringBuilder headTags = new StringBuilder();
            if (model.MetadataFields.ContainsKey("seo"))
            {
                IComponent seo = model.MetadataFields["seo"].LinkedComponentValues[0];

                string pagetitle = seo.Fields.ContainsKey("page_title") ? seo.Fields["page_title"].Value : String.Empty;
                if (String.IsNullOrEmpty(pagetitle))
                {
                    title.Append(PageTitles(model));
                }
                else
                {
                    title.Append(pagetitle);
                }

                //title.Append(seo.Fields.ContainsKey("page_title") ? seo.Fields["page_title"].Value : String.Empty);
                //ViewBag.Title = seo.Fields["page_title"].Value;
                ViewBag.Description = seo.Fields.ContainsKey("description") ? seo.Fields["description"].Value : string.Empty;
                ViewBag.CanonicalLink = seo.Fields.ContainsKey("canonical_link") ? seo.Fields["canonical_link"].Value : string.Empty;
                // ViewBag.HeadTags = seo.Fields.ContainsKey("head_tags") ? seo.Fields["head_tags"].Value : string.Empty;
                headTags.Append(seo.Fields.ContainsKey("head_tags") ? seo.Fields["head_tags"].Value : string.Empty);
                //Added by Ajaya for HREF Change
                if (!UrlPath.StartsWith("/"))
                    UrlPath = "/" + UrlPath;

                string[] SameUrls = base.PageFactory.GetAllPublishedPageUrlsWithSame(UrlPath);

                for (int urlscnt = 0; urlscnt < SameUrls.Length; urlscnt++)
                {
                    string[] urlpart = SameUrls[urlscnt].Split("/".ToCharArray());
                    //take domain name & country url like CRAFTS
                    string HrefKey = urlpart[1] + urlpart[2];
                    string Middlepart = SameUrls[urlscnt].Replace("/" + urlpart[1] + "/" + urlpart[2], "");
                     Middlepart = Middlepart.Substring(0,(Middlepart.LastIndexOf("/")+1));
                    string HrefUrl = ConfigurationManager.AppSettings[HrefKey];
                    if (HrefUrl != null && HrefUrl.Length > 0)
                    {
                        string hreflang = HrefUrl.Substring(HrefUrl.IndexOf(".com") + 5);
                        
                        hreflang = hreflang.ToString().TrimEnd("/".ToCharArray());
                        if (hreflang.Contains(ConfigurationManager.AppSettings["Default"].ToLower()))
                            headTags.Append("<link rel=" + "\"" + "x-default" + "\" ");
                        else
                            headTags.Append("<link rel=" + "\"" + "alternate" + "\" ");
                        headTags.Append("hreflang="+"\"" + hreflang + "\" ");
                        headTags.Append("href= "+"\"" + HrefUrl + Middlepart.Trim() + "\" ");
                        headTags.Append("/> ");
                        headTags.AppendLine();

                    }

                }

                ViewBag.HeadTags = headTags.ToString();
            }
            else
            {
                title.Append(PageTitles(model));
                //ViewBag.Title = model.Title;
            }

            title.Append(WebConfiguration.Current.CoatsStrapline);

            ViewBag.Title = title.ToString().TrimEnd(", ".ToCharArray());

            //Add a Facebook open graph tag for the banner image
            if (model.ComponentPresentations.Count > 0)
            {
                var bannerImg = model.ComponentPresentations[0].Component.Fields.ContainsKey("banner") ?
                            model.ComponentPresentations[0].Component.Fields["banner"] : null;

                if (bannerImg != null) {  ViewBag.OGImageTag = HtmlHelpers.ImageHelper.GetImagePhysicalPath(bannerImg);}
            }
        }

        /// <summary>
        /// Depending upon the content type, adjust the page title to include other info - this only happen though if no SEO component or
        /// an SEO component without a title has been used.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string PageTitles(IPage model)
        {
            string title = model.Title;

            if (model.ComponentPresentations.Count > 0)
            {
                title = model.ComponentPresentations[0].Component.Fields.ContainsKey("title") ? model.ComponentPresentations[0].Component.Fields["title"].Value : title;

                if (model.ComponentPresentations[0].Component.Schema.Title != "")
                {
                    string schemaTitle = model.ComponentPresentations[0].Component.Schema.Title;

                    //if (schemaTitle.Equals(WebConfiguration.Current.ContentTypeArticleTitle))
                    //{
                    //    // No further keywords to be added yet
                    //}
                    //else if (schemaTitle.Equals(WebConfiguration.Current.ContentTypeMoodboard))
                    //{
                    //    // No further keywords to be added yet
                    //}

                    if (schemaTitle.Equals(WebConfiguration.Current.ContentTypeProject))
                    {
                        title += " - " + WebConfiguration.Current.ProductFree + " " + FacetKeywords(model);
                    }
                    else if (schemaTitle.Equals(WebConfiguration.Current.ContentTypeDesigner))
                    {
                        title += " : " + WebConfiguration.Current.PageTitleProfileAppend;
                    }
                    else if (schemaTitle.Equals(WebConfiguration.Current.ContentTypeBrand))
                    {
                        title += " : " + WebConfiguration.Current.PageTitleBrandAppend;
                    }
                    else if (WebConfiguration.Current.ContentTypeProduct.Match(schemaTitle))
                    {
                        var prods = model.ComponentPresentations[0].Component.Fields.ContainsKey("product_group") ? model.ComponentPresentations[0].Component.Fields["product_group"].Value : "";
                        title += " : " + prods + " " + WebConfiguration.Current.PageTitleBrandAppend;
                    }
                }
            }
            return title;
        }


        private string FacetKeywords(IPage model)
        {
            StringBuilder facetKeywords = new StringBuilder();

            // Check first CP for any facets to use to append to title
            if (model.ComponentPresentations.Count > 0)
            {
                IComponent component = model.ComponentPresentations[0].Component;
                if (component.MetadataFields.ContainsKey("facets"))
                {
                    // Get component containing facets
                    IComponent facets = component.MetadataFields["facets"].LinkedComponentValues[0];
                    if (facets.Fields.Count > 0)
                        facetKeywords.Append(" - ");

                    // For each field...
                    facets.Fields.ToList().ForEach(f =>
                    {
                        // Only want keywords ...
                        f.Value.Keywords.ToList().ForEach(k =>
                        {
                            if (!String.IsNullOrEmpty(k.Description))
                                facetKeywords.AppendFormat("{0}, ", k.Description);
                        });
                    });
                }
            }

            return facetKeywords.ToString();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            // Output a custom error page with correct status code
            // we have to do this because custom errors causes a 302 then 200 which is incorrect

            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                int errCode = 500; //default

                var httpException = filterContext.Exception as HttpException;

                if (httpException != null)
                {
                    errCode = httpException.GetHttpCode();
                }

                filterContext.ExceptionHandled = true;

                string errorPagePath;

                if (errCode == 404)
                {
                    errorPagePath = WebConfiguration.Current.App404Path;
                    Logger.Debug("TridionPageController Exception: ", filterContext.Exception);
                }
                else {
                    errorPagePath = WebConfiguration.Current.AppErrorPath;
                    Logger.ErrorFormat("TridionPageController Exception: {0}", filterContext.RequestContext.HttpContext.Request.Url.ToString());
                    Logger.Error("TridionPageController Exception: ", filterContext.Exception);
                }

                var error = base.GetModelForPage(errorPagePath);

                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/TridionPage/ContentPage.cshtml",
                    MasterName = "~/Views/Shared/_Layout.cshtml",
                    ViewData = new ViewDataDictionary { Model = error }
                };

                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = errCode;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }

    }
}
