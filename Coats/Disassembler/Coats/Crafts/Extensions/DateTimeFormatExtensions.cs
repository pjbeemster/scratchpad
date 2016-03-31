namespace Coats.Crafts.Extensions
{
    using Coats.Crafts.Configuration;
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public static class DateTimeFormatExtensions
    {
        public static string CoatsDatePatternByTimezone(DateTime dateTime)
        {
            string timeZone = WebConfiguration.Current.TimeZone;
            return string.Format("{0}, {1}", TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern), TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern));
        }

        public static string CorrectedLongDatePattern(CultureInfo cultureInfo)
        {
            return Regex.Replace(cultureInfo.DateTimeFormat.LongDatePattern, "dddd,?", string.Empty).Trim();
        }
    }
}

