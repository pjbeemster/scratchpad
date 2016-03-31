using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Configuration;
using System.Text;

namespace Coats.Crafts.Configuration
{
    public static class FredHopperExtendedProperties
    {
        static Hashtable _settings;

        /// <summary>
        /// Returns the current configuration settings for this web site.
        /// </summary>
        public static Hashtable Current
        {
            get
            {
                if (_settings == null)
                {
                    try
                    {
                        _settings = (Hashtable)ConfigurationManager.GetSection("FredHopperExtendedProperties");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Missing or erroneous FredHopperExtendedProperties section in web.config", ex);
                    }
                }
                return _settings;
            }
        }

        public static string ToCommaDel(this Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var val in ht.Values)
            {
                if (sb.Length > 0) { sb.Append(","); }
                sb.Append(val.ToString());
            }
            return sb.ToString();
        }

    }
}