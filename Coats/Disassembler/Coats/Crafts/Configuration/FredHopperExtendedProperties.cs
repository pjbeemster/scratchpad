namespace Coats.Crafts.Configuration
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class FredHopperExtendedProperties
    {
        private static Hashtable _settings;

        public static string ToCommaDel(this Hashtable ht)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in ht.Values)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(obj2.ToString());
            }
            return builder.ToString();
        }

        public static Hashtable Current
        {
            get
            {
                if (_settings == null)
                {
                    try
                    {
                        _settings = (Hashtable) ConfigurationManager.GetSection("FredHopperExtendedProperties");
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Missing or erroneous FredHopperExtendedProperties section in web.config", exception);
                    }
                }
                return _settings;
            }
        }
    }
}

