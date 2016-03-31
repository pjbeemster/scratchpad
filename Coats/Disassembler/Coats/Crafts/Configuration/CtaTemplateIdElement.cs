namespace Coats.Crafts.Configuration
{
    using System;
    using System.Configuration;

    public class CtaTemplateIdElement : ConfigurationElement
    {
        private const string _ctaTemplateIdList = "CtaTemplateIdList";
        private const string _description = "description";
        private const string _tcmId = "tcmId";

        [ConfigurationProperty("description", IsRequired=false)]
        public string Description
        {
            get
            {
                return (string) base["description"];
            }
            set
            {
                base["description"] = value;
            }
        }

        [ConfigurationProperty("tcmId", IsKey=true, IsRequired=true)]
        public string TcmId
        {
            get
            {
                return (string) base["tcmId"];
            }
            set
            {
                base["tcmId"] = value;
            }
        }
    }
}

