namespace Coats.Crafts.Controllers
{
    using Castle.Core.Logging;
    using Coats.Crafts.Configuration;
    using Coats.Crafts.CustomOutputCache;
    using Coats.Crafts.Extensions;
    using Coats.Crafts.FredHopper;
    using Coats.Crafts.Models;
    using com.fredhopper.lang.query;
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    [FredHopperOutputCache(CacheProfile="FeaturedItems")]
    public class FeaturedItemsController : Controller
    {
        private readonly IAppSettings _settings;
        private const int MaxItems = 6;

        public FeaturedItemsController(IAppSettings settings)
        {
            this._settings = settings;
        }

        [ChildActionOnly]
        public ActionResult Index(IComponentPresentation componentPresentation)
        {
            string title = componentPresentation.Component.Schema.Title;
            IComponent component = componentPresentation.Component;
            FeaturedItems featuredItems = new FeaturedItems();
            if (title == "Generic.ComponentGroup")
            {
                featuredItems.Title = component.Fields.ContainsKey("title") ? component.Fields["title"].Value : string.Empty;
                IList<IComponent> linkedComponentValues = component.Fields["items"].LinkedComponentValues;
                featuredItems.Components = new List<Component>();
                if (linkedComponentValues.Count > 0)
                {
                    string secondid = "fh_secondid={0}-16_tcm_{1}-{2}-32";
                    string str2 = string.Join("&", (from c in linkedComponentValues select string.Format(secondid, c.Id.Replace(":", "_"), WebConfiguration.Current.PublicationId, SchemaTemplate.Instance.Template[c.Schema.Title])).ToArray<string>());
                    DD4TComponents components = new DD4TComponents(this.Logger);
                    this.Logger.InfoFormat("Featured static items query: {0}", new object[] { str2 });
                    featuredItems.Components = components.GetComponents(str2, true);
                }
            }
            else if (title == "Crafts.ContentByFacet")
            {
                Query query = new Query();
                component.Fields.ToList<KeyValuePair<string, IField>>().ForEach(delegate (KeyValuePair<string, IField> f) {
                    if (f.Key == "title")
                    {
                        featuredItems.Title = f.Value.Value;
                    }
                    else if ((f.Key == "facets") && (f.Value.LinkedComponentValues.Count > 0))
                    {
                        int publicationId = this._settings.PublicationId;
                        IFieldSet fieldSet = (from lcv in f.Value.LinkedComponentValues[0].Fields
                            where lcv.Key != "promote"
                            select lcv).ToFieldSet();
                        query.ParseQuery(fieldSet, publicationId, 6);
                        DD4TComponents components = new DD4TComponents(this.Logger);
                        this.Logger.InfoFormat("Featured items query: {0}", new object[] { query.toString() });
                        featuredItems.Components = components.GetComponents(query.toString(), true);
                    }
                });
            }
            else
            {
                return base.View();
            }
            string featureComponentId = string.Empty;
            try
            {
                featureComponentId = componentPresentation.Page.ComponentPresentations[0].Component.Id;
            }
            catch
            {
                featureComponentId = string.Empty;
            }
            featuredItems.Components = (from c in featuredItems.Components
                where c.Id != featureComponentId
                select c).ToList<Component>();
            return base.View(featuredItems);
        }

        public ILogger Logger { get; set; }
    }
}

