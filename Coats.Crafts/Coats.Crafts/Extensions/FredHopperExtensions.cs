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

namespace Coats.Crafts.Extensions
{
    public static class FredHopperExtensions
    {
        public static void ParseQuery(this Query query, string fh_params)
        {
            // Example fh_params...
            // fh_location=//catalog01/en_US/trend<{vintage}&fh_start_index=6&fh_view_size=6&fh_view=lister
            
            // Split the param list on the ampersand
            //string [] paramList = fh_params.Replace("fh_params=", "").Split('&');
            string[] paramList = fh_params.Split('&');
            
            // Create a lookup dictionary from the string array
            //Dictionary<string, string> paramLookup = paramList.ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);
            Dictionary<string, string> paramLookup = new Dictionary<string, string>();
            foreach (var item in paramList)
            {
                int i = item.IndexOf('=');
                if (i > -1 && i < item.Length)
                {
                    paramLookup.Add(item.Substring(0, i), item.Substring(i+1));
                }
            }

            // Set the location first
            if(paramLookup.ContainsKey("fh_location"))
            {
                Location loc = new Location(HttpUtility.UrlDecode(paramLookup["fh_location"]));
                query.setLocation(loc);
            }

            // Next, set fh_start_index
            if (paramLookup.ContainsKey("fh_start_index"))
            {
                int startIdx = 0;
                if(int.TryParse(paramLookup["fh_start_index"], out startIdx))
                {
                    query.setListStartIndex(startIdx);
                }
            }

            // Next, set fh_view_size
            if (paramLookup.ContainsKey("fh_view_size"))
            {
                int viewSize = 0;
                if (int.TryParse(paramLookup["fh_view_size"], out viewSize))
                {
                    query.setListViewSize(viewSize);
                }
            }

            // Next, set the fh_view
            if (paramLookup.ContainsKey("fh_view"))
            {
                //ViewType vt = ViewType.parse(paramLookup["fh_view"].ToUpper());
                ViewType vt = ViewType.LISTER;
                vt = vt.ParseString(paramLookup["fh_view"]);
                query.setView(vt);
            }

            // Next, set the fh_sort_by filter
            if (paramLookup.ContainsKey("fh_sort_by"))
            {
                query.setSortingBy(paramLookup["fh_sort_by"]);
            }
        }

        public static void ParseQuery(this Query query, IFieldSet fieldSet, int publicationId, int maxItems)
        {
            Location loc = new Location(FredHopperInterface.GetPublicationPath(publicationId));
            query.setLocation(loc);
            //query.ParseQuery(fieldSet, maxItems);
            query.ParseSuperFacetQuery(fieldSet, maxItems, publicationId);
        }

        public static string ToFhParams(this Query query)
        {
            string url = "?fh_params=" + HttpUtility.UrlEncode(query.toString());
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["fh_ticked"]))
            {
                string ticked = HttpContext.Current.Request["fh_ticked"];
                url += String.Format("&fh_ticked={0}", ticked);
            }

            if (!String.IsNullOrEmpty(HttpContext.Current.Request["pg"]))
            {
                string param = HttpContext.Current.Request["pg"];
                url += String.Format("&pg={0}", param);
            }

