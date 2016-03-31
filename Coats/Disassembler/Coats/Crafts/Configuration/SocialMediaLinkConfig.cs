namespace Coats.Crafts.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.CompilerServices;

    public class SocialMediaLinkConfig
    {
        protected Dictionary<string, SocialMediaLinkSectionElement> _socialmedia = new Dictionary<string, SocialMediaLinkSectionElement>();
        private static volatile SocialMediaLinkConfig instance;
        private static object syncRoot = new object();

        private SocialMediaLinkConfig()
        {
            SocialMediaLinkSection section = (SocialMediaLinkSection) ConfigurationManager.GetSection("SocialMediaLinkSection");
            foreach (SocialMediaLinkSectionElement element in section.Instances)
            {
                this._socialmedia.Add(element.Name, element);
            }
        }

        public static SocialMediaLinkConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SocialMediaLinkConfig();
                        }
                    }
                }
                return instance;
            }
        }

        public Dictionary<string, SocialMediaLinkSectionElement> SocialMedia
        {
            get
            {
                return this._socialmedia;
            }
        }
    }
}

