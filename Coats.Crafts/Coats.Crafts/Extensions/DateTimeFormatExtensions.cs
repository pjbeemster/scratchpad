using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Coats.Crafts.Configuration;
using System.Globalization;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Coats.Crafts.Extensions
{
    public static class DateTimeFormatExtensions
    {
		public static string CorrectedLongDatePattern(CultureInfo cultureInfo)
		{
			var info = cultureInfo.DateTimeFormat;
			return Regex.Replace(info.LongDatePattern, "dddd,?", String.Empty).Trim();
		}

		public static string CoatsDatePatternByTimezone(DateTime dateTime)
		{
			string timeZone = WebConfiguration.Current.TimeZone;

			string date = string.Format("{0}, {1}", TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern), TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern));

			return date;
		}
    }
}