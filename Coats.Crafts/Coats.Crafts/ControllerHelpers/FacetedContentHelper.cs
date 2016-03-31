using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

using DD4T.ContentModel;

using Coats.Crafts.Models;
using Coats.Crafts.Configuration;
using Coats.Crafts.Extensions;
using Coats.Crafts.Resources;

using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location.criteria;

using Castle.Core.Logging;
using Castle.Windsor;

namespace Coats.Crafts.ControllerHelpers
{
    public sealed class FacetedContentHelper
    {
        public static ILogger Logger
        {
            get
            {
                var accessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                var logger = accessor.Container.Resolve<ILogger>();
                return logger;
            }
        }

        public static string NestedLevelDelimiter
        {
            get 
            {
                // Letting it throw an exception for now - after all, we do need it in the config.
                return WebConfiguration.Current.NestedLevelDelimiter;//ConfigurationManager.AppSettings["NestedLevelDelimiter"];
            }
        }

        public static bool IsViewFromProductExplorerSection(string view)
        {
            bool result = false;

            if (view.ToLower() == "productexplorer" || view.ToLower() == "ourbrands" || view.ToLower() == "storelocator")
            {
                result = true;
            }

            return result;
        }

        public static T InjectIntroComponent<T>(T facetedContent, IField introMarkup, bool historyBack) where T : FacetedContentBase
        {
            try
            {
                // First page check
                //if (facetedContent.ComponentList.Pagination[0].Selected)
                if (historyBack || (facetedContent.ComponentList.Pagination.Count == 0) || (facetedContent.ComponentList.Pagination.Count > 0 && facetedContent.ComponentList.Pagination[0].Selected))
                {
                    // Attempt to merge the intro text from the page
                    DD4T.ContentModel.Component intro = new Component();
                    //var introMarkup = componentPresentation.Component.Fields["titleText"].EmbeddedValues[0]["text"];
                    intro.Fields.Add("intro", introMarkup);
                    intro.Schema.Title = WebConfiguration.Current.ContentTypeIntroduction;
                    intro.Title = "Intro";
                    facetedContent.ComponentList.Components.Insert(0, intro);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("InjectIntroComponent<T>(T facetedContent, IField introMarkup, bool historyBack): {0}", ex);
            }


            return facetedContent;
        }

        //public static T InjectCTAComponents<T>(T facetedContent, IList<IComponentPresentation> ctaList) where T : FacetedContentBase
        //{
        //    return InjectCTAComponents(facetedContent, ctaList, false);
        //}

        public static T InjectCTAComponents<T>(T facetedContent, IList<IComponentPresentation> ctaList, bool randomInsert = false, bool historyBack = false) where T : FacetedContentBase
        {
            // Null checks
            if (facetedContent.ComponentList != null && facetedContent.ComponentList.Pagination != null)
            {
                // First page check (or history back check)
                if ((historyBack) || (facetedContent.ComponentList.Pagination.Count == 0) || (facetedContent.ComponentList.Pagination.Count > 0 && facetedContent.ComponentList.Pagination[0].Selected))
                {
                    facetedContent.ComponentList.Components = InjectCTAComponents(facetedContent.ComponentList.Components, ctaList, randomInsert, historyBack);
                }
            }

            return facetedContent;
        }

        private static object syncRoot = new object();

        public static List<Component> InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack)
        {
            Logger.Info("InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack)");

            Random rnd = new Random();
            int min = Math.Min(4, (historyBack ? 9 : components.Count-1));
            //int max = Math.Min(components.Count, 9);
            //int max = components.Count - 1;
            int max = Math.Min(components.Count - 1, (historyBack ? 9 : components.Count - 1));

            if (min >= max)
            {
                randomInsert = false;
            }

            Logger.InfoFormat("randomInsert: {0}", randomInsert);
            Logger.InfoFormat("ctaList.Count: {0}", ctaList.Count);

            // Get single instance of config
            CtaTemplateIdConfig config = CtaTemplateIdConfig.Instance;
            Logger.InfoFormat("config: {0}", config.GetHashCode());

            foreach (var cta in ctaList)
            {
                try
                {
                    // Web.config lookup
                    string lookup = UtilityHelper.GetTcmUri(cta.ComponentTemplate.Id).TcmItemId.ToString();

                    var checkTemplate = config.Templates.ContainsKey(lookup);
                    Logger.InfoFormat("checkTemplate: {0}", checkTemplate);

                    //if (CtaTemplateIdConfig.List().ContainsKey(lookup) && cta.ComponentTemplate.MetadataFields.ContainsKey("view"))
                    //if (checkTemplate && cta.ComponentTemplate.MetadataFields.ContainsKey("view"))

                    // NG - Changing this so that we don't modify the component fields dictionary.
                    // Metadata already carries a reference to the view, so there seems little point in adding the field
                    if (checkTemplate)
                    {
                        // Don't we just need...?
                        //if (cta.Component.Fields.ContainsKey("view"))
                        //{
                        //    cta.Component.Fields.Remove("view");
                        //}

                        //// Lock access to ensure only this thread updates the dictionary
                        //Logger.Info("Lock around update to component");
                        //lock (syncRoot)
                        //{
                        //    cta.Component.Fields.Add("view", cta.ComponentTemplate.MetadataFields["view"]);
                        //}           
                        Logger.InfoFormat("use_view: {0}", cta.ComponentTemplate.MetadataFields["view"].Value);
                        cta.Component.Url = String.Format("use_view={0}", cta.ComponentTemplate.MetadataFields["view"].Value);

                        if (randomInsert)
                        {
                            components.Insert(rnd.Next(min, max), (Component)cta.Component);
                        }
                        else
                        {
                            // Changed so if not random, put at front
                            components.Insert(0, (Component)cta.Component); 
                            //components.Add((Component)cta.Component);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack): {0}", ex);
                }
            }
            return components;
        }

        public static Query BuildQuery<T>(ref T facetedContent, int itemsPerPage, string viewType, int publicationId) where T : FacetedContentBase
        {
            #region Build FredHopper Query

            string fh_location = facetedContent.FredHopperLocation;

            var loc = new com.fredhopper.lang.query.location.Location(fh_location);

            // Active Discussion filter
            if (facetedContent.Sort != null && facetedContent.Sort.ActiveDiscussions)
            {
                loc.SetActiveDiscussion();
            }

            var query = new Query(loc);

            // Set the view type to lister
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);

            // Set number of items per page
            query.setListViewSize(itemsPerPage);

            //// Create the "Sort by" select list
            //if (facetedContent.Sort == null) { facetedContent.Sort = new FacetSort(); }
            //facetedContent.Sort.SortByList = CreateSortByList(facetedContent.Sort.SortBy, query);

            // Set the sort by attribute
            //query.setSortingBy(facetedContent.Sort.SortBy);
            var tmpQuery = new Query();

            if (facetedContent.Sort != null)
            {
                tmpQuery.ParseQuery(HttpUtility.UrlDecode(facetedContent.Sort.SortBy.Replace("?fh_params=", "")));
                query.setSortingBy(GetSelectedSortingAttribute(tmpQuery.getSortingAttribute(0), tmpQuery.getSortingDirection(0), viewType, publicationId));
            }

            // Remove any search criteria from the location 
            query.getLocation().removeSearchCriteria();

            if (!string.IsNullOrEmpty(facetedContent.ComponentSection.SearchTerm))
            {
                //HttpContext.Current.Session.Add("SEARCH", facetedContent.ComponentSection.SearchTerm);
                com.fredhopper.lang.query.location.criteria.Criterion searchCriterion = new com.fredhopper.lang.query.location.criteria.SearchCriterion(facetedContent.ComponentSection.SearchTerm);
                query.getLocation().addCriterion(searchCriterion);
            }

            // Store the current query
            fh_location = query.getLocation().toString();
            facetedContent.FredHopperLocation = fh_location;

            #endregion

            return query;
        }

        /// <summary>
        /// Appends the optional facet.
        /// </summary>
        /// <param name="fh_params">The fh_params.</param>
        /// <param name="optionalFacet">The optional facet.</param>
        /// <returns></returns>
        public static string AppendOptionalFacet(string fh_params, string optionalFacet)
        {
            string fhLocationNew = string.Empty;
            string toReturn = string.Empty;
            string[] facetParts = optionalFacet.Split('|');
            string[] fhParamParts = fh_params.Split('&');

            //fh_location=//catalog01/en_US/publicationid=tcm_0_70_1/schematitle>{crafts2ecarerepair;crafts2efaq;crafts2etutorial}/techniques>{techniques__dir__sewing}
            //&fh_view_size=10
            //&fh_sort_by=-publishdate

            // get location section and position
            int locationPosition = 0;
            string fhLocation = string.Empty;
            foreach (string x in fhParamParts)
            {
                if (x.StartsWith(@"fh_location"))
                {
                    fhLocation = x;
                    break;
                }
                locationPosition++;
            }

            // append optional facet to query
            if (fhLocation != string.Empty)
            {
                if (fhLocation.Contains(facetParts[0]))
                {
                    // category is already present
                    int posStart = fhLocation.IndexOf(facetParts[0] + ">{") + (facetParts[0] + ">{").Length;
                    int posEnd = fhLocation.IndexOf("}", posStart);

                    string[] filters = fhLocation.Substring(posStart, posEnd - posStart).Split(';');
                    bool alreadyPresent = (from x in filters.Where(y => y == facetParts[1])
                                           select x).Any();
                    if (!alreadyPresent)
                    {
                        string insertText = string.Empty;
                        foreach (string filter in filters)
                        {
                            if (insertText == string.Empty)
                                insertText += filter;
                            else
                                insertText += ";" + filter;
                        }
                        if (insertText == string.Empty)
                            insertText = facetParts[1];
                        else
                        {
                            //insertText += ";" + facetParts[0] + @"__" + facetParts[1];
                            insertText += ";" + facetParts[0] + NestedLevelDelimiter + facetParts[1];
                        }


                        insertText = @"{" + insertText + @"}";

                        fhLocationNew = fhLocation.Substring(0, posStart - 1) +
                                        insertText +
                                        fhLocation.Substring(posEnd + 1, (fhLocation.Length - 1) - posEnd);
                    }
                    else
                        fhLocationNew = fhLocation;   // do nothing
                }
                else
                {
                    //fhLocationNew = fhLocation + @"/" + facetParts[0] + ">{" + facetParts[0] + "__" + facetParts[1] + "}";
                    fhLocationNew = fhLocation + @"/" + facetParts[0] + ">{" + facetParts[0] + NestedLevelDelimiter + facetParts[1] + "}";
                }

                // build up return string
                toReturn = fhLocationNew;
                foreach (string x in fhParamParts)
                {
                    if (!x.StartsWith(@"fh_location"))
                    {
                        toReturn += "&" + x;
                    }
                }
            }
            else
            {
                //fhLocationNew = fh_params + @"/" + facetParts[0] + ">{" + facetParts[0] + "__" + facetParts[1] + "}";
                fhLocationNew = fh_params + @"/" + facetParts[0] + ">{" + facetParts[0] + NestedLevelDelimiter + facetParts[1] + "}";
                toReturn = fhLocationNew;
            }


            return toReturn;
        }

        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="facetedContent">Content of the faceted.</param>
        /// <param name="fh_params">The fh_params.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="optionalFacet">The optional facet.</param>
        /// <returns></returns>
        public static Query BuildQuery<T>(ref T facetedContent, string fh_params, string viewType, int itemsPerPage, string defaultLocation, int publicationId, string optionalFacet = "", string nonSelected = "") where T : FacetedContentBase, new()
        {
            #region Build FredHopper Query

            //facetedContent = new T();
            com.fredhopper.lang.query.Query query = null;

            // Get the display component types (schemas. e.g. Projects; Articles; Mood boards; Designers)
            //facetedContent.ComponentSection = new ComponentSearchSection();

            // Check if the call includes query string params or not
            if (!string.IsNullOrEmpty(fh_params))
            {
                // Parse the query string params into the facetedContent object
                query = new com.fredhopper.lang.query.Query();

                // URL REWRITE PRE-SELECTED BRAND FILTER - append optional facet
                if (optionalFacet != string.Empty)
                    fh_params = AppendOptionalFacet(fh_params, optionalFacet);

                query.ParseQuery(fh_params);

                // Get the default component types for this view (Articles, Moodboard, etc)
                //facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType, true);
                //if (viewType.ToLower() == "productexplorer" || viewType.ToLower() == "ourbrands" || viewType.ToLower() == "storelocator")
                if (FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType);
                }
                else
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType, true, nonSelected);
                }

