namespace Coats.Crafts.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.CompilerServices;

    public class CountryConfig
    {
        protected Dictionary<string, CountrySectionElement> _countries = new Dictionary<string, CountrySectionElement>();
        private static volatile CountryConfig instance;
        private static object syncRoot = new object();

        private CountryConfig()
        {
            CountryConfigSection section = (CountryConfigSection) ConfigurationManager.GetSection("CountryConfigSection");
            foreach (CountrySectionElement element in section.Instances)
            {
                this._countries.Add(element.Name, element);
            }
        }

        public Dictionary<string, CountrySectionElement> Countries
        {
            get
            {
                return this._countries;
            }
        }

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
    }
}

