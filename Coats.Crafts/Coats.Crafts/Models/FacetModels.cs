using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DD4T.ContentModel;
using Coats.Crafts.Extensions;
using DD4T.Factories;
using Coats.Crafts.FredHopper;
using System.Text;
using System.Dynamic;
using System.Text.RegularExpressions;

using com.fredhopper.lang.query.location;
using com.fredhopper.lang.query;
using com.fredhopper.lang.query.location.criteria;


namespace Coats.Crafts.Models
{
    public class FacetItem : SelectListItem
    {
        public int NumItems { get; set; }
        /// <summary>
        /// Used for get actions (full query url)
        /// </summary>
        public string Href { get; set; }
        public string FHId { get; set; }
        private bool _enabled = true;
        public FacetSection childSection { get; set; }
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
    }

    public class FacetSection
    {
        // Maintaining the FH attibute name
        public string on { get; set; }
        public bool Visible { get; set; }
        public string SectionTitle { get; set; }
        public Coats.Crafts.FASWebService.customfield[] CustomFields { get; set; }
        public IList<FacetItem> Facets { get; set; }
        public string ToLocationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("schematitle>{");
            try
            {
                foreach (var facet in Facets.Where(f => f.Selected))
                {
                    sb.AppendFormat("{0};", facet.Value);
                }
                sb.Append("}");
                return sb.ToString().Replace(";}", "}");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Intended for use with the ComponentSection.
        /// Originally, the Component section checkboxes (e.g. "Articles", "Projects & Patterns", "Moodboards", "Designers", etc)
        /// were intended to be used in a form post submit.
        /// However, nearly all FredHopper faceted content lookups have been converted to href links - so this is a little helper
        /// methoed to follow suit.
        /// </summary>
        /// <returns></returns>
        public void SetHrefLinks(com.fredhopper.lang.query.Query query)
        {
            // Make a copy of the original location and list start index, just for the hell of it!
            Location locOrg = new Location(query.getLocation());
            int startIndexOrg = query.getListStartIndex();
            
            // Strip the list start index from the query (we will return it later).
            // This is because we want any links to refresh to page one.
            query.removeListStartIndex();


            // CCPLCR-1 - default unchecked content-types
            // Get total of selected facets before we start changing anything
            int selected = Facets.Count(f => f.Selected);
            // This is true on first load as we add it at the end
            string ticked = HttpContext.Current.Request["fh_ticked"] ?? "";


            // Loop through facet list
            foreach (var facet in Facets)
            {
                // Copy the original location to manipulate
                Location loc = new Location(locOrg);
                
                // CCPLCR-1 - default unchecked content-types
                // Initial load means no params on querystring. Use this set facet unticked but still enabled and change link to only add this facet value
                MultiValuedCriterion mvc = loc.getCriterion("schematitle") as MultiValuedCriterion;
                // Not multivalue in Product Explorer so could be null - i
                if (mvc == null)
                    continue;

                // Remove any disabled facets from the criteria (they're disabled as part of GetComponentSchemaTypes()
                Facets.Where(f => !f.Enabled).ToList().ForEach(f =>
                {
                    mvc.getGreaterThan().remove(f.Value);
                });


                // Anything passed?
                if (String.IsNullOrEmpty(ticked))
                {                        
                    // Are all possible facets marked as selected?
                    if (selected == mvc.getGreaterThan().valueSet().size())
                    {                            
                        // Deselect this and change its criteria to only itself
                        facet.Selected = false;
                        mvc.getGreaterThan().clear().add(facet.Value);
                    }
                }
                else
                {
                    // Remove this facet from the query if not in ticked list
                    if (ticked.Split(',').Contains(facet.Value))
                    {
                        mvc.getGreaterThan().remove(facet.Value);
                    }
                    else
                    {
                        mvc.getGreaterThan().add(facet.Value);
                    }

                    // Empty, then remove criteria altogether
                    if (mvc.getGreaterThan().isEmpty())
                        loc.removeCriteria("schematitle");
                }

                // Set the new location
                query.setLocation(loc);

                // Add to the href
                facet.Href = query.ToFhParams();

                // Set correct ticked param ..
                if (String.IsNullOrEmpty(ticked))
                {
                    facet.Href += "&fh_ticked=" + facet.Value;
                }
                else
                {
                    var items = ticked.Split(',').ToList();

                    // Anythning not enabled is removed
                    Facets.Where(f => !f.Enabled).ToList().ForEach(f =>
                    {
                        items.Remove(f.Value);
                    });

                    if (items.Contains(facet.Value))
                    {
                        items.Remove(facet.Value);
                    }
                    else
                    {
                        items.Add(facet.Value);
                    }

                    // Remove the one persisted by the ToFhParams() call above
                    facet.Href = Regex.Replace(facet.Href, @"&fh_ticked=[\w|,]+", "");
                    if (items.Count > 0)
                        facet.Href += "&fh_ticked=" + String.Join(",", items);
                }
            }

            // Finally, return the query's original location and list start index.
            query.setLocation(locOrg);
            query.setListStartIndex(startIndexOrg);

            // For each facet, check the ticked list and set accordingly.
            // If youre not in the list, youre off!
            Facets.ToList().ForEach(f =>
            {
                if (f.Enabled) 
                    f.Selected = false;

                var items = ticked.Split(',').ToList();
                if (f.Enabled && items.Contains(f.Value))
                    f.Selected = true;
            });

            //ticked.Split(',').ToList().ForEach(t =>
            //{
            //    Facets.Where(f => f.Value == t && f.Enabled).ToList().ForEach(f => f.Selected = true);
            //});
        }      
    }

    public class FacetCollection
    {
        public IList<FacetSection> FacetSections { get; set; }
        /// <summary>
        /// Dedicated Url to reset JUST the facet checkboxes. Previously, the components types 
        /// (e.g. "Projecs", "Articles", "Moodboards", "Designers", etc.) were being reset as well.
        /// </summary>
        public string ResetFacetsUrl { get; set; }
    }

    public interface IComponentSearchSection
    {
        FacetSection ComponentTypes { get; set; }
        string SearchTerm { get; set; }
    }

    public class ComponentSearchSection : IComponentSearchSection
    {
        public FacetSection ComponentTypes { get; set; }
        public string SearchTerm { get; set; }
    }

    public class ExtComponentSearchSection : ComponentSearchSection, IComponentSearchSection
    {
        public List<FacetItem> ChooseCategoryList { get; set; }
        public string ChosenCategory { get; set; }
        public bool HasSelectedItems
        {
            get
            {
                try { return ChooseCategoryList.Count(c => c.Selected) > 0; }
                catch (Exception) { return false; }
            }
        }
    }

    public class FacetPagination
    {
        public string Url { get; set; }
        public string LinkText { get; set; }
        public bool Selected { get; set; }
    }

    public class FacetSort
    {
        private bool ?_activeDiscussionsEnabled;
        public IList<SelectListItem> SortByList { get; set; }
        public string SortBy { get; set; }
        public bool ActiveDiscussions { get; set; }
        public string ActiveDiscussionsUrl { get; set; }
        public bool ActiveDiscussionsEnabled
        {
            get 
            {
                if (!_activeDiscussionsEnabled.HasValue)
                {
                    return true; // Default to true
                }
                return _activeDiscussionsEnabled.Value;
            }
            set 
            {
                _activeDiscussionsEnabled = value;
            }
        }
    }

    public class PaginatedComponentList
    {
        public List<Component> Components { get; set; }
        public IList<FacetPagination> Pagination { get; set; }
    }

    //public interface IFacetedContent
    //{
    //    PaginatedComponentList ComponentList { get; set; }
    //    FacetCollection FacetMap { get; set; }
    //    IComponentSearchSection ComponentSection { get; set; }
    //    string FredHopperLocation { get; set; }
    //    FacetSort Sort { get; set; }
    //}

    //public class FacetedContentBase<T> where T : IComponentSearchSection
    public abstract class FacetedContentBase
    {
        public PaginatedComponentList ComponentList { get; set; }
        public FacetCollection FacetMap { get; set; }
        public FASWebService.filter ReturnedSchemas { get; set; }
        public string FredHopperLocation { get; set; }
        public FacetSort Sort { get; set; }
        public abstract ComponentSearchSection ComponentSection { get; set; }

        // Done the old school way to ensure the field is initialised
        private Field _brand = new Field();
        public Field Brand
        {
            get
            {
                return _brand;
            }
            set
            {
                _brand = value;
            }
        }
    }

    public class FacetedContent : FacetedContentBase
    {
        private ComponentSearchSection _componentSection;
        public override ComponentSearchSection ComponentSection
        {
            get
            {
                if (_componentSection == null) { _componentSection = new ComponentSearchSection(); }
                return _componentSection;
            }
            set
            {
                _componentSection = value;
            }
        }
    }

    public class ProductExplorer : FacetedContentBase
    {
        private ExtComponentSearchSection _extComponentSection;
        public ExtComponentSearchSection ExtComponentSection 
        {
            get
            {
                if (_extComponentSection == null) { _extComponentSection = new ExtComponentSearchSection(); }
                return _extComponentSection;
            }
            set { _extComponentSection = value; }
        }
        public override ComponentSearchSection ComponentSection
        {
            get
            {
                return (ComponentSearchSection)ExtComponentSection;
            }
            set
            {
                ExtComponentSection.ComponentTypes = value.ComponentTypes;
                ExtComponentSection.SearchTerm = value.SearchTerm;
            }
        }
    }

    public class SegregatedContentItem
    {
        public string Title { get; set; }
        public string LocationString { get; set; }
        public int ViewSize { get; set; }
        public List<Component> Components { get; set; }
    }

    public class SegregatedContent
    {
        public Dictionary<string, SegregatedContentItem> SegregatedContentItems { get; set; }
        public ComponentSearchSection ComponentSection { get; set; }
    }

    public class FeaturedItems
    {
        public string Title { get; set; }
        public List<Component> Components { get; set; }
    }

    public class HomePageBannerContent
    {
        public List<Component> FredHopperComponents { get; set; }
        public IComponentPresentation Banner { get; set; }
        public List<SelectListItem> Tabs { get; set; }
    }
}