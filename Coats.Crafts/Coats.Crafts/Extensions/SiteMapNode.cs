using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Extensions
{
    public static class SiteMapNodeExtensions
    {
        public static bool ToBoolean(this String input)
        {
            if (String.IsNullOrEmpty(input))
                return false;

            return input == "Yes";
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

        public static string view(this SiteMapNode node)
        {
            return node["view"];
        }

        /// <summary>
        /// Return only nodes which are not listed as invisible and which aren't
        /// buried below n levels
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static List<SiteMapNode> VisibleChildNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from SiteMapNode n in node.ChildNodes
                    where
                        (!n.HideFromNav()) && (n.Url.Count(c => c == '/') <= urlDepth)
                    select n).ToList();
        }


        public static List<SiteMapNode> GetAllFilteredNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from SiteMapNode n in node.GetAllNodes()
                    where (n.IncludeInFilter()) && (!n.HideFromNav()) && (n.Url.Count(c => c == '/') <= urlDepth)
                    select n).ToList();
        }

        public static List<SiteMapNode> GetAllFilteredQuickLinkNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from SiteMapNode n in node.ChildNodes
                    where
                        (n.IncludeInQuickLinks()) && (n.Url.Count(c => c == '/') <= urlDepth)
                    select n).ToList();
        }

        public static List<SiteMapNode> GetAllFilteredSiteMapNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (from SiteMapNode n in node.ChildNodes
                    where
                        (n.IncludeInSiteMap()) && (n.Url.Count(c => c == '/') <= urlDepth)
                    select n).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static bool HasVisibleChildNodes(this SiteMapNode node, int urlDepth = 4)
        {
            return (node.VisibleChildNodes(urlDepth).Count > 0)
                       ? true
                       : false;
        }
    }
}