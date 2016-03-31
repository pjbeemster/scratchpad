namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CtaTemplateIdSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public CtaTemplateIdCollection Instances
        {
            get
            {
                return (CtaTemplateIdCollection) base[""];
            }
            set
            {
                base[""] = value;
            }
        }
    }
}

