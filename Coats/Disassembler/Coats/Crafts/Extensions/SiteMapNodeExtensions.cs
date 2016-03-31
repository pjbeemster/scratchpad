namespace Coats.Crafts.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web;

    public static class SiteMapNodeExtensions
    {
        public static List<SiteMapNode> GetAllFilteredNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from n in node.GetAllNodes().Cast<SiteMapNode>()
                where (n.IncludeInFilter() && !n.HideFromNav()) && (n.Url.Count<char>(c => (c == '/')) <= urlDepth)
                select n).ToList<SiteMapNode>();
        }

        public static List<SiteMapNode> GetAllFilteredQuickLinkNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from n in node.ChildNodes.Cast<SiteMapNode>()
                where n.IncludeInQuickLinks() && (n.Url.Count<char>(c => (c == '/')) <= urlDepth)
                select n).ToList<SiteMapNode>();
        }

        public static List<SiteMapNode> GetAllFilteredSiteMapNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from n in node.ChildNodes.Cast<SiteMapNode>()
                where n.IncludeInSiteMap() && (n.Url.Count<char>(c => (c == '/')) <= urlDepth)
                select n).ToList<SiteMapNode>();
        }

        public static bool HasVisibleChildNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (node.VisibleChildNodes(urlDepth).Count > 0);
        }

        public static bool HideFromNav(this SiteMapNode node)
        {
            return !node["primarynavigation"].ToBoolean();
        }

        public static bool IncludeInFilter(this SiteMapNode node)
        {
            return node["useinfilter"].ToBoolean();
        }

        public static bool IncludeInQuickLinks(this SiteMapNode node)
        {
            return node["showinquicklinks"].ToBoolean();
        }

        public static bool IncludeInSiteMap(this SiteMapNode node)
        {
            return node["showinsitemap"].ToBoolean();
        }

        public static bool ToBoolean(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return (input == "Yes");
        }

        public static string view(this SiteMapNode node)
        {
            return node["view"];
        }

        public static List<SiteMapNode> VisibleChildNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from n in node.ChildNodes.Cast<SiteMapNode>()
                where !n.HideFromNav() && (n.Url.Count<char>(c => (c == '/')) <= urlDepth)
                select n).ToList<SiteMapNode>();
        }
    }
}

