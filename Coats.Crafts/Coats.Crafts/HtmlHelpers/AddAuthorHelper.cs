using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DD4T.ContentModel;
using Coats.Crafts.Resources;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Extensions;

namespace Coats.Crafts.HtmlHelpers
{
    public static class AddAuthorHelper
    {
        //
        // GET: /AddAuthorHelper/
       
        public static MvcHtmlString addAuthor(this HtmlHelper helper, string prefix, bool avatar)
        {
            IComponentPresentation model = (IComponentPresentation)helper.ViewData.Model;

            if (prefix != "")
            {
                prefix = Helper.GetResource(prefix);
            } else {
                prefix = Helper.GetResource("WrittenBy");
            }

            var fields = model.Component.Fields;

            MvcHtmlString author = MvcHtmlString.Empty;

            string auth = string.Empty;

            if (fields.ContainsKey("by"))
            {
                if (fields["by"].EmbeddedValues[0].ContainsKey("author"))
                {
                    auth = prefix + "&nbsp;" + fields["by"].EmbeddedValues[0]["author"].Value;
                    author = new MvcHtmlString(auth);
                }

                if (fields["by"].EmbeddedValues[0].ContainsKey("designer"))
                {
                    if (fields["by"].EmbeddedValues[0].Count > 0)
                    {
                        if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues.Count > 0)
                        {
                            string commenter = string.Empty;
                            if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields.ContainsKey("commenter"))
                            {
                                commenter = fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["commenter"].Value;
                            }

                            HtmlString grav = new HtmlString("");

                            if (avatar == true)
                            {
                                grav = GravatarHtmlHelper.GravatarImage(helper, commenter, 30);
                            }

                            var urlHelper = new UrlHelper(((MvcHandler)HttpContext.Current.Handler).RequestContext);

                            auth = grav + " " + prefix + "&nbsp;<a href='" + urlHelper.Content(fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].GetResolvedUrl().AddApplicationRoot()) + "' class='author' itemprop='author'>" + fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["title"].Value + "</a>";
                            author = new MvcHtmlString(auth);
                        }
                    }
                }
            }

            return author;
        }

        public static MvcHtmlString addAuthor(this HtmlHelper helper, IComponent comp)
        {
            var fields = comp.Fields;

            MvcHtmlString author = MvcHtmlString.Empty;

            if (fields.ContainsKey("by"))
            {
                if (fields["by"].EmbeddedValues[0].ContainsKey("author"))
                {
                    author = fields["by"].EmbeddedValues[0]["author"].Value.ResolveRichText();
                }

                if (fields["by"].EmbeddedValues[0].ContainsKey("designer"))
                {
                    if (fields["by"].EmbeddedValues[0].Count > 0)
                    {
                        if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues.Count > 0)
                        {
                            if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields.ContainsKey("title"))
                            {
                                author = fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["title"].Value.ResolveRichText();
                            }
                        }
                    }
                }
            }

            return author;
        }

    }
}
