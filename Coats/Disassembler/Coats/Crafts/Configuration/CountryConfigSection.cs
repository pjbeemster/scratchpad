namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CountryConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public CountrySectionCollection Instances
        {
            get
            {
                return (CountrySectionCollection) base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}