            return url;
        }

        private static string PathToFacetPath(string path, int publicationId)
        {
            string facetPath = path;
            while(facetPath.StartsWith("\\"))
            {
                facetPath = facetPath.Remove(0, 1);
            }
            //return facetPath.Replace("\\", "__").Replace(' ', '_').ToLower();
            //return facetPath.Replace("\\", FacetedContentHelper.NestedLevelDelimiter).Replace(' ', '_').ToLower();

            // First, replace the slashes with the nested level delimiter (currently "__dir__")
            facetPath =  facetPath.Replace("\\", FacetedContentHelper.NestedLevelDelimiter).ToLower();

            // Now, replace all the illegal characters with underscores - AND prefix with the publication id
            return string.Format("{0}_{1}", publicationId, Regex.Replace(facetPath.ToLower(), "[^a-z0-9_]", "_"));
        }

        //public static void ParseQuery(this Query query, IFieldSet fieldSet, int maxItems)
        //{
        //    // Get field value => create query
        //    // /Brands/Camila
        //    //brand>{brands__dir__camila}

        //    Location loc = query.getLocation();
        //    foreach (var field in fieldSet)
        //    {
        //        // Add this criterion to the criteria list
        //        foreach (IKeyword keyword in field.Value.Keywords)
        //        {
        //            //// Check if we already have a criteria list for this key
        //            //string criteriaKey = field.Value.CategoryName.ToLower().Replace(' ', '_');
        //            //var criterion = CriterionFactory.parse(string.Format("{0}>{{{1}}}", criteriaKey, PathToFacetPath(keyword.Path)));
        //            //loc.addCriterion(criterion);

        //            // The above code used to work for facets of one level.
        //            // e.g. "techniques\crochet" => "techniques>{techniques__dir__crochet}"
        //            // However, if the content is tagged with a nested level, e.g. "techniques\crochet\granny squares",
        //            // then having a query of "techniques>{techniques__dir__crochet__dir__granny_squares}" will not retrieve
        //            // anything because the query should be "techniques__dir__crochet>{techniques__dir__crochet__dir__granny_squares}"
        //            // The simple answer is to parse the the keyword path at the last ocurance of "__dir__".
        //            string path = PathToFacetPath(keyword.Path);
        //            int i = path.LastIndexOf("__dir__");
        //            // Quick check we're not screwing any legacy code up (no "__dir__" found)
        //            if (i < 0)
        //            {
        //                //string criteriaKey = field.Value.CategoryName.ToLower().Replace(' ', '_');
        //                string criteriaKey = PathToFacetPath(field.Value.CategoryName); // Use PathToFacetPath for consistancy
        //                var criterion = CriterionFactory.parse(string.Format("{0}>{{{1}}}", criteriaKey, PathToFacetPath(keyword.Path)));
        //                loc.addCriterion(criterion);
        //            }
        //            else
        //            {
        //                var criterion = CriterionFactory.parse(string.Format("{0}>{{{1}}}", path.Substring(0, i), path));
        //                loc.addCriterion(criterion);
        //            }


        //        }
        //    }
        //    query.setLocation(new Location(CreateOrCondition(loc.toString())));
        //    query.setListViewSize(maxItems);
        //}

        public static void ParseSuperFacetQuery(this Query query, IFieldSet fieldSet, int maxItems, int publicationId)
        {
            // Create a query specifically for the new "super facet", which contains all facets combined into one big...
            // err... super facet!

            Location loc = query.getLocation();

            ValueSet values = new ValueSet(ValueSet.AggregationType.OR);

            foreach (var field in fieldSet)
            {
                // Add this criterion to the criteria list
                foreach (IKeyword keyword in field.Value.Keywords)
                {
                    string path = PathToFacetPath(keyword.Path, publicationId);
                    values.add(path);
                }
            }
            MultiValuedCriterion superFacet = new MultiValuedCriterion(string.Format("{0}_super_facet", publicationId), values, null, false);
            loc.addCriterion(superFacet);
            query.setListViewSize(maxItems);
        }

        public static void setSortingBy(this Query query, string sortAttribute)
        {
            SortDirection direction = SortDirection.ASC;
            query.removeSortingBy();

            string attribute = HttpUtility.UrlDecode(sortAttribute).Trim();

            if (attribute.StartsWith("-")) // Minus means DESCending
            {
                attribute = attribute.Remove(0, 1);
                direction = SortDirection.DESC;
            }
            else if (attribute.StartsWith("+"))
            {
                attribute = attribute.Remove(0, 1);
            }
            query.setSortingBy(attribute, direction); // OK, so this is obsolete: any chance of an alternative???
        }

        public static IList<FacetItem> ParseSelectedSchemaTypes(this Query query, IList<FacetItem> componentSchemaTypes)
        {
            Location loc = query.getLocation();
            // Updated as part of CCPLCR-1
            //string criterion = loc.getCriteria("schematitle").ToString();
            // Will look something like "schematitle>{crafts2earticle;craft2eproject}"

            //foreach (var comp in componentSchemaTypes)
            //{
            //    comp.Selected = criterion.Contains(comp.Value);
            //}
            MultiValuedCriterion mvc = loc.getCriterion("schematitle") as MultiValuedCriterion;
            if (mvc == null)
                return componentSchemaTypes;

            componentSchemaTypes.ToList().ForEach(t => {
                t.Selected = mvc.getGreaterThan().contains(t.Value);
            });

            return componentSchemaTypes;
        }

        public static bool IsActiveDiscussion(this Location loc)
        {
            return ((loc.getCriterion("commentcount") != null) && (loc.getCriterion("lastcommentdate") != null));
        }

        public static void SetActiveDiscussion(this Location loc)
        {
            //loc.removeCriteria("lastcommentdate");
            //loc.removeCriteria("commentcount");
            loc.RemoveActiveDiscussion();
            loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse("commentcount>5")); // N.B. "...>5" actually mean 5 or more!!!
            DateTime dt = DateTime.Now.AddDays(-5);
            string adLoc = string.Format("lastcommentdate>{0}", dt.ToString("yyMMddHHmm"));
            loc.addCriterion(com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(adLoc));
        }

        public static void RemoveActiveDiscussion(this Location loc)
        {
            loc.removeCriteria("lastcommentdate");
            loc.removeCriteria("commentcount");
        }

        public static ViewType ParseString(this ViewType vt, string viewType)
        {
            // Had to write this because the supplied "parse" method doesn't work!
            // I tried to be clever by using reflection, because this is actually a class, NOT an enum -
            // but it is actually a ViewType class of nested static ViewType classes. Hmmm... genius!
            // Reflection was a no go, so reverted to a good old fasioned case statement.
            ViewType type = ViewType.LISTER;

            switch(viewType.ToUpper())
            {
                case "COMPARE" : 
                    type = ViewType.COMPARE;
                    break;
                case "DETAIL" :
                    type = ViewType.DETAIL;
                    break;
                case "EMPTY" :
                    type = ViewType.EMPTY;
                    break;
                case "HOME" :
                    type = ViewType.HOME;
                    break;
                case "INDEX" :
                    type = ViewType.INDEX;
                    break;
                case "KEEPLIST" :
                    type = ViewType.KEEPLIST;
                    break;
                case "LISTER" :
                    type = ViewType.LISTER;
                    break;
                case "REDIRECT" :
                    type = ViewType.REDIRECT;
                    break;
                case "SEARCH" :
                    type = ViewType.SEARCH;
                    break;
                case "SUMMARY" :
                    type = ViewType.SUMMARY;
                    break;
                default:
                    type = ViewType.LISTER;
                    break;
            }

            return type;
        }

        public static string RemoveFacetValues(string fh_location, string facet, string facetValue)
        {
            if (String.IsNullOrEmpty(facetValue))
                return fh_location;

            string linkUrl = string.Empty;

            // NG - Hold tight!
            // The previous code simply text-removed the select crtieria e.g. "themes__dir__people" to leave a URL
            // we can user to untick a selecte item. The problem is this now buggered up nested items which carry their
            // parent e.g. "themes__dir__people__dir__women__pp_" would get malformed into "__dir__women__pp_".
            // This regex is an attempt to correctly remove the parent and any child.                  
            string removeFacetValues = "/?" + facetValue + ";?(__[^}]*})?(>[^}]*})?";
            linkUrl = Regex.Replace(fh_location, removeFacetValues, "", RegexOptions.IgnoreCase);

            // The above maye leave use with an empty attribute - e.g  70_brand>{} - so remove that as well
            string removeEmptyFacet = "/" + facet + ">{}";
            linkUrl = Regex.Replace(linkUrl, removeEmptyFacet, "", RegexOptions.IgnoreCase);

            return linkUrl;
        }

        public static FacetItem getFacetItem(filtersection fs, string fh_location, filter f)
        {
            FacetItem facetItem = new FacetItem();

            // Store the actual FredHopper Id (e.g. "techniques__dir__crochet")
            facetItem.FHId = fs.value.Value;

            // Get the number of items in the facet
            facetItem.NumItems = fs.nr;

            // Is this facet selected?
            facetItem.Selected = fs.selected;

            // Get the Query parameters that have to be queried if the user clicks on the facet
            FASWebService.link l = fs.link;

            // Facet text
            facetItem.Text = l.name;

            // Get the current facet location
            //string curFacetLoc = HttpUtility.UrlDecode(l.urlparams).Split('&').SingleOrDefault(p => p.StartsWith("fh_location=", StringComparison.InvariantCultureIgnoreCase));
            Query linkQuery = new Query();
            linkQuery.ParseQuery(l.urlparams);

            // Potentially a new page, so reset the start pagination index
            linkQuery.removeListStartIndex();

            string linkUrl = string.Empty;

            // Selected?
            if (fs.selected)
            {
                // Build "remove" path.
                // Basically, get the current location string from query, and remove the relevant
                // facet from the path - e.g.:
                // fh_location=//catalog01/en_US/technique>{crochet}/season>{autumn;winter}/skill_level>{beginner}
                // and the current facet is "autumn", then remove the "autumn" from "season>{autumn;winter}" to get
                // fh_location=//catalog01/en_US/technique>{crochet}/season>{winter}/skill_level>{beginner}
                // A further complication is to make sure a whole facet is removed if there is only one item
                // within the curly braces. e.g.
                // fh_location=//catalog01/en_US/technique>{crochet}/season>{winter}/skill_level>{beginner}
                // and the current facet is "winter", then remove the whole "/season>{winter}" to get
                // fh_location=//catalog01/en_US/technique>{crochet}/skill_level>{beginner}

                // fh_location will contain a list something like "skill_level>{beginner}" or "skill_level>{beginner;intermediate}"
                // The last criterion will be a single value, e.g. "skill_level>{beginner}"
                //string lastCriterion = criterionLoc.getLastCriterion().toString();

                string lastCriterion = linkQuery.getLocation().getLastCriterion().toString();


               // if (fh_location.Contains(lastCriterion))
               // {
               //     linkUrl = fh_location.Replace("/" + lastCriterion, "");
               // }
               // else
                //{
                    // Failed the single facet test, so fh_location contains a list of "OR" items.
                    // fh_location will contain a list something like "{summer;autumn;winter}"
                    // Extract the facet value encased in the curly braces
                    string facetValue = lastCriterion.Substring(lastCriterion.IndexOf('{') + 1).Replace("}", "");
                    linkUrl = FredHopperExtensions.RemoveFacetValues(
                                    fh_location,
                                    linkQuery.getLocation().getLastCriterion().getAttributeName(),
                                    facetValue);

                    //linkUrl = linkUrl.Replace(facetValue, "").Replace(";;", ";").Replace("{;", "{").Replace(";}", "}");

                    // Remove the facet from the location, making sure we tidy up any occurances of ";;" or "{;" or ";}"
                    //linkUrl = fh_location.Replace(facetValue, "").Replace(";;", ";").Replace("{;", "{").Replace(";}", "}");
                //}
            }
            else
            {
                // Get the current query location (copy, otherwise you stuff everything up if you use the query.getLocation()!!!)
                //com.fredhopper.lang.query.location.Location queryLoc = new com.fredhopper.lang.query.location.Location(query.getLocation());
                com.fredhopper.lang.query.location.Location queryLoc = new com.fredhopper.lang.query.location.Location(fh_location);

                // Add the facet criterion to the end of the current query location
                // The last criterion should be the current facet criterion
                //queryLoc.addCriterion(criterionLoc.getLastCriterion());
                queryLoc.addCriterion(linkQuery.getLocation().getLastCriterion());

                linkUrl = queryLoc.toString();

                // CombineFacets?
                if (f.customfields != null && f.customfields.SingleOrDefault(c => c.name == "RefineMethod").Value == "CombineFacets")
                {
                    linkUrl = CreateOrCondition(linkUrl);
                }
            }

            com.fredhopper.lang.query.location.Location criterionLoc = new com.fredhopper.lang.query.location.Location(linkUrl);
            linkQuery.setLocation(criterionLoc);

            //facetItem.Value = "?fh_params=" + HttpUtility.UrlEncode(linkQuery.toString());
            facetItem.Value = linkQuery.ToFhParams();
            return facetItem;
        }

        public static FASWebService.filter GetSchemaCollection(this facetmap fm, string fh_location)
        {
            if (fm.filter == null)
                return null;

            return fm.filter.Where(f => f.on == "schematitle").FirstOrDefault();
        }

        public static FacetCollection GetHierarchicalFacetCollection(this facetmap fm, string fh_location, int publicationId)
        {
            FacetCollection facetCollection = new FacetCollection();

            if (fm.filter == null)
            {
                // Just return an empty collection for now.
                facetCollection.FacetSections = new List<FacetSection>();

                Location reset = Resetfilters(fh_location, false, publicationId);
                //facetCollection.ResetFacetsUrl = reset.ToFhParams() + "&resetFilters=true";
                facetCollection.ResetFacetsUrl = reset.toString(); //reset.ToFhParams();

                return facetCollection;
            }

            // Get the first of each unique facet heading (e.g. "technique", "skill_level", etc)
            // to get the first "Lat Stay"
            List<FASWebService.filter> filters = fm.filter.ToList();
            var uniqueFilters = filters.GroupBy(f => f.title).Select(grp => grp.First());

            //IEnumerable<FASWebService.filter> childFilters = uniqueFilters.Where(f => f.on.Contains("__"));
            //IEnumerable<FASWebService.filter> parentFilters = uniqueFilters.Where(f => !f.on.Contains("__"));
            IEnumerable<FASWebService.filter> childFilters = uniqueFilters.Where(f => f.on.Contains(FacetedContentHelper.NestedLevelDelimiter));
            
            IEnumerable<FASWebService.filter> parentFilters = uniqueFilters.Where(f => !f.on.Contains(FacetedContentHelper.NestedLevelDelimiter));

            foreach (var f in parentFilters)
            {
                // Do we need to show this facet group???
                if (f.customfields != null 
                    && f.customfields.SingleOrDefault(c => c.name == "VisibleFacet").Value == "False"
                    && f.on != String.Format(
                                WebConfiguration.Current.ProductGroupFormat, 
                                WebConfiguration.Current.PublicationId))
                {
                    continue;
                }

                FacetSection facetSection = new FacetSection();
                facetSection.on = f.on;
    
                // Set visibility
                facetSection.Visible = false;
                if (f.customfields != null)
                    facetSection.Visible = f.customfields.SingleOrDefault(c => c.name == "VisibleFacet").Value == "True";

                // Set section title
                facetSection.SectionTitle = f.title;
                // Set custom fields
                facetSection.CustomFields = f.customfields;

                facetSection.Facets = new List<FacetItem>();

                #region New "OR" Location

                // Iterator over facet values
                List<FASWebService.filtersection> filterSections = f.filtersection.ToList();
                foreach (var fs in filterSections)
                {
                    FacetItem facetItem = getFacetItem(fs, fh_location, f);
                    var child = childFilters.Where(s => s.on == fs.value.Value).FirstOrDefault();

                    if (child != null)
                    {
                        facetItem.childSection = getChildSectionRecursive(child, fh_location, childFilters);
                    }

                    facetSection.Facets.Add(facetItem);
                }

                #endregion

                if (facetCollection.FacetSections == null)
                {
                    facetCollection.FacetSections = new List<FacetSection>();
                }

                facetCollection.FacetSections.Add(facetSection);
            }

            Location resetFacets = Resetfilters(fh_location, true, publicationId);
            // Set the Url
            //facetCollection.ResetFacetsUrl = resetFacets.ToFhParams() + "&resetFilters=true";

            // Change to toString so that I can manipulate the url upon the return 
            facetCollection.ResetFacetsUrl = resetFacets.toString(); //resetFacets.ToFhParams();

            return facetCollection;

        }


        private static Location Resetfilters(string fh_location, bool includeSearchTerm, int publicationId)
        {
            // Finally, create the "Reset Filters" url. This basically consists of stripping the current fh_location param of all
            // criteria EXCEPT for the schema title and publication id.
            // [edit] Also, product groups should be re-instated for Product Explorer
            Location loc = new Location(fh_location);
            // Store the publication id and schema title selections
            var critPub = loc.getCriterion("publicationid");
            var critSchema = loc.getCriterion("schematitle");
            var prodGroups = loc.getCriterion(string.Format("{0}_product_groups", publicationId));
            // Store the search criteria
            var critSearch = loc.getSearchCriterion();
            // Remove the whole sha-boodle!
            loc.removeAllCriteria();
            // Return the publication id, schema title, and product group selections
            if (critPub != null) { loc.addCriterion(critPub); }
            if (critSchema != null) { loc.addCriterion(critSchema); }
            if (prodGroups != null) { loc.addCriterion(prodGroups); }
            // Return the search term (if exists)
            if (includeSearchTerm)
                if (critSearch != null) { loc.addCriterion(critSearch); }
            // Create query using modified location
            return loc; // new Query(loc);
        }

        public static FacetSection getChildSectionRecursive(FASWebService.filter child, string fh_location, IEnumerable<FASWebService.filter> childFilters)
        {
            FacetSection childSection = new FacetSection();

            childSection.Facets = new List<FacetItem>();
            childSection.SectionTitle = child.title;
            childSection.CustomFields = child.customfields;

            List<FASWebService.filtersection> childFilterSections = child.filtersection.ToList();

            // ASH: Wasn't quite nesting properly for nested levels greater than two.
            //      Simplified and re-arranged the logic slightly...
            //foreach (var cfs in childFilterSections)
            //{
            //    FacetItem childFacetItem = getFacetItem(cfs, fh_location, child);
            //    childFacetItem.Selected = cfs.selected;
            //    childSection.Facets.Add(childFacetItem);
            //}

            //foreach (var cfs in childFilterSections)
            //{
            //    if (childSection.Facets != null)
            //    {
            //        foreach (var facet in childSection.Facets)
            //        {
            //            var rChild = childFilters.Where(s => s.on == facet.Value).FirstOrDefault();
            //            if (rChild != null)
            //            {
            //                 facet.childSection = getChildSectionRecursive(rChild, fh_location, childFilters);
            //            }
            //        }
            //    }
            //}

            foreach (var cfs in childFilterSections)
            {
                FacetItem childFacetItem = getFacetItem(cfs, fh_location, child);

                if (childFacetItem != null)
                {
                    childFacetItem.Selected = cfs.selected;
                    // ASH: This was the catch with the previous logic:
                    // s.on would be something like "brand__dir__anchor__dir__anchor_artiste" but facet.Value
                    // would contain an actual FH query URL, so the match would fail.
                    // var rChild = childFilters.Where(s => s.on == facet.Value).FirstOrDefault();
                    // Instead, compare on cfs.value.Value
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

        //public static FacetCollection GetFacetCollection(this facetmap fm, string fh_location, int publicationId)
        //{
        //    // Iterator over facets
        //    FacetCollection facetCollection = new FacetCollection();

        //    if (fm.filter == null)
        //    {
        //        // Just return an empty collection for now.
        //        facetCollection.FacetSections = new List<FacetSection>();
        //        return facetCollection;
        //    }

        //    //string fh_location = query.getLocation().toString();

        //    // Get the first of each unique facet heading (e.g. "technique", "skill_level", etc)
        //    // to get the first "Lat Stay"
        //    List<FASWebService.filter> filters = fm.filter.ToList();
        //    var uniqueFilters = filters.GroupBy(f => f.title).Select(grp => grp.First());

        //    foreach (var f in uniqueFilters)
        //    {
        //        // Do we need to show this facet group???
        //        if (f.customfields != null && f.customfields.SingleOrDefault(c => c.name == "VisibleFacet").Value == "False")
        //        {
        //            continue;
        //        }
                
        //        FacetSection facetSection = new FacetSection();

        //        // Set section title
        //        facetSection.SectionTitle = f.title;
        //        // Set custom fields
        //        facetSection.CustomFields = f.customfields;

        //        facetSection.Facets = new List<FacetItem>();

        //        #region New "OR" Location

        //        // Iterator over facet values
        //        List<FASWebService.filtersection> filterSections = f.filtersection.ToList();
        //        foreach (var fs in filterSections)
        //        {
        //            FacetItem facetItem = new FacetItem();

        //            // Get the number of items in the facet
        //            facetItem.NumItems = fs.nr;

        //            // Is this facet selected?
        //            facetItem.Selected = fs.selected;

        //            // Get the Query parameters that have to be queried if the user clicks on the facet
        //            FASWebService.link l = fs.link;

        //            // Facet text
        //            facetItem.Text = l.name;

        //            // Get the current facet location
        //            //string curFacetLoc = HttpUtility.UrlDecode(l.urlparams).Split('&').SingleOrDefault(p => p.StartsWith("fh_location=", StringComparison.InvariantCultureIgnoreCase));
        //            Query linkQuery = new Query();
        //            linkQuery.ParseQuery(l.urlparams);

        //            // Potentially a new page, so reset the start pagination index
        //            linkQuery.removeListStartIndex();

        //            string linkUrl = string.Empty;

        //            // Selected?
        //            if (fs.selected)
        //            {
        //                // Build "remove" path.
        //                // Basically, get the current location string from query, and remove the relevant
        //                // facet from the path - e.g.:
        //                // fh_location=//catalog01/en_US/technique>{crochet}/season>{autumn;winter}/skill_level>{beginner}
        //                // and the current facet is "autumn", then remove the "autumn" from "season>{autumn;winter}" to get
        //                // fh_location=//catalog01/en_US/technique>{crochet}/season>{winter}/skill_level>{beginner}
        //                // A further complication is to make sure a whole facet is removed if there is only one item
        //                // within the curly braces. e.g.
        //                // fh_location=//catalog01/en_US/technique>{crochet}/season>{winter}/skill_level>{beginner}
        //                // and the current facet is "winter", then remove the whole "/season>{winter}" to get
        //                // fh_location=//catalog01/en_US/technique>{crochet}/skill_level>{beginner}

        //                // fh_location will contain a list something like "skill_level>{beginner}" or "skill_level>{beginner;intermediate}"
        //                // The last criterion will be a single value, e.g. "skill_level>{beginner}"
        //                //string lastCriterion = criterionLoc.getLastCriterion().toString();
        //                string lastCriterion = linkQuery.getLocation().getLastCriterion().toString();

        //                if (fh_location.Contains(lastCriterion))
        //                {
        //                    linkUrl = fh_location.Replace("/" + lastCriterion, "");
        //                }
        //                else
        //                {
        //                    // Failed the single facet test, so fh_location contains a list of "OR" items.
        //                    // fh_location will contain a list something like "{summer;autumn;winter}"
        //                    // Extract the facet value encased in the curly braces
        //                    string facetValue = lastCriterion.Substring(lastCriterion.IndexOf('{') + 1).Replace("}", "");

        //                    // Remove the facet from the location, making sure we tidy up any occurances of ";;" or "{;" or ";}"
        //                    linkUrl = fh_location.Replace(facetValue, "").Replace(";;", ";").Replace("{;", "{").Replace(";}", "}");
        //                }
        //            }
        //            else
        //            {
        //                // Get the current query location (copy, otherwise you stuff everything up if you use the query.getLocation()!!!)
        //                //com.fredhopper.lang.query.location.Location queryLoc = new com.fredhopper.lang.query.location.Location(query.getLocation());
        //                com.fredhopper.lang.query.location.Location queryLoc = new com.fredhopper.lang.query.location.Location(fh_location);

        //                // Add the facet criterion to the end of the current query location
        //                // The last criterion should be the current facet criterion
        //                //queryLoc.addCriterion(criterionLoc.getLastCriterion());
        //                queryLoc.addCriterion(linkQuery.getLocation().getLastCriterion());

        //                linkUrl = queryLoc.toString();

        //                // CombineFacets?
        //                if (f.customfields != null && f.customfields.SingleOrDefault(c => c.name == "RefineMethod").Value == "CombineFacets")
        //                {
        //                    linkUrl = CreateOrCondition(linkUrl);
        //                }
        //            }

        //            com.fredhopper.lang.query.location.Location criterionLoc = new com.fredhopper.lang.query.location.Location(linkUrl);
        //            linkQuery.setLocation(criterionLoc);
        //            facetItem.Value = "?fh_params=" + HttpUtility.UrlEncode(linkQuery.toString());
        //            facetSection.Facets.Add(facetItem);
        //        }

        //        #endregion

        //        if (facetCollection.FacetSections == null)
        //        {
        //            facetCollection.FacetSections = new List<FacetSection>();
        //        }

        //        facetCollection.FacetSections.Add(facetSection);
        //    }

        //    // Finally, create the "Reset Filters" url. This basically consists of stripping the current fh_location param of all
        //    // criteria EXCEPT for the schema title and publication id.
        //    com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(fh_location);
        //    // Store the publication id and schema title selections

        //    // *** ASH - Should we not be calling the method "Resetfilters" ??? ***
        //    //var critPub = loc.getCriterion("publicationid");
        //    //var critSchema = loc.getCriterion("schematitle");

        //    //// Store the search criteria
        //    //var critSearch = loc.getSearchCriterion();
        //    //// Remove the whole sha-boodle!
        //    //loc.removeAllCriteria();
        //    //// Return the publication id and schema title selections
        //    //if (critPub != null) { loc.addCriterion(critPub); }
        //    //if (critSchema != null) { loc.addCriterion(critSchema); }
        //    //// Return the search term (if exists)
        //    //if (critSearch != null) { loc.addCriterion(critSearch); }
        //    //// Create query using modified location
        //    //Query resetFacets = new Query(loc);

        //    Location resetFacets = Resetfilters(fh_location, true, publicationId);

        //    // Set the Url
        //    //facetCollection.ResetFacetsUrl = resetFacets.ToFhParams() + "&resetFilters=true";
        //    facetCollection.ResetFacetsUrl = resetFacets.toString(); //resetFacets.ToFhParams();

        //    return facetCollection;

        //}

        public static List<FacetPagination> GetFacetPagination(this results r, Query query, int itemsPerPage)
        {
            int totalItems = r.total_items;

            List<FacetPagination> pagination = new List<FacetPagination>();

            int numPaginationLinks = totalItems / itemsPerPage;
            numPaginationLinks += (totalItems % itemsPerPage == 0) ? 0 : 1;
            int listStartIndex = query.getListStartIndex();
            for (int i = 0; i < numPaginationLinks; i++)
            {
                int startIndex = (i * itemsPerPage);
                FacetPagination paginationItem = new FacetPagination();
                paginationItem.LinkText = (i + 1).ToString();
                paginationItem.Selected = (listStartIndex == startIndex);

                string url = query.setListStartIndex(i * itemsPerPage).toString();
                paginationItem.Url = "?fh_params=" + HttpUtility.UrlEncode(url);

                if (!String.IsNullOrEmpty(HttpContext.Current.Request["fh_ticked"]))
                    paginationItem.Url += "&fh_ticked=" + HttpContext.Current.Request["fh_ticked"];

                pagination.Add(paginationItem);
            }

            query.setListStartIndex(listStartIndex);

            return pagination;
        }

        private static string CreateOrCondition(string urlParams)
        {
            // Get the location string from urlParams
            string fh_location = HttpUtility.UrlDecode(urlParams).Split('&').SingleOrDefault(p => p.StartsWith("fh_location=", StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(fh_location))
            {
                fh_location = urlParams;
            }

            // Load the location. This gives us loads of nice utility methods and properties!
            //com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(fh_location.Split('=')[1]);
            // FLAWED LOGIC if used in conjunction with a search!!!
            // fh_location=//catalog01/en_US/$s=Three&fh_view=lister&fh_view_size=20
            // Splitting the above on "=" will give "fh_location", "//catalog01/en_US/$s", "Three", which is an invalide criterion (search string split)
            com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(fh_location.Replace("fh_location=", ""));

            // -------------------------------------------------------------------------------------------------
            // Now the manipulation bit...
            // -------------------------------------------------------------------------------------------------
            // OK, so what are we trying to do here? [...At times, I wish I knew...]
            // -------------------------------------------------------------------------------------------------
            // FredHopper is designed to output facet URLs as an "AND" condition.
            // e.g. fh_location=//catalog01/en_US/technique>{crochet}/season>{autumn}/technique>{knitting}
            // FredHopper will to refine on "technique = crochet" AND "technique = knitting",
            // then refine the already narrowed result set on "season = autumn"
            // -------------------------------------------------------------------------------------------------
            // So what do we need to do to output facet URLs as "an OR" condition?
            // -------------------------------------------------------------------------------------------------
            // Taking the above e.g. fh_location=//catalog01/en_US/technique>{crochet}/season>{autumn}/technique>{knitting}
            // First, we need to find out if the last criterion's attribute ("techinque") already exists in the location.
            // If it doesn't, then simple leave as is.
            // However, in our example, we have 2 "technique" criterions ("technique>{crochet}" and "technique>{knitting}").
            // To create an "OR" list, we need to merge these two into a single, semi-colon delimited criterion
            // e.g. "technique>{crochet;knitting}".
            // Then we simply remove the last criterion, so we get the following complete location string:
            // fh_location=//catalog01/en_US/technique>{crochet;knitting}/season>{autumn}
            // -------------------------------------------------------------------------------------------------
            // Im glad that's all clear then...
            // -------------------------------------------------------------------------------------------------

            // Special case!!! Search criteria : fh_location=//catalog01/en_US/technique>{crochet;knitting}/$s=Three/season>{autumn}
            // We need to remove and store the search criteria                                             /--------/
            // Then re-apply it later.
            com.fredhopper.lang.query.location.criteria.SearchCriterion search = loc.getSearchCriterion();
            if (search != null)
            {
                loc.removeSearchCriteria();
            }

            // Now, get the last criterion. FredHopper appends the new facet on the end.
            com.fredhopper.lang.query.location.criteria.Criterion lastCriterion = loc.getLastCriterion();

            // Check if the location already has an attribute of the same type
            com.fredhopper.lang.query.location.criteria.Criterion existingCriterion = loc.getCriterion(lastCriterion.getAttributeName());

            // If the location DOESN'T have an existing attribute of the same name, then just return what was passed in
            if (existingCriterion == lastCriterion)
            {
                // Finally, add the stored SearchCriterion (if there was one)
                if (search != null)
                {
                    loc.addCriterion(search);
                    //return HttpUtility.UrlEncode("fh_location=" + loc.toString());
                    return loc.toString();
                }
                return urlParams;
            }

            // -------------------------------------------------------------------------------------------------
            // Got to here, so we need to do some further manipulation
            // -------------------------------------------------------------------------------------------------

            // Get the criterion name (wrapped in curly braces)
            string src = lastCriterion.toString();
            src = src.Substring(src.IndexOf('{') + 1);
            src = src.Replace("}", "");

            // Remove the last criterion (needs to be merged with existing criterion)
            loc = loc.removeLastCriterion();

            string trg = existingCriterion.toString().Replace("}", (";" + src + "}"));

            // Remove the exisinting criterion (needs to be merged with existing criterion)
            loc = loc.removeCriteria(existingCriterion.getAttributeName());

            // Add the newly created criterion
            var merged = com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(trg);
            loc.addCriterion(merged);

            // Finally, add the stored SearchCriterion (if there was one)
            if (search != null)
            {
                loc.addCriterion(search);
            }

            //return HttpUtility.UrlEncode("fh_location=" + loc.toString());
            return loc.toString();
        }

    }
}