using System.Configuration;

namespace Coats.Crafts.Configuration
{
    public class SocialMediaLinkSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public SocialMediaLinkSectionCollection Instances
        {
            get { return (SocialMediaLinkSectionCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class SocialMediaLinkSectionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SocialMediaLinkSectionElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SocialMediaLinkSectionElement)element).Name;
        }
    }


    public class SocialMediaLinkSectionElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("href", IsKey = true, IsRequired = true)]
        public string Href
        {
            get { return (string)base["href"]; }
            set { base["href"] = value; }
        }

        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get { return (string)base["title"]; }
            set { base["title"] = value; }
        }

        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get { return (string)base["class"]; }
            set { base["class"] = value; }
        }
    }
}