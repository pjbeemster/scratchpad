namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class HomePageTabsElement : ConfigurationElement
    {
        private const string _dataFilter = "dataFilter";
        private const string _displayOrder = "displayOrder";
        private const string _key = "key";
        private const string _tabText = "tabText";

        [ConfigurationProperty("dataFilter", IsRequired=true)]
        public string DataFilter
        {
            get
            {
                return (string) base["dataFilter"];
            }
            set
            {
                base["dataFilter"] = value;
            }
        }

        [ConfigurationProperty("displayOrder", IsRequired=true)]
        public int DisplayOrder
        {
            get
            {
                return (int) base["displayOrder"];
            }
            set
            {
                base["displayOrder"] = value;
            }
        }

        [ConfigurationProperty("key", IsKey=true, IsRequired=true)]
        public string Key
        {
            get
            {
                return (string) base["key"];
            }
            set
            {
                base["key"] = value;
            }
        }

        [ConfigurationProperty("tabText", IsRequired=true)]
        public string TabText
        {
            get
            {
                return (string) base["tabText"];
            }
            set
            {
                base["tabText"] = value;
            }
        }
    }
}

