using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Coats.Crafts.Configuration
{
    public class HomePageTabsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public HomePageTabsCollection Instances
        {
            get { return (HomePageTabsCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class HomePageTabsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new HomePageTabsElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HomePageTabsElement)element).Key;
        }
    }

    public class HomePageTabsElement : ConfigurationElement
    {
        // Using constants to cut down typos
        private const string _key = "key";
        //private const string _facet = "facet";
        private const string _dataFilter = "dataFilter";
        private const string _displayOrder = "displayOrder";
        private const string _tabText = "tabText";

        [ConfigurationProperty(_key, IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)base[_key]; }
            set { base[_key] = value; }
        }

        //[ConfigurationProperty(_facet, IsRequired = true)]
        //public string Facet
        //{
        //    get { return (string)base[_facet]; }
        //    set { base[_facet] = value; }
        //}

        [ConfigurationProperty(_dataFilter, IsRequired = true)]
        public string DataFilter
        {
            get { return (string)base[_dataFilter]; }
            set { base[_dataFilter] = value; }
        }

        [ConfigurationProperty(_displayOrder, IsRequired = true)]
        public int DisplayOrder
        {
            get { return (int)base[_displayOrder]; }
            set { base[_displayOrder] = value; }
        }

        [ConfigurationProperty(_tabText, IsRequired = true)]
        public string TabText
        {
            get { return (string)base[_tabText]; }
            set { base[_tabText] = value; }
        }

    }
}

