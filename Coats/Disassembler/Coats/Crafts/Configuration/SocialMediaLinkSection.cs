namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class SocialMediaLinkSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public SocialMediaLinkSectionCollection Instances
        {
            get
            {
                return (SocialMediaLinkSectionCollection) base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}

