namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CountrySectionElement : ConfigurationElement
    {
        [ConfigurationProperty("culture", IsRequired=true)]
        public string Culture
        {
            get
            {
                return (string) base["culture"];
            }
            set
            {
                base["culture"] = value;
            }
        }

        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name
        {
            get
            {
                return (string) base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("url", IsRequired=true)]
        public string Url
        {
            get
            {
                return (string) base["url"];
            }
            set
            {
                base["url"] = value;
            }
        }
    }
}

