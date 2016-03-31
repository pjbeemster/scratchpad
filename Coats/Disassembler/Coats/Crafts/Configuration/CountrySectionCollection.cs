namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CountrySectionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CountrySectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CountrySectionElement) element).Name;
        }
    }
}