                facetedContent.ComponentSection.ComponentTypes.Facets = query.ParseSelectedSchemaTypes(facetedContent.ComponentSection.ComponentTypes.Facets);
                facetedContent.Sort = new FacetSort();

                facetedContent.Sort.ActiveDiscussions = query.getLocation().IsActiveDiscussion();
            }
            else
            {
                // No query string params, so use default location
                com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(defaultLocation);

                // Get the default component types for this view (Articles, Moodboard, etc)
                //facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType, true);
                //if(viewType.ToLower() == "productexplorer" || viewType.ToLower() == "ourbrands" || viewType.ToLower() == "storelocator")
                if (FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType);
                }
                else
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType, true, nonSelected);
                }

                //// Add the newly created criterion
                //var componentLoc = com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(facetedContent.ComponentSection.ComponentTypes.ToLocationString());
                //loc.addCriterion(componentLoc);

                // URL REWRITE PRE-SELECTED BRAND FILTER - add optional facet
                if (optionalFacet != string.Empty)
                {
                    // add optional facet to query
                    // If from within Prodict Explorer, dont add component types!
                    if (!FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
                    {
                        var componentLoc = CriterionFactory.parse(facetedContent.ComponentSection.ComponentTypes.ToLocationString());
                        loc.addCriterion(componentLoc);
                    }

                    fh_params = "fh_location=" + AppendOptionalFacet(loc.toString(), optionalFacet);
                    query = new com.fredhopper.lang.query.Query();
                    query.ParseQuery(fh_params);
                }
                else
                    // normal operation
                    query = new com.fredhopper.lang.query.Query(loc);
            }

            facetedContent.ComponentSection.ComponentTypes.SectionTitle = "Show me";

            // Set the view type to lister
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);

            // Set number of items per page
            if (query.getListViewSize() == -1)
            {
                query.setListViewSize(itemsPerPage);
            }

            // Store the current query location in the model
            //facetedContent.FredHopperLocation = query.getLocation().toString();

            //###ASH
            //// Create the "Sort by" select list
            //string sortingAttribute = query.getSortingAttribute(0);
            //facetedContent.Sort.SortByList = CreateSortByList(sortingAttribute, query.getSortingDirection(0), query, out sortingAttribute);

            // Set the sort by attribute
            query.setSortingBy(GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId));

            // Set the search phrase
            facetedContent.ComponentSection.SearchTerm = query.getSearchPhrase();

            #endregion

            return query;
        }

        public static string GetSelectedSortingAttribute(string sortingAttribute, SortDirection sortingDirection, string viewType, int publicationId)
        {
            if (sortingAttribute == null)
            {
                viewType = viewType.ToLower();
                //if (viewType != "productexplorer" && viewType != "ourbrands" && viewType != "storelocator")
                if (!FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
                {
                    // Default to featured content if non of the above.
                    return "-featuredcontent";
                }
                else if (viewType == "ourbrands")
                {
                    // Default to featured content, then published date for "our brands".
                    return "-featuredcontent,-publishdate";
                }
                else
                {
                    // Product explorer, our brands and store locator don't have featrued content, so default to publish date.
                    // JM - changed this to alphabetical
                    //return "title";
                    
                    // ASH:
                    // The following was being ignored by FredHopper. Suspect that it is because the attribute id ("71_title") starts
                    // with in integer, so created a duplicate attribute, but re-named it to "sort_by_title"
                    //return string.Format("{0}_title", publicationId); // Title is used for seraching, so need to apply the pub id e.g. "71_title"
                    //return "sort_by_title";

                    // Rach: CCPLCR-118 - default is now Most popular
                    //return "-rating";

                    // CCPLCR-144 - default is now Most recent
                    return "-publishdate";
                }
            }
            else
            {
                if (sortingDirection == SortDirection.DESC)
                {
                    return "-" + sortingAttribute;
                }
            }
            return sortingAttribute;
        }

        public static List<SelectListItem> CreateSortByList(Query query, string viewType, int publicationId, out string selectedSortingAttribute)
        {
            selectedSortingAttribute = GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId);
            return CreateSortByList(query, viewType, publicationId);
        }

        public static List<SelectListItem> CreateSortByList(Query query, string viewType, int publicationId)
        {
            // Store the query's existing settings
            string sortingAttribute = GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId);
            int startIndex = query.getListStartIndex();
             
            List<SelectListItem> sortByList = new List<SelectListItem>();

            // Reset the query's start index to the first page.
            query.setListStartIndex(0);

            viewType = viewType.ToLower();
            //if (viewType != "productexplorer" && viewType != "ourbrands" && viewType != "storelocator")
            if (!FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
            {
                // Only add the featured content sort by option if the view type is non of the above
                query.setSortingBy("-featuredcontent");

                sortByList.Add(new SelectListItem { Selected = (sortingAttribute == "-featuredcontent"), Text = Helper.GetResource("FeaturedContent"), Value = query.ToFhParams() });
            }

            query.setSortingBy("-publishdate");
            sortByList.Add(new SelectListItem { Selected = (sortingAttribute == "-publishdate"), Text = Helper.GetResource("MostRecent"), Value = query.ToFhParams() });

            if (!FacetedContentHelper.IsViewFromProductExplorerSection(viewType))
            {
                query.setSortingBy("-rating");
                sortByList.Add(new SelectListItem { Selected = (sortingAttribute == "-rating"), Text = Helper.GetResource("MostPopular"), Value = query.ToFhParams() });
            }

            // Title is used for searching, so requires the pub id, e.g. "71_title"
            //query.setSortingBy("title");
            //sortByList.Add(new SelectListItem { Selected = (sortingAttribute == "title"), Text = Helper.GetResource("AtoZ"), Value = query.ToFhParams() });
            
            // ASH:
            // The following was being ignored by FredHopper. Suspect that it is because the attribute id ("71_title") starts
            // with in integer, so created a duplicate attribute, but re-named it to "sort_by_title"
            //string title = string.Format("{0}_title", publicationId); // Title is used for seraching, so need to apply the pub id e.g. "71_title"
            string title = "sort_by_title"; 
            
            query.setSortingBy(title);
            sortByList.Add(new SelectListItem { Selected = (sortingAttribute == title), Text = Helper.GetResource("AtoZ"), Value = query.ToFhParams() });

            //query.setSortingBy("-title");
            //sortByList.Add(new SelectListItem { Selected = (sortingAttribute == "-title"), Text = Helper.GetResource("ZtoA"), Value = query.ToFhParams() });
            title = "-" + title;
            query.setSortingBy(title);
            sortByList.Add(new SelectListItem { Selected = (sortingAttribute == title), Text = Helper.GetResource("ZtoA"), Value = query.ToFhParams() });

            // Return the query to it's original state
            query.setSortingBy(sortingAttribute);
            query.setListStartIndex(startIndex);

            return sortByList;
        }

        public static string GetActiveDiscussionUrl(Query query)
        {
            com.fredhopper.lang.query.location.Location loc = new com.fredhopper.lang.query.location.Location(query.getLocation());
            if (loc.IsActiveDiscussion())
            {
                // If the active discussion filter has already been selected, then we need to create the "remove" link
                loc.RemoveActiveDiscussion();
            }
            else
            {
                // Alternatively, if the active discussion filter has not been selected, then we need to create the "set" link
                loc.SetActiveDiscussion();
            }
            Query adQuery = new Query(loc);
            return adQuery.ToFhParams();
        }

        public static FacetSection GetComponentSchemaTypes(string viewType)
        {
            return GetComponentSchemaTypes(viewType, null, "");
        }

        public static FacetSection GetComponentSchemaTypes(string viewType, bool? selectedState, string nonSelected){
            return GetComponentSchemaTypes(viewType, selectedState, nonSelected, null);
        }

        public static FacetSection GetComponentSchemaTypes(string viewType, bool? selectedState, string nonSelected, FacetedContent content)
        {
            // List of items returned in results - to compare so that only those available can be used.
            List<FASWebService.filtersection> returnedSchemasList = new List<FASWebService.filtersection>();
            // List of selected items in the model - so we can check if one remains
            IList<FacetItem> selectedContentTypes = new List<FacetItem>();

            if (content != null)
            {
                if (content.ReturnedSchemas != null)
                {
                    returnedSchemasList = content.ReturnedSchemas.filtersection.ToList();
                    selectedContentTypes = content.ComponentSection.ComponentTypes.Facets;
                }
            }
            //RM
            List<FacetItem> componentSchemaTypes = new List<FacetItem>();

            switch (viewType.ToLower())
            {
                case "discover":
                    //crafts2eproject
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("ProjectsAndPatterns"), Value = "crafts2eproject"});
                    //crafts2earticle
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Articles"), Value = "crafts2earticle" });
                    //crafts2emoodboard
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("MoodBoards"), Value = "crafts2emoodboard" });
                    //crafts2edesigner
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Designers"), Value = "crafts2edesigner" });                    
                    break;
                case "learn":
                    //crafts2etutorial
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Tutorials"), Value = "crafts2etutorial" });
                    //crafts2ecarerepair
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("CareAndRepair"), Value = "crafts2ecarerepair" });
                    //crafts2efaq
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("FAQs"), Value = "crafts2efaq" });
                    break;
                case "share":
                    //crafts2eblog
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Blog"), Value = "crafts2eblog" });
                    //crafts2eactivediscussions
                    //componentSchemaTypes.Add(new FacetItem { Text = "Active discussions", Value = "crafts2eactivediscussions" });
                    //crafts2eevent
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Events"), Value = "crafts2eevent" });
                    break;
                //case "events":
                //    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Events"), Value = "crafts2eevent" });
                //    break;
                case "productexplorer":
                case "ourbrands":
                case "storelocator":
                    // NOT the most elegant of solutions, but haven't got the time to re-work things!!!
                    // Bumming in on the component schema types to provide the links.

                    var urlProductExplorer = WebConfiguration.Current.ProductExplorer.AddApplicationRoot();
                    var urlOurBrands = WebConfiguration.Current.Brands.AddApplicationRoot();
                    var urlStoreFinder = WebConfiguration.Current.StoreFinder.AddApplicationRoot();

                    //crafts2eproduct
                    // Products link
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Products"), Value = urlProductExplorer, Selected = viewType.ToLower() == "productexplorer" });
                    //crafts2ebrand
                    // Our brands link
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("OurBrands"), Value = urlOurBrands, Selected = viewType.ToLower() == "ourbrands" });
                    //Store locator link
                    componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("StoreLocator"), Value = urlStoreFinder, Selected = viewType.ToLower() == "storelocator" });
                    break;
            }

            if (selectedState.HasValue)
            {
                foreach (var item in componentSchemaTypes)
                {
                    if (item.Value != nonSelected)
                    {
                        item.Selected = selectedState.Value;
                    }
                }
            }

            //we are filtering the by returned schemas so set enabled=false on any items not in the returnedSchemasList
            if (returnedSchemasList.Count > 0)
            {
                componentSchemaTypes.Where(x => !returnedSchemasList.Exists(y => y.value.Value == x.Value)).ToList()
                    .ForEach(x =>
                    {
                        x.Enabled = false;
                        x.Selected = false;
                    });
            }
            else
            {
                // Edge case - content not null, but no returned schema in results, set all to disabled
                if (content != null)
                {
                    if (content.ReturnedSchemas == null)
                        componentSchemaTypes.ForEach(c => {
                            c.Enabled = false;
                            c.Selected= false;
                        });
                }
            }

            // Check if theres only one left in possible results...
            int facetsResultsLeft = componentSchemaTypes.Count(c => c.Enabled == true);
            if (facetsResultsLeft == 1)
            {
                componentSchemaTypes.SingleOrDefault(c => c.Enabled == true).Enabled = false;
            }

            // Check how many selected are actually enabled if only one then this is disabled as well to stop a whoops
            var s = (from selected in selectedContentTypes
                     join enabled in componentSchemaTypes on selected.Value equals enabled.Value
                     where selected.Selected && enabled.Enabled
                     select selected).ToList();

            int facetsLeftEnabled = s.Count();         
            if (facetsLeftEnabled > 0)
            {
                // Assign selected state
                componentSchemaTypes.ToList().ForEach(x =>
                    {
                        var b = s.Exists(y => y.Value == x.Value);
                        x.Selected = b;
                    });
            }

            // Make sure the only one left is disabled
            if (facetsLeftEnabled == 1 && componentSchemaTypes.Count(f => f.Enabled && !f.Selected) == 0)
            {
                var selected = s[0];
                componentSchemaTypes.SingleOrDefault(c => c.Value == selected.Value).Enabled = false;
            }

            // Create the return object and return
            FacetSection facetSection = new FacetSection();
            facetSection.Facets = componentSchemaTypes;
            return facetSection;
        }


    }
}