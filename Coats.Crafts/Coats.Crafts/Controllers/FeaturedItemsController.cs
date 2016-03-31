using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DD4T.ContentModel;
using Coats.Crafts.Models;
using com.fredhopper.lang.query;
using Coats.Crafts.Extensions;
using Coats.Crafts.FredHopper;
using Coats.Crafts.Configuration;

using Castle.Core.Logging;
using Coats.Crafts.CustomOutputCache;

namespace Coats.Crafts.Controllers
{
    [FredHopperOutputCache(CacheProfile = "FeaturedItems")]
    public class FeaturedItemsController : Controller
    {
        public ILogger Logger { get; set; }

        private readonly IAppSettings _settings;
        private const int MaxItems = 6;

        public FeaturedItemsController(IAppSettings settings)
        {
            _settings = settings;
        }

        [ChildActionOnly]
        public ActionResult Index(IComponentPresentation componentPresentation)
        {
             // Cold be id? IN web.config
            string schema = componentPresentation.Component.Schema.Title;

            IComponent component = componentPresentation.Component;
            FeaturedItems featuredItems = new FeaturedItems();

            if (schema == "Generic.ComponentGroup")
            {
                //featuredItems.Title = "Static Components [change me!]";
                featuredItems.Title = component.Fields.ContainsKey("title") ? component.Fields["title"].Value : string.Empty;
                
                // Statically assiged content
                IList<IComponent> components = component.Fields["items"].LinkedComponentValues;
                // ToComponentList()... too slow...???
                //featuredItems.Components = components.ToComponentList();

                // COATSCRAFTSWARRANTY-40
                // Cant just add components in directly - they have top be retrieved from FH else they dont have a rating or comment count!
                // Can't query by component id as this is not a live attribute (would reuiqre a complete reindex)
                // So nned to query by second id - which is component id + template id (which at this point we dont have, so had to create a hack mapping).
                featuredItems.Components = new List<Component>();
                
                if (components.Count > 0)
                {
                    // secondids look line - tcm_72-55498-16_tcm_72-19974-32
                    // DD4T component id not ccarry the 16 and the template id is gotten through the name/value object
                    string secondid = "fh_secondid={0}-16_tcm_{1}-{2}-32";
                    string query = string.Join("&", components.Select(c => 
                        string.Format(secondid, 
                            c.Id.Replace(":","_"),
                            WebConfiguration.Current.PublicationId,
                            SchemaTemplate.Instance.Template[c.Schema.Title])
                        ).ToArray());

                    // Execute query
                    DD4TComponents dd4t = new DD4TComponents(Logger);
                    Logger.InfoFormat("Featured static items query: {0}", query);

                    featuredItems.Components = dd4t.GetComponents(query, true);
                } 
            }
            else if (schema == "Crafts.ContentByFacet")
            {
                Query query = new Query();

                // Extract facets out of component
                component.Fields.ToList().ForEach(f =>
                {
                    if (f.Key == "title")
                    {
                        featuredItems.Title = f.Value.Value;
                    }
                    else if (f.Key == "facets" && f.Value.LinkedComponentValues.Count > 0)
                    {
                        int publicationId = _settings.PublicationId;
                        var fields = f.Value.LinkedComponentValues[0].Fields.Where(lcv => lcv.Key != "promote").ToFieldSet();
                        query.ParseQuery(fields, publicationId, MaxItems);
                        DD4TComponents dd4t = new DD4TComponents(Logger);

                        Logger.InfoFormat("Featured items query: {0}", query.toString());

                        featuredItems.Components = dd4t.GetComponents(query.toString(), true);
                    }
                                      
                });

            }
            else
            {
                // Log not allowed
                return View();
            }

            // Quick de-dupe fix so that we don't display an featured item component that is the same 
            // as the actual main page feature component.
            string featureComponentId = string.Empty;
            try { featureComponentId = componentPresentation.Page.ComponentPresentations[0].Component.Id; }
            catch { featureComponentId = string.Empty; }
            featuredItems.Components = featuredItems.Components.Where(c => c.Id != featureComponentId).ToList();

            return View(featuredItems);

        }

    }
}
