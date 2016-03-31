namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class HomePageTabsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new HomePageTabsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HomePageTabsElement) element).Key;
        }
    }
}

