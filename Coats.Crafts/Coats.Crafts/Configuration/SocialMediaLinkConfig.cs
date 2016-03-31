using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Configuration
{
    public class SocialMediaLinkConfig
    {
        protected Dictionary<string, SocialMediaLinkSectionElement> _socialmedia;

        private static volatile SocialMediaLinkConfig instance;
        private static object syncRoot = new object();

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

        private SocialMediaLinkConfig()
        {
            _socialmedia = new Dictionary<string, SocialMediaLinkSectionElement>();

            var sec = (SocialMediaLinkSection)System.Configuration.ConfigurationManager.GetSection("SocialMediaLinkSection");
            foreach (SocialMediaLinkSectionElement i in sec.Instances)
            {
                _socialmedia.Add(i.Name, i);
            }
        }

        public Dictionary<string, SocialMediaLinkSectionElement> SocialMedia
        {
            get
            {
                return _socialmedia;
            }
        }

        //protected static Dictionary<string, SocialMediaLinkSectionElement> _instances;

        //static SocialMediaLinkConfig()
        //{
        //    _instances = new Dictionary<string, SocialMediaLinkSectionElement>();

        //    var sec = (SocialMediaLinkSection)System.Configuration.ConfigurationManager.GetSection("SocialMediaLinkSection");
        //    foreach (SocialMediaLinkSectionElement i in sec.Instances)
        //    {
        //        _instances.Add(i.Name, i);
        //    }
        //}

        //public static SocialMediaLinkSectionElement Instances(string instanceName)
        //{
        //    return _instances[instanceName];
        //}

        //public static Dictionary<string, SocialMediaLinkSectionElement> List()
        //{
        //    return _instances;
        //}

        //private SocialMediaLinkConfig()
        //{
        //}
    }
}