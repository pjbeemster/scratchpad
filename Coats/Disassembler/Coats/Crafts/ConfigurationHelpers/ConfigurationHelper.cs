namespace Coats.Crafts.ConfigurationHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

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

