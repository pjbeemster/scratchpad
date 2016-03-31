using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Configuration
{
    /// <summary>
    /// updated as per http://msdn.microsoft.com/en-us/library/ff650316.aspx
    /// </summary>
    public class CtaTemplateIdConfig
    {
        // Using constants to cut down typos
        //private const string _ctaTemplateIdSection = "CtaTemplateIdSection";

        //protected static Dictionary<string, CtaTemplateIdElement> _instances;
        //protected Dictionary<string, CtaTemplateIdElement> _templates;
        protected ConcurrentDictionary<string, CtaTemplateIdElement> _templates;

        private static volatile CtaTemplateIdConfig instance;
        private static object syncRoot = new object();

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

        private CtaTemplateIdConfig()
        {
            _templates = new ConcurrentDictionary<string, CtaTemplateIdElement>();

            var sec = (CtaTemplateIdSection)System.Configuration.ConfigurationManager.GetSection("CtaTemplateIdSection");
            foreach (CtaTemplateIdElement i in sec.Instances)
            {
                _templates.TryAdd(i.TcmId, i);
            }
        }

        public ConcurrentDictionary<string, CtaTemplateIdElement> Templates
        {
            get
            {
                return _templates;
            }
        }

        //static CtaTemplateIdConfig()
        //{
        //    _instances = new Dictionary<string, CtaTemplateIdElement>();

        //    var sec = (CtaTemplateIdSection)System.Configuration.ConfigurationManager.GetSection(_ctaTemplateIdSection);
        //    foreach (CtaTemplateIdElement i in sec.Instances)
        //    {
        //        _instances.Add(i.TcmId, i);
        //    }
        //}

        //public static CtaTemplateIdElement Instances(string instanceName)
        //{
        //    return _instances[instanceName];
        //}

        //public static Dictionary<string, CtaTemplateIdElement> List()
        //{
        //    return _instances;
        //}
    }
}