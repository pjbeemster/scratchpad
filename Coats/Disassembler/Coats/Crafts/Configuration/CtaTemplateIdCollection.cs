namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CtaTemplateIdCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CtaTemplateIdElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CtaTemplateIdElement) element).TcmId;
        }
    }
}

