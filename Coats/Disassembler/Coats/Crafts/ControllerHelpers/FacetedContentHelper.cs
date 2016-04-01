namespace Coats.Crafts.ControllerHelpers
{
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.FASWebService;
    using Coats.Crafts.Models;
    using Coats.Crafts.Resources;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.Mvc;

    public sealed class FacetedContentHelper
    {
        private static object syncRoot = new object();

        internal static int DefaultMaximumViewSize = 20;
        private const string MaximumViewSizeConfigKey = "Fredhopper.MaximumViewSize";

        public static int AssertCappedViewSize(int requested)
        {
            // assume maximum value for unset query.getListViewSize() values
            if(requested < 0) { requested = int.MaxValue; } 

            int configured;
            int max;
            if (int.TryParse(ConfigurationManager.AppSettings[MaximumViewSizeConfigKey], out configured))
            {
                max = configured;
            }
            else
            {
                max = DefaultMaximumViewSize;
            }
            return (requested < max) ? requested : max;
        }

        public static string AppendOptionalFacet(string fh_params, string optionalFacet)
        {
            Func<string, bool> predicate = null;
            string str = string.Empty;
            string str2 = string.Empty;
            string[] facetParts = optionalFacet.Split(new char[] { '|' });
            string[] strArray = fh_params.Split(new char[] { '&' });
            int num = 0;
            string str3 = string.Empty;
            foreach (string str4 in strArray)
            {
                if (str4.StartsWith("fh_location"))
                {
                    str3 = str4;
                    break;
                }
                num++;
            }
            if (str3 != string.Empty)
            {
                if (str3.Contains(facetParts[0]))
                {
                    int startIndex = str3.IndexOf(facetParts[0] + ">{") + (facetParts[0] + ">{").Length;
                    int index = str3.IndexOf("}", startIndex);
                    string[] source = str3.Substring(startIndex, index - startIndex).Split(new char[] { ';' });
                    if (predicate == null)
                    {
                        predicate = y => y == facetParts[1];
                    }
                    if (!(from x in source.Where<string>(predicate) select x).Any<string>())
                    {
                        string str5 = string.Empty;
                        foreach (string str6 in source)
                        {
                            if (str5 == string.Empty)
                            {
                                str5 = str5 + str6;
                            }
                            else
                            {
                                str5 = str5 + ";" + str6;
                            }
                        }
                        if (str5 == string.Empty)
                        {
                            str5 = facetParts[1];
                        }
                        else
                        {
                            string str8 = str5;
                            str5 = str8 + ";" + facetParts[0] + NestedLevelDelimiter + facetParts[1];
                        }
                        str5 = "{" + str5 + "}";
                        str = str3.Substring(0, startIndex - 1) + str5 + str3.Substring(index + 1, (str3.Length - 1) - index);
                    }
                    else
                    {
                        str = str3;
                    }
                }
                else
                {
                    str = str3 + "/" + facetParts[0] + ">{" + facetParts[0] + NestedLevelDelimiter + facetParts[1] + "}";
                }
                str2 = str;
                foreach (string str4 in strArray)
                {
                    if (!str4.StartsWith("fh_location"))
                    {
                        str2 = str2 + "&" + str4;
                    }
                }
                return str2;
            }
            return (fh_params + "/" + facetParts[0] + ">{" + facetParts[0] + NestedLevelDelimiter + facetParts[1] + "}");
        }

        public static Query BuildQuery<T>(ref T facetedContent, int itemsPerPage, string viewType, int publicationId) where T : FacetedContentBase
        {
            itemsPerPage = FacetedContentHelper.AssertCappedViewSize(itemsPerPage);

            Location loc = new Location(facetedContent.FredHopperLocation);
            if ((facetedContent.Sort != null) && facetedContent.Sort.ActiveDiscussions)
            {
                loc.SetActiveDiscussion();
            }
            Query query = new Query(loc);
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);
            query.setListViewSize(itemsPerPage);
            Query query2 = new Query();
            if (facetedContent.Sort != null)
            {
                query2.ParseQuery(HttpUtility.UrlDecode(facetedContent.Sort.SortBy.Replace("?fh_params=", "")));
                query.setSortingBy(GetSelectedSortingAttribute(query2.getSortingAttribute(0), query2.getSortingDirection(0), viewType, publicationId));
            }
            query.getLocation().removeSearchCriteria();
            if (!string.IsNullOrEmpty(facetedContent.ComponentSection.SearchTerm))
            {
                Criterion criterion = new SearchCriterion(facetedContent.ComponentSection.SearchTerm);
                query.getLocation().addCriterion(criterion);
            }
            string str = query.getLocation().toString();
            facetedContent.FredHopperLocation = str;
            return query;
        }

        public static Query BuildQuery<T>(ref T facetedContent, string fh_params, string viewType, int itemsPerPage, string defaultLocation, int publicationId, string optionalFacet = "", string nonSelected = "") where T : FacetedContentBase, new()
        {
            itemsPerPage = FacetedContentHelper.AssertCappedViewSize(itemsPerPage);
            Query query = null;
            if (!string.IsNullOrEmpty(fh_params))
            {
                query = new Query();
                if (optionalFacet != string.Empty)
                {
                    fh_params = AppendOptionalFacet(fh_params, optionalFacet);
                }
                query.ParseQuery(fh_params);
                if (IsViewFromProductExplorerSection(viewType))
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
                Location location = new Location(defaultLocation);
                if (IsViewFromProductExplorerSection(viewType))
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType);
                }
                else
                {
                    facetedContent.ComponentSection.ComponentTypes = GetComponentSchemaTypes(viewType, true, nonSelected);
                }
                if (optionalFacet != string.Empty)
                {
                    if (!IsViewFromProductExplorerSection(viewType))
                    {
                        Criterion criterion = CriterionFactory.parse(facetedContent.ComponentSection.ComponentTypes.ToLocationString());
                        location.addCriterion(criterion);
                    }
                    fh_params = "fh_location=" + AppendOptionalFacet(location.toString(), optionalFacet);
                    query = new Query();
                    query.ParseQuery(fh_params);
                }
                else
                {
                    query = new Query(location);
                }
            }
            facetedContent.ComponentSection.ComponentTypes.SectionTitle = "Show me";
            query.setView(com.fredhopper.lang.query.ViewType.LISTER);
            query.setListViewSize(itemsPerPage);
            query.setSortingBy(GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId));
            facetedContent.ComponentSection.SearchTerm = query.getSearchPhrase();
            return query;
        }

        public static List<SelectListItem> CreateSortByList(Query query, string viewType, int publicationId)
        {
            string sortAttribute = GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId);
            int offset = query.getListStartIndex();
            List<SelectListItem> list = new List<SelectListItem>();
            query.setListStartIndex(0);
            viewType = viewType.ToLower();
            if (!IsViewFromProductExplorerSection(viewType))
            {
                query.setSortingBy("-featuredcontent");
                SelectListItem item = new SelectListItem
                {
                    Selected = sortAttribute == "-featuredcontent",
                    Text = Helper.GetResource("FeaturedContent"),
                    Value = query.ToFhParams()
                };
                list.Add(item);
            }
            query.setSortingBy("-publishdate");
            SelectListItem item3 = new SelectListItem
            {
                Selected = sortAttribute == "-publishdate",
                Text = Helper.GetResource("MostRecent"),
                Value = query.ToFhParams()
            };
            list.Add(item3);
            if (!IsViewFromProductExplorerSection(viewType))
            {
                query.setSortingBy("-rating");
                SelectListItem item2 = new SelectListItem
                {
                    Selected = sortAttribute == "-rating",
                    Text = Helper.GetResource("MostPopular"),
                    Value = query.ToFhParams()
                };
                list.Add(item2);
            }
            string str2 = "sort_by_title";
            query.setSortingBy(str2);
            SelectListItem item4 = new SelectListItem
            {
                Selected = sortAttribute == str2,
                Text = Helper.GetResource("AtoZ"),
                Value = query.ToFhParams()
            };
            list.Add(item4);
            str2 = "-" + str2;
            query.setSortingBy(str2);
            SelectListItem item5 = new SelectListItem
            {
                Selected = sortAttribute == str2,
                Text = Helper.GetResource("ZtoA"),
                Value = query.ToFhParams()
            };
            list.Add(item5);
            query.setSortingBy(sortAttribute);
            query.setListStartIndex(offset);
            return list;
        }

        public static List<SelectListItem> CreateSortByList(Query query, string viewType, int publicationId, out string selectedSortingAttribute)
        {
            selectedSortingAttribute = GetSelectedSortingAttribute(query.getSortingAttribute(0), query.getSortingDirection(0), viewType, publicationId);
            return CreateSortByList(query, viewType, publicationId);
        }

        public static string GetActiveDiscussionUrl(Query query)
        {
            Location loc = new Location(query.getLocation());
            if (loc.IsActiveDiscussion())
            {
                loc.RemoveActiveDiscussion();
            }
            else
            {
                loc.SetActiveDiscussion();
            }
            Query query2 = new Query(loc);
            query2.setListViewSize(FacetedContentHelper.AssertCappedViewSize(int.MaxValue));
            return query2.ToFhParams();
        }

        public static FacetSection GetComponentSchemaTypes(string viewType)
        {
            return GetComponentSchemaTypes(viewType, null, "");
        }

        public static FacetSection GetComponentSchemaTypes(string viewType, bool? selectedState, string nonSelected)
        {
            return GetComponentSchemaTypes(viewType, selectedState, nonSelected, null);
        }

        public static FacetSection GetComponentSchemaTypes(string viewType, bool? selectedState, string nonSelected, FacetedContent content)
        {
            Action<FacetItem> action = null;
            List<filtersection> returnedSchemasList = new List<filtersection>();
            IList<FacetItem> selectedContentTypes = new List<FacetItem>();
            if ((content != null) && (content.ReturnedSchemas != null))
            {
                returnedSchemasList = content.ReturnedSchemas.filtersection.ToList<filtersection>();
                selectedContentTypes = content.ComponentSection.ComponentTypes.Facets;
            }
            List<FacetItem> componentSchemaTypes = new List<FacetItem>();
            switch (viewType.ToLower())
            {
                case "discover":
                    {
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("ProjectsAndPatterns"), Value = "crafts2eproject" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Articles"), Value = "crafts2earticle" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("MoodBoards"), Value = "crafts2emoodboard" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Designers"), Value = "crafts2edesigner" });
                        break;
                    }
                case "learn":
                    {
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Tutorials"), Value = "crafts2etutorial" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("CareAndRepair"), Value = "crafts2ecarerepair" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("FAQs"), Value = "crafts2efaq" });
                        break;
                    }
                case "share":
                    {
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Blog"), Value = "crafts2eblog" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Events"), Value = "crafts2eevent" });
                        break;
                    }
                case "productexplorer":
                case "ourbrands":
                case "storelocator":
                    {
                        string urlProductExplorer = WebConfiguration.Current.ProductExplorer.AddApplicationRoot();
                        string urlOurBrands = WebConfiguration.Current.Brands.AddApplicationRoot();
                        string urlStoreFinder = WebConfiguration.Current.StoreFinder.AddApplicationRoot();
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("Products"), Value = urlProductExplorer, Selected = viewType.ToLower() == "productexplorer" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("OurBrands"), Value = urlOurBrands, Selected = viewType.ToLower() == "ourbrands" });
                        componentSchemaTypes.Add(new FacetItem { Text = Helper.GetResource("StoreLocator"), Value = urlStoreFinder, Selected = viewType.ToLower() == "storelocator" });
                        break;
                    }
            }
            if (selectedState.HasValue)
            {
                foreach (FacetItem item in componentSchemaTypes)
                {
                    if (item.Value != nonSelected)
                    {
                        item.Selected = selectedState.Value;
                    }
                }
            }
            if (returnedSchemasList.Count > 0)
            {
                componentSchemaTypes.Where(x => !returnedSchemasList.Exists(y => y.value.Value == x.Value)).ToList()
                    .ForEach(x =>
                    {
                        x.Enabled = false;
                        x.Selected = false;
                    });
            }
            else if ((content != null) && (content.ReturnedSchemas == null))
            {
                componentSchemaTypes.ForEach(c => {
                    c.Enabled = false;
                    c.Selected = false;
                });
            }
            if (componentSchemaTypes.Count<FacetItem>(c => c.Enabled) == 1)
            {
                componentSchemaTypes.SingleOrDefault<FacetItem>(c => c.Enabled).Enabled = false;
            }

            var s = (from selected in selectedContentTypes
                     join enabled in componentSchemaTypes on selected.Value equals enabled.Value
                     where selected.Selected && enabled.Enabled
                     select selected).ToList();
            int facetsLeftEnabled = s.Count<FacetItem>();
            if (facetsLeftEnabled > 0)
            {
                if (action == null)
                {
                    action = delegate (FacetItem x)
                    {
                        bool flag = s.Exists(y => y.Value == x.Value);
                        x.Selected = flag;
                    };
                }
                componentSchemaTypes.ToList<FacetItem>().ForEach(action);
            }
            if ((facetsLeftEnabled == 1) && (componentSchemaTypes.Count<FacetItem>(f => (f.Enabled && !f.Selected)) == 0))
            {
                FacetItem selected = s[0];
                componentSchemaTypes.SingleOrDefault<FacetItem>(c => (c.Value == selected.Value)).Enabled = false;
            }
            return new FacetSection { Facets = componentSchemaTypes };
        }

        public static string GetSelectedSortingAttribute(string sortingAttribute, SortDirection sortingDirection, string viewType, int publicationId)
        {
            if (sortingAttribute == null)
            {
                viewType = viewType.ToLower();
                if (!IsViewFromProductExplorerSection(viewType))
                {
                    return "-featuredcontent";
                }
                if (viewType == "ourbrands")
                {
                    return "-featuredcontent,-publishdate";
                }
                return "-publishdate";
            }
            if (sortingDirection == SortDirection.DESC)
            {
                return ("-" + sortingAttribute);
            }
            return sortingAttribute;
        }

        public static List<Component> InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack)
        {
            Logger.Info("InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack)");
            Random random = new Random();
            int minValue = Math.Min(4, historyBack ? 9 : (components.Count - 1));
            int maxValue = Math.Min(components.Count - 1, historyBack ? 9 : (components.Count - 1));
            if (minValue >= maxValue)
            {
                randomInsert = false;
            }
            Logger.InfoFormat("randomInsert: {0}", new object[] { randomInsert });
            Logger.InfoFormat("ctaList.Count: {0}", new object[] { ctaList.Count });
            CtaTemplateIdConfig instance = CtaTemplateIdConfig.Instance;
            Logger.InfoFormat("config: {0}", new object[] { instance.GetHashCode() });
            foreach (IComponentPresentation presentation in ctaList)
            {
                try
                {
                    string key = UtilityHelper.GetTcmUri(presentation.ComponentTemplate.Id).TcmItemId.ToString();
                    bool flag = instance.Templates.ContainsKey(key);
                    Logger.InfoFormat("checkTemplate: {0}", new object[] { flag });
                    if (flag)
                    {
                        Logger.InfoFormat("use_view: {0}", new object[] { presentation.ComponentTemplate.MetadataFields["view"].Value });
                        presentation.Component.Url = string.Format("use_view={0}", presentation.ComponentTemplate.MetadataFields["view"].Value);
                        if (randomInsert)
                        {
                            components.Insert(random.Next(minValue, maxValue), (Component)presentation.Component);
                        }
                        else
                        {
                            components.Insert(0, (Component)presentation.Component);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.ErrorFormat("InjectCTAComponents(List<Component> components, IList<IComponentPresentation> ctaList, bool randomInsert, bool historyBack): {0}", new object[] { exception });
                }
            }
            return components;
        }

        public static T InjectCTAComponents<T>(T facetedContent, IList<IComponentPresentation> ctaList, bool randomInsert = false, bool historyBack = false) where T : FacetedContentBase
        {
            if (((facetedContent.ComponentList != null) && (facetedContent.ComponentList.Pagination != null)) && ((historyBack || (facetedContent.ComponentList.Pagination.Count == 0)) || ((facetedContent.ComponentList.Pagination.Count > 0) && facetedContent.ComponentList.Pagination[0].Selected)))
            {
                facetedContent.ComponentList.Components = InjectCTAComponents(facetedContent.ComponentList.Components, ctaList, randomInsert, historyBack);
            }
            return facetedContent;
        }

        public static T InjectIntroComponent<T>(T facetedContent, IField introMarkup, bool historyBack) where T : FacetedContentBase
        {
            try
            {
                if ((historyBack || (facetedContent.ComponentList.Pagination.Count == 0)) || ((facetedContent.ComponentList.Pagination.Count > 0) && facetedContent.ComponentList.Pagination[0].Selected))
                {
                    Component item = new Component
                    {
                        Fields = { {
                            "intro",
                            introMarkup
                        } },
                        Schema = { Title = WebConfiguration.Current.ContentTypeIntroduction },
                        Title = "Intro"
                    };
                    facetedContent.ComponentList.Components.Insert(0, item);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat("InjectIntroComponent<T>(T facetedContent, IField introMarkup, bool historyBack): {0}", new object[] { exception });
            }
            return facetedContent;
        }

        public static bool IsViewFromProductExplorerSection(string view)
        {
            bool flag = false;
            if (((view.ToLower() == "productexplorer") || (view.ToLower() == "ourbrands")) || (view.ToLower() == "storelocator"))
            {
                flag = true;
            }
            return flag;
        }

        public static ILogger Logger
        {
            get
            {
                IContainerAccessor applicationInstance = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                return applicationInstance.Container.Resolve<ILogger>();
            }
        }

        public static string NestedLevelDelimiter
        {
            get
            {
                return WebConfiguration.Current.NestedLevelDelimiter;
            }
        }
    }
}

