using System.Collections.Generic;

namespace Coats.Crafts.Configuration
{
    public class CountryConfig
    {
        protected Dictionary<string, CountrySectionElement> _countries;

        private static volatile CountryConfig instance;
        private static object syncRoot = new object();

        public static CountryConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new CountryConfig();
                        }
                    }
                }

                return instance;
            }
        }

        private CountryConfig()
        {
            _countries = new Dictionary<string, CountrySectionElement>();

            var sec = (CountryConfigSection)System.Configuration.ConfigurationManager.GetSection("CountryConfigSection");
            foreach (CountrySectionElement i in sec.Instances)
            {
                _countries.Add(i.Name, i);
            }
        }

        public Dictionary<string, CountrySectionElement> Countries
        {
            get
            {
                return _countries;
            }
        }

        //protected static Dictionary<string, CountrySectionElement> _instances;

        //static CountryConfig()
        //{
        //    _instances = new Dictionary<string, CountrySectionElement>();

        //    var sec = (CountryConfigSection)System.Configuration.ConfigurationManager.GetSection("CountryConfigSection");
        //    foreach (CountrySectionElement i in sec.Instances)
        //    {
        //        _instances.Add(i.Name, i);
        //    }
        //}

        //public static CountrySectionElement Instances(string instanceName)
        //{
        //    return _instances[instanceName];
        //}

        //public static Dictionary<string, CountrySectionElement> List()
        //{
        //    return _instances;
        //}

        //private CountryConfig()
        //{
        //}

    }
}