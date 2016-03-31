namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class HomePageTabsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public HomePageTabsCollection Instances
        {
            get
            {
                return (HomePageTabsCollection) base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}

