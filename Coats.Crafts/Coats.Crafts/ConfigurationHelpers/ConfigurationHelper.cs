using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Coats.Crafts.ConfigurationHelpers
{
    public static class ConfigurationHelper
    {
        public static bool Match(this string pattern, string setting)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
                return Regex.Match(setting.ToLower(), pattern.ToLower()).Success;
            }
            catch (Exception) 
            {
                return false;
            }
            
        }
    }
}