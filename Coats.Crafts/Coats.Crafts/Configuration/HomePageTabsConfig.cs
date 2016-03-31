using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Models;
using System.Text;

namespace Coats.Crafts.Configuration
{
    public class HomePageTabsConfig
    {
        protected Dictionary<string, HomePageTabsElement> _tabs;

        private static volatile HomePageTabsConfig instance;
        private static object syncRoot = new object();

        public static HomePageTabsConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new HomePageTabsConfig();
                        }
                    }
                }

                return instance;
            }
        }

        private HomePageTabsConfig()
        {
            _tabs = new Dictionary<string, HomePageTabsElement>();

            var sec = (HomePageTabsSection)System.Configuration.ConfigurationManager.GetSection("HomePageTabsSection");
            foreach (HomePageTabsElement i in sec.Instances)
            {
                _tabs.Add(i.Key, i);
            }
        }

        public Dictionary<string, HomePageTabsElement> Tabs
        {
            get
            {
                return _tabs.OrderBy(i => i.Value.DisplayOrder).ToDictionary(d => d.Key, d => d.Value);
            }
        }


        // Using constants to cut down typos
        //private const string _homePageTabsSection = "HomePageTabsSection";

        //protected static Dictionary<string, HomePageTabsElement> _instances;

        //static HomePageTabsConfig()
        //{
        //    _instances = new Dictionary<string, HomePageTabsElement>();

        //    var sec = (HomePageTabsSection)System.Configuration.ConfigurationManager.GetSection(_homePageTabsSection);
        //    foreach (HomePageTabsElement i in sec.Instances)
        //    {
        //        _instances.Add(i.Key, i);
        //    }
        //}

        //public static HomePageTabsElement Instances(string instanceName)
        //{
        //    return _instances[instanceName];
        //}

        //public static Dictionary<string, HomePageTabsElement> List()
        //{
        //    return _instances.OrderBy(i => i.Value.DisplayOrder).ToDictionary(d => d.Key, d => d.Value);
        //}

        public static List<SelectListItem> ToSelectList()
        {
            return ToSelectList(string.Empty);
        }

        public static List<SelectListItem> ToSelectList(string container)
        {
            var config = HomePageTabsConfig.Instance;
            var tabs = config.Tabs;

            List<SelectListItem> items = new List<SelectListItem>();
            //foreach (var de in HomePageTabsConfig.List())
            foreach (var de in tabs)
            {
                //items.Add(new SelectListItem { Text = de.Key.ToString(), Value = de.Value.DataFilter.ToString(), Selected = (de.Value.DisplayOrder == 1) });
                items.Add(new SelectListItem { Text = de.Value.TabText.ToString(), Value = de.Value.DataFilter.ToString(), Selected = (de.Value.DisplayOrder == 1) });
            }
            return items;
        }

        public static com.fredhopper.lang.query.location.criteria.Criterion ToCriteria(int publicationId)
        {
            var config = HomePageTabsConfig.Instance;
            var tabs = config.Tabs;

            // NOTE:    THIS IS A VERY QUICK FIX THAT RELIES ON THE WEB CONFIG SETTINGS FOR THE TABS BEING PURELY FOR TECHNIQUES.
            // Why?:    Well, it's late at night and my brain is frazzled! 
            string facetType = string.Format("{0}_techniques>{{", publicationId);
            StringBuilder sb = new StringBuilder(facetType);

            //foreach (var de in HomePageTabsConfig.List())
            foreach (var de in tabs)
            {
                if (sb.Length > facetType.Length) { sb.Append(";"); }
                sb.Append(de.Key.ToString());
            }
            sb.Append("}");

            return com.fredhopper.lang.query.location.criteria.CriterionFactory.parse(sb.ToString());
        }

        //private HomePageTabsConfig()
        //{
        //}
    }

}
