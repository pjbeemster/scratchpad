namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class SocialMediaLinkSectionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SocialMediaLinkSectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SocialMediaLinkSectionElement) element).Name;
        }
    }
}

