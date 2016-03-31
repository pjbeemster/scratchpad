namespace Coats.Crafts.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Runtime.CompilerServices;

    public class SchemaTemplate
    {
        private static volatile SchemaTemplate instance;
        private static object syncRoot = new object();
        protected NameValueCollection template = (ConfigurationManager.GetSection("SchemaTemplate") as NameValueCollection);

        private SchemaTemplate()
        {
            if (this.template == null)
            {
                this.template = new NameValueCollection();
            }
        }

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

        public NameValueCollection Template
        {
            get
            {
                return this.template;
            }
        }
    }
}

