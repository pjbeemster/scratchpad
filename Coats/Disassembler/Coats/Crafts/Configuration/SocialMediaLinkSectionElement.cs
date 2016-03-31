namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class SocialMediaLinkSectionElement : ConfigurationElement
    {
        [ConfigurationProperty("class", IsRequired=true)]
        public string Class
        {
            get
            {
                return (string) base["class"];
            }
            set
            {
                base["class"] = value;
            }
        }

        [ConfigurationProperty("href", IsKey=true, IsRequired=true)]
        public string Href
        {
            get
            {
                return (string) base["href"];
            }
            set
            {
                base["href"] = value;
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

        [ConfigurationProperty("title", IsRequired=true)]
        public string Title
        {
            get
            {
                return (string) base["title"];
            }
            set
            {
                base["title"] = value;
            }
        }
    }
}

