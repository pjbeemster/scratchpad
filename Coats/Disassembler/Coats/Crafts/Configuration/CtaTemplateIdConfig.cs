namespace Coats.Crafts.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Runtime.CompilerServices;

    public class CtaTemplateIdConfig
    {
        protected ConcurrentDictionary<string, CtaTemplateIdElement> _templates = new ConcurrentDictionary<string, CtaTemplateIdElement>();
        private static volatile CtaTemplateIdConfig instance;
        private static object syncRoot = new object();

        private CtaTemplateIdConfig()
        {
            CtaTemplateIdSection section = (CtaTemplateIdSection) ConfigurationManager.GetSection("CtaTemplateIdSection");
            foreach (CtaTemplateIdElement element in section.Instances)
            {
                this._templates.TryAdd(element.TcmId, element);
            }
        }

        public static CtaTemplateIdConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new CtaTemplateIdConfig();
                        }
                    }
                }
                return instance;
            }
        }

        public ConcurrentDictionary<string, CtaTemplateIdElement> Templates
        {
            get
            {
                return this._templates;
            }
        }
    }
}

