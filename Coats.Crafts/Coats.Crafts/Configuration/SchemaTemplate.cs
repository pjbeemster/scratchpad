using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;

namespace Coats.Crafts.Configuration
{
    public class SchemaTemplate
    {
        protected NameValueCollection template;

        private static volatile SchemaTemplate instance;
        private static object syncRoot = new object();

        public static SchemaTemplate Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new SchemaTemplate();
                        }
                    }
                }

                return instance;
            }
        }

        private SchemaTemplate()
        {
            template = ConfigurationManager.GetSection("SchemaTemplate") as NameValueCollection;
            if (template == null)
            {
                template = new NameValueCollection();
            }
        }

        public NameValueCollection Template
        {
            get
            {
                return template;
            }
        }
    }
}