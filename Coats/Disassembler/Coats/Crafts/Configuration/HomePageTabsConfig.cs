namespace Coats.Crafts.Configuration
{
    using com.fredhopper.lang.query.location.criteria;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web.Mvc;

    public class HomePageTabsConfig
    {
        protected Dictionary<string, HomePageTabsElement> _tabs = new Dictionary<string, HomePageTabsElement>();
        private static volatile HomePageTabsConfig instance;
        private static object syncRoot = new object();

        private HomePageTabsConfig()
        {
            HomePageTabsSection section = (HomePageTabsSection) ConfigurationManager.GetSection("HomePageTabsSection");
            foreach (HomePageTabsElement element in section.Instances)
            {
                this._tabs.Add(element.Key, element);
            }
        }

        public static Criterion ToCriteria(int publicationId)
        {
            Dictionary<string, HomePageTabsElement> tabs = Instance.Tabs;
            string str = string.Format("{0}_techniques>{{", publicationId);
            StringBuilder builder = new StringBuilder(str);
            foreach (KeyValuePair<string, HomePageTabsElement> pair in tabs)
            {
                if (builder.Length > str.Length)
                {
                    builder.Append(";");
                }
                builder.Append(pair.Key.ToString());
            }
            builder.Append("}");
            return CriterionFactory.parse(builder.ToString());
        }

        public static List<SelectListItem> ToSelectList()
        {
            return ToSelectList(string.Empty);
        }

        public static List<SelectListItem> ToSelectList(string container)
        {
            Dictionary<string, HomePageTabsElement> tabs = Instance.Tabs;
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (KeyValuePair<string, HomePageTabsElement> pair in tabs)
            {
                SelectListItem item = new SelectListItem {
                    Text = pair.Value.TabText.ToString(),
                    Value = pair.Value.DataFilter.ToString(),
                    Selected = pair.Value.DisplayOrder == 1
                };
                list.Add(item);
            }
            return list;
        }

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

        public Dictionary<string, HomePageTabsElement> Tabs
        {
            get
            {
                return (from i in this._tabs
                    orderby i.Value.DisplayOrder
                    select i).ToDictionary<KeyValuePair<string, HomePageTabsElement>, string, HomePageTabsElement>(d => d.Key, d => d.Value);
            }
        }
    }
}

