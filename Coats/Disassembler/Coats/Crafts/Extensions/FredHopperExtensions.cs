namespace Coats.Crafts.Extensions
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Text.RegularExpressions;

    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;

    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using Coats.Crafts.FredHopper;
    using Coats.Crafts.FASWebService;
    using Coats.Crafts.ControllerHelpers;

    using DD4T.ContentModel;

    public static class FredHopperExtensions
    {
        private static string CreateOrCondition(string urlParams)
        {
            string str = HttpUtility.UrlDecode(urlParams).Split(new char[] { '&' }).SingleOrDefault<string>(p => p.StartsWith("fh_location=", StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(str))
            {
                str = urlParams;
            }
            Location location = new Location(str.Replace("fh_location=", ""));
            SearchCriterion criterion = location.getSearchCriterion();
            if (criterion != null)
            {
                location.removeSearchCriteria();
            }
            Criterion criterion2 = location.getLastCriterion();
            Criterion criterion3 = location.getCriterion(criterion2.getAttributeName());
            if (criterion3 == criterion2)
            {
                if (criterion != null)
                {
                    location.addCriterion(criterion);
                    return location.toString();
                }
                return urlParams;
            }
            string str2 = criterion2.toString();
            str2 = str2.Substring(str2.IndexOf('{') + 1).Replace("}", "");
            location = location.removeLastCriterion();
            string str3 = criterion3.toString().Replace("}", ";" + str2 + "}");
            location = location.removeCriteria(criterion3.getAttributeName());
            Criterion criterion4 = CriterionFactory.parse(str3);
            location.addCriterion(criterion4);
            if (criterion != null)
            {
                location.addCriterion(criterion);
            }
            return location.toString();
        }

        public static FacetSection getChildSectionRecursive(filter child, string fh_location, IEnumerable<filter> childFilters)
        {
            FacetSection childSection = new FacetSection();
            childSection.Facets = new List<FacetItem>();
            childSection.SectionTitle = child.title;
            childSection.CustomFields = child.customfields;
            List<filtersection> childFilterSections = child.filtersection.ToList();
            foreach (var cfs in childFilterSections)
            {
                FacetItem childFacetItem = getFacetItem(cfs, fh_location, child);
                if (childFacetItem != null)
                {
                    childFacetItem.Selected = cfs.selected;
                    var rChild = childFilters.Where(s => s.on == cfs.value.Value).FirstOrDefault();
                    if (rChild != null)
                    {
                        childFacetItem.childSection = getChildSectionRecursive(rChild, fh_location, childFilters);
                    }
                    childSection.Facets.Add(childFacetItem);
                }
            }
            return childSection;
        }

        public static FacetItem getFacetItem(filtersection fs, string fh_location, filter f)
        {
            FacetItem item = new FacetItem {
                FHId = fs.value.Value,
                NumItems = fs.nr,
                Selected = fs.selected
            };
            link link = fs.link;
            item.Text = link.name;
            Query query = new Query();
            query.ParseQuery(link.urlparams);
            query.removeListStartIndex();
            string urlParams = string.Empty;
            if (fs.selected)
            {
                string str2 = query.getLocation().getLastCriterion().toString();
                string facetValue = str2.Substring(str2.IndexOf('{') + 1).Replace("}", "");
                urlParams = RemoveFacetValues(fh_location, query.getLocation().getLastCriterion().getAttributeName(), facetValue);
            }
            else
            {
                Location location = new Location(fh_location);
                location.addCriterion(query.getLocation().getLastCriterion());
                urlParams = location.toString();
                if ((f.customfields != null) && (f.customfields.SingleOrDefault<customfield>(c => (c.name == "RefineMethod")).Value == "CombineFacets"))
                {
                    urlParams = CreateOrCondition(urlParams);
                }
            }
            Location location2 = new Location(urlParams);
            query.setLocation(location2);
            item.Value = query.ToFhParams();
            return item;
        }

        public static List<FacetPagination> GetFacetPagination(this results r, Query query, int itemsPerPage)
        {
            int num = r.total_items;
            List<FacetPagination> list = new List<FacetPagination>();
            int num2 = num / itemsPerPage;
            num2 += ((num % itemsPerPage) == 0) ? 0 : 1;
            int offset = query.getListStartIndex();
            for (int i = 0; i < num2; i++)
            {
                int num5 = i * itemsPerPage;
                FacetPagination item = new FacetPagination();
                item.LinkText = (i + 1).ToString();
                item.Selected = offset == num5;
                string str = query.setListStartIndex(i * itemsPerPage).toString();
                item.Url = "?fh_params=" + HttpUtility.UrlEncode(str);
                if (!string.IsNullOrEmpty(HttpContext.Current.Request["fh_ticked"]))
                {
                    item.Url = item.Url + "&fh_ticked=" + HttpContext.Current.Request["fh_ticked"];
                }
                list.Add(item);
            }
            query.setListStartIndex(offset);
            return list;
        }

        public static FacetCollection GetHierarchicalFacetCollection(this facetmap fm, string fh_location, int publicationId)
        {
            FacetCollection facets = new FacetCollection();
            if (fm.filter == null)
            {
                facets.FacetSections = new List<FacetSection>();
                facets.ResetFacetsUrl = Resetfilters(fh_location, false, publicationId).toString();
                return facets;
            }

            List<filter> filters = fm.filter.ToList();
            var uniqueFilters = filters.GroupBy(f => f.title).Select(grp => grp.First());
            IEnumerable<filter> childFilters = uniqueFilters.Where(f => f.on.Contains(FacetedContentHelper.NestedLevelDelimiter));
            IEnumerable<filter> parentFilters = uniqueFilters.Where(f => !f.on.Contains(FacetedContentHelper.NestedLevelDelimiter));

            foreach (filter filter in parentFilters)
            {
                if (((filter.customfields == null) || (filter.customfields.SingleOrDefault<customfield>(c => (c.name == "VisibleFacet")).Value != "False")) || (filter.on == string.Format(WebConfiguration.Current.ProductGroupFormat, WebConfiguration.Current.PublicationId)))
                {
                    FacetSection section = new FacetSection {
                        on = filter.on,
                        Visible = false
                    };
                    if (filter.customfields != null)
                    {
                        section.Visible = filter.customfields.SingleOrDefault<customfield>(c => (c.name == "VisibleFacet")).Value == "True";
                    }
                    section.SectionTitle = filter.title;
                    section.CustomFields = filter.customfields;
                    section.Facets = new List<FacetItem>();
                    List<filtersection> filterSections = filter.filtersection.ToList<filtersection>();
                    foreach (var fs in filterSections)
                    {
                        FacetItem facetItem = getFacetItem(fs, fh_location, filter);
                        var child = childFilters.Where(s => s.on == fs.value.Value).FirstOrDefault();
                        if (child != null)
                        {
                            facetItem.childSection = getChildSectionRecursive(child, fh_location, childFilters);
                        }
                        section.Facets.Add(facetItem);
                    }
                    if(facets.FacetSections == null)
                    {
                        facets.FacetSections = new List<FacetSection>();
                    }
                    facets.FacetSections.Add(section);
                }
            }
            facets.ResetFacetsUrl = Resetfilters(fh_location, true, publicationId).toString();
            return facets;
        }

        public static filter GetSchemaCollection(this facetmap fm, string fh_location)
        {
            if (fm.filter == null)
            {
                return null;
            }
            return fm.filter.Where(f => f.on == "schematitle").FirstOrDefault();
        }

        public static bool IsActiveDiscussion(this Location loc)
        {
            return ((loc.getCriterion("commentcount") != null) && (loc.getCriterion("lastcommentdate") != null));
        }

        public static void ParseQuery(this Query query, string fh_params)
        {
            string[] strArray = fh_params.Split(new char[] { '&' });
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string str in strArray)
            {
                int index = str.IndexOf('=');
                if ((index > -1) && (index < str.Length))
                {
                    dictionary.Add(str.Substring(0, index), str.Substring(index + 1));
                }
            }
            if (dictionary.ContainsKey("fh_location"))
            {
                Location location = new Location(HttpUtility.UrlDecode(dictionary["fh_location"]));
                query.setLocation(location);
            }
            if (dictionary.ContainsKey("fh_start_index"))
            {
                int result = 0;
                if (int.TryParse(dictionary["fh_start_index"], out result))
                {
                    query.setListStartIndex(result);
                }
            }
            if (dictionary.ContainsKey("fh_view_size"))
            {
                int num3 = 0;
                if (int.TryParse(dictionary["fh_view_size"], out num3))
                {
                    num3 = FacetedContentHelper.AssertCappedViewSize(num3);
                    query.setListViewSize(num3);
                }
            }
            if (dictionary.ContainsKey("fh_view"))
            {
                ViewType viewType = ViewType.LISTER.ParseString(dictionary["fh_view"]);
                query.setView(viewType);
            }
            if (dictionary.ContainsKey("fh_sort_by"))
            {
                query.setSortingBy(dictionary["fh_sort_by"]);
            }
        }

        public static void ParseQuery(this Query query, IFieldSet fieldSet, int publicationId, int maxItems)
        {
            Location location = new Location(FredHopperInterface.GetPublicationPath(publicationId));
            query.setLocation(location);
            query.ParseSuperFacetQuery(fieldSet, maxItems, publicationId);
        }

        public static IList<FacetItem> ParseSelectedSchemaTypes(this Query query, IList<FacetItem> componentSchemaTypes)
        {
            Location location = query.getLocation();
            MultiValuedCriterion mvc = location.getCriterion("schematitle") as MultiValuedCriterion;
            if (mvc != null)
            {
                componentSchemaTypes.ToList<FacetItem>().ForEach(delegate (FacetItem t) {
                    t.Selected = mvc.getGreaterThan().contains(t.Value);
                });
            }
            return componentSchemaTypes;
        }

        public static ViewType ParseString(this ViewType vt, string viewType)
        {
            switch (viewType.ToUpper())
            {
                case "COMPARE":
                    return ViewType.COMPARE;

                case "DETAIL":
                    return ViewType.DETAIL;

                case "EMPTY":
                    return ViewType.EMPTY;

                case "HOME":
                    return ViewType.HOME;

                case "INDEX":
                    return ViewType.INDEX;

                case "KEEPLIST":
                    return ViewType.KEEPLIST;

                case "LISTER":
                    return ViewType.LISTER;

                case "REDIRECT":
                    return ViewType.REDIRECT;

                case "SEARCH":
                    return ViewType.SEARCH;

                case "SUMMARY":
                    return ViewType.SUMMARY;
            }
            return ViewType.LISTER;
        }

        public static void ParseSuperFacetQuery(this Query query, IFieldSet fieldSet, int maxItems, int publicationId)
        {
            maxItems = FacetedContentHelper.AssertCappedViewSize(maxItems);
            Location location = query.getLocation();
            ValueSet lowSet = new ValueSet(ValueSet.AggregationType.OR);
            foreach (KeyValuePair<string, IField> pair in fieldSet)
            {
                foreach (IKeyword keyword in pair.Value.Keywords)
                {
                    string str = PathToFacetPath(keyword.Path, publicationId);
                    lowSet.add(str);
                }
            }
            MultiValuedCriterion criterion = new MultiValuedCriterion(string.Format("{0}_super_facet", publicationId), lowSet, null, false);
            location.addCriterion(criterion);
            query.setListViewSize(maxItems);
        }

        private static string PathToFacetPath(string path, int publicationId)
        {
            string str = path;
            while (str.StartsWith(@"\"))
            {
                str = str.Remove(0, 1);
            }
            str = str.Replace(@"\", FacetedContentHelper.NestedLevelDelimiter).ToLower();
            return string.Format("{0}_{1}", publicationId, Regex.Replace(str.ToLower(), "[^a-z0-9_]", "_"));
        }

        public static void RemoveActiveDiscussion(this Location loc)
        {
            loc.removeCriteria("lastcommentdate");
            loc.removeCriteria("commentcount");
        }

        public static string RemoveFacetValues(string fh_location, string facet, string facetValue)
        {
            if (string.IsNullOrEmpty(facetValue))
            {
                return fh_location;
            }
            string input = string.Empty;
            string pattern = "/?" + facetValue + ";?(__[^}]*})?(>[^}]*})?";
            input = Regex.Replace(fh_location, pattern, "", RegexOptions.IgnoreCase);
            string str3 = "/" + facet + ">{}";
            return Regex.Replace(input, str3, "", RegexOptions.IgnoreCase);
        }

        private static Location Resetfilters(string fh_location, bool includeSearchTerm, int publicationId)
        {
            Location location = new Location(fh_location);
            Criterion criterion = location.getCriterion("publicationid");
            Criterion criterion2 = location.getCriterion("schematitle");
            Criterion criterion3 = location.getCriterion(string.Format("{0}_product_groups", publicationId));
            SearchCriterion criterion4 = location.getSearchCriterion();
            location.removeAllCriteria();
            if (criterion != null)
            {
                location.addCriterion(criterion);
            }
            if (criterion2 != null)
            {
                location.addCriterion(criterion2);
            }
            if (criterion3 != null)
            {
                location.addCriterion(criterion3);
            }
            if (includeSearchTerm && (criterion4 != null))
            {
                location.addCriterion(criterion4);
            }
            return location;
        }

        public static void SetActiveDiscussion(this Location loc)
        {
            loc.RemoveActiveDiscussion();
            loc.addCriterion(CriterionFactory.parse("commentcount>5"));
            string criterion = string.Format("lastcommentdate>{0}", DateTime.Now.Date.AddDays(-5.0).ToString("yyMMddHHmm"));
            loc.addCriterion(CriterionFactory.parse(criterion));
        }

        public static void setSortingBy(this Query query, string sortAttribute)
        {
            SortDirection aSC = SortDirection.ASC;
            query.removeSortingBy();
            string attributeName = HttpUtility.UrlDecode(sortAttribute).Trim();
            if (attributeName.StartsWith("-"))
            {
                attributeName = attributeName.Remove(0, 1);
                aSC = SortDirection.DESC;
            }
            else if (attributeName.StartsWith("+"))
            {
                attributeName = attributeName.Remove(0, 1);
            }
            query.setSortingBy(attributeName, aSC);
        }

        public static string ToFhParams(this Query query)
        {
            // enforce capped fh_view_size parameter
            int viewsize = query.getListViewSize();
            viewsize = FacetedContentHelper.AssertCappedViewSize(viewsize);
            query.setListViewSize(viewsize);

            string str = "?fh_params=" + HttpUtility.UrlEncode(query.toString());
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["fh_ticked"]))
            {
                string str2 = HttpContext.Current.Request["fh_ticked"];
                str = str + string.Format("&fh_ticked={0}", str2);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["pg"]))
            {
                string str3 = HttpContext.Current.Request["pg"];
                str = str + string.Format("&pg={0}", str3);
            }
            return str;
        }
    }
}

