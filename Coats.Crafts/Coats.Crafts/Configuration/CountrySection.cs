using System.Configuration;

namespace Coats.Crafts.Configuration
{
    public class CountryConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public CountrySectionCollection Instances
        {
            get { return (CountrySectionCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class CountrySectionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CountrySectionElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CountrySectionElement)element).Name;
        }
    }


    public class CountrySectionElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        [ConfigurationProperty("culture", IsRequired = true)]
        public string Culture
        {
            get { return (string)base["culture"]; }
            set { base["culture"] = value; }
        }
    }

}