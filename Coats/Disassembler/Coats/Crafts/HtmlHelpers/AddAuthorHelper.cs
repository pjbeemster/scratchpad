namespace Coats.Crafts.HtmlHelpers
{
    using Coats.Crafts.Extensions;
    using Coats.Crafts.Resources;
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Mvc;

    public static class AddAuthorHelper
    {
        public static MvcHtmlString addAuthor(this HtmlHelper helper, IComponent comp)
        {
            IFieldSet fields = comp.Fields;
            MvcHtmlString empty = MvcHtmlString.Empty;
            if (fields.ContainsKey("by"))
            {
                if (fields["by"].EmbeddedValues[0].ContainsKey("author"))
                {
                    empty = fields["by"].EmbeddedValues[0]["author"].Value.ResolveRichText();
                }
                if (!fields["by"].EmbeddedValues[0].ContainsKey("designer"))
                {
                    return empty;
                }
                if (fields["by"].EmbeddedValues[0].Count <= 0)
                {
                    return empty;
                }
                if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues.Count <= 0)
                {
                    return empty;
                }
                if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields.ContainsKey("title"))
                {
                    empty = fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["title"].Value.ResolveRichText();
                }
            }
            return empty;
        }

        public static MvcHtmlString addAuthor(this HtmlHelper helper, string prefix, bool avatar)
        {
            IComponentPresentation model = (IComponentPresentation) helper.ViewData.Model;
            if (prefix != "")
            {
                prefix = Helper.GetResource(prefix);
            }
            else
            {
                prefix = Helper.GetResource("WrittenBy");
            }
            IFieldSet fields = model.Component.Fields;
            MvcHtmlString empty = MvcHtmlString.Empty;
            if (!fields.ContainsKey("by"))
            {
                return empty;
            }
            if (fields["by"].EmbeddedValues[0].ContainsKey("author"))
            {
                empty = new MvcHtmlString(prefix + "&nbsp;" + fields["by"].EmbeddedValues[0]["author"].Value);
            }
            if (!fields["by"].EmbeddedValues[0].ContainsKey("designer"))
            {
                return empty;
            }
            if (fields["by"].EmbeddedValues[0].Count <= 0)
            {
                return empty;
            }
            if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues.Count <= 0)
            {
                return empty;
            }
            string emailAddress = string.Empty;
            if (fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields.ContainsKey("commenter"))
            {
                emailAddress = fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["commenter"].Value;
            }
            HtmlString str4 = new HtmlString("");
            if (avatar)
            {
                str4 = helper.GravatarImage(emailAddress, 30, GravatarHtmlHelper.DefaultImage.MysteryMan, "", false, GravatarHtmlHelper.Rating.G, false);
            }
            UrlHelper helper2 = new UrlHelper(((MvcHandler) HttpContext.Current.Handler).RequestContext);
            return new MvcHtmlString(string.Concat(new object[] { str4, " ", prefix, "&nbsp;<a href='", helper2.Content(fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].GetResolvedUrl().AddApplicationRoot()), "' class='author' itemprop='author'>", fields["by"].EmbeddedValues[0]["designer"].LinkedComponentValues[0].Fields["title"].Value, "</a>" }));
        }
    }
}

