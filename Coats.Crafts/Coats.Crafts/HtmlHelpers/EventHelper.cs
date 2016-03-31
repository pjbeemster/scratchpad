using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DD4T.ContentModel;
using Coats.Crafts.Resources;
using Coats.Crafts.HtmlHelpers;
using Coats.Crafts.Extensions;
using Coats.Crafts.Models;
using System.Globalization;
using Coats.Crafts.Configuration;


//***
//THE Tridion Calendar does not allow the Content Editor to only select date and not time
//A flag allows the Site Editor to say only date
//The logic for the evnt component considers the followin;
// Case 1 - Start Date / End Date combo

//Start date (time - no )
//Start date (time - yes) 
//Start date (time - no ) - End date (time - no)
//Start date (time - no ) - End date (time - yes)
//Start date (time - yes) - End date (time - no)
//Start date (time - yes) - End date (time - yes)

//case 2 - Recurring date
//Day (time - no )
//Day (time - yes) 
//Day (time - no ) - until date (time - no)
//Day (time - no ) - until date (time - yes)
//Day (time - yes ) - until date (time - no)
//Day (time - yes ) - until date (time - yes)

//**/
namespace Coats.Crafts.HtmlHelpers
{
    public static class EventHelper
    {
		
        //
		// GET: /GetEventsNearYou/
		public static MvcHtmlString GetEvent(this HtmlHelper helper, IComponentPresentation component)
        {
			var fields = component.Component.Fields;

			var dates = fields.ContainsKey("date") ? fields["date"].EmbeddedValues[0] : null;
			var specificDate = dates.ContainsKey("specific") ? dates["specific"] : null;

			DateTime specificStartDate = specificDate.EmbeddedValues[0].ContainsKey("start") ? @specificDate.EmbeddedValues[0]["start"].DateTimeValues[0] :new DateTime();
			DateTime specificEndDate = specificDate.EmbeddedValues[0].ContainsKey("end") ? @specificDate.EmbeddedValues[0]["end"].DateTimeValues[0] : new DateTime();

			string eventStr = string.Empty;
			string sDate = string.Empty;
			string eDate = string.Empty;
			string totDate = string.Empty;

			var sep = ResourceHelper.GetResource(helper, "TimeSeperator");
			var dsep = ResourceHelper.GetResource(helper, "DateSeperator");
			var and = ResourceHelper.GetResource(helper, "and");

			if (specificStartDate != DateTime.MinValue)
			{
				bool useStartTime = specificDate.EmbeddedValues[0]["use_start_time"].Values[0] == "Yes"? true : false;
				bool useEndTime = specificDate.EmbeddedValues[0]["use_end_time"].Values[0] == "Yes" ? true : false;

				try
				{
					if (specificStartDate != DateTime.MinValue)
					{
						sDate = String.Format(ResourceHelper.GetResource(helper, "StartsOn"), SetDateStringFormatted(specificStartDate, useStartTime, ResourceHelper.GetResource(helper, "at")));
					}
				}
				catch (Exception ex)
				{
				}
				try
				{
					if (specificEndDate != DateTime.MinValue)
					{
						eDate = String.Format(ResourceHelper.GetResource(helper, "EndsOn"), SetDateStringFormatted(specificEndDate, useEndTime, ResourceHelper.GetResource(helper, "at")));
					}
				}
				catch
				{
				}

				eventStr = SetTotalString(sDate, eDate, and);


			} else {

				//recurring
				var repeaterDate = dates.ContainsKey("repeat") ? dates["repeat"] : null;
				string recurringDays = string.Join(",", repeaterDate.EmbeddedValues[0]["day"].Values.ToList());
				DateTime times = repeaterDate.EmbeddedValues[0].ContainsKey("time") ? repeaterDate.EmbeddedValues[0]["time"].DateTimeValues[0] : new DateTime();
				DateTime recurUntil = repeaterDate.EmbeddedValues[0].ContainsKey("until") ? repeaterDate.EmbeddedValues[0]["until"].DateTimeValues[0] : new DateTime();

				bool useEndTime = repeaterDate.EmbeddedValues[0]["use_until_time"].Values[0] == "Yes" ? true : false;

				if (repeaterDate != null)
				{
					if (times != DateTime.MinValue)
					{
						eventStr = SetRecurring(recurringDays, times.TimeOfDay, sep, recurUntil, useEndTime,ResourceHelper.GetResource(helper, "until"));
					} else {
						eventStr = SetRecurring(recurringDays, null, sep, recurUntil, useEndTime, ResourceHelper.GetResource(helper, "until"));
					}
					eventStr = String.Format("{0} {1} ", ResourceHelper.GetResource(helper,"RepeatsEvery"), eventStr);
				}

			}

			return new MvcHtmlString(eventStr);
		}

		public static MvcHtmlString GetEventsNearYou(this HtmlHelper helper, EventsNearYou Model, int eventId)
        {
			DateTime EpochDateTime =  new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			string eventStr = string.Empty;
			string sDate = string.Empty;
			string eDate = string.Empty;
			string totDate = string.Empty;

			var sep = ResourceHelper.GetResource(helper, "TimeSeperator");
			var dsep = ResourceHelper.GetResource(helper,"DateSeperator");

			if (Model.Events[eventId].EventDateStart.ToShortDateString() != DateTime.MinValue.ToShortDateString())
			{
				try
				{
					if (Model.Events[eventId].EventDateStart.ToShortDateString() != EpochDateTime.ToShortDateString())
					{
						sDate = SetDateStringFormatted(Model.Events[eventId].EventDateStart, Model.Events[eventId].UseStartTime, sep);
					}
				}
				catch
				{
				}
				try
				{
					if (Model.Events[eventId].EventDateEnd.ToShortDateString() != EpochDateTime.ToShortDateString())
					{
						eDate = SetDateStringFormatted(Model.Events[eventId].EventDateEnd, Model.Events[eventId].UseEndTime, sep);
					}
				}
				catch
				{
				}
				
				eventStr = SetTotalString(sDate, eDate, dsep);

			} else {
				bool useEndTime = Model.Events[eventId].UseEndTime;
				eventStr = SetRecurring(Model.Events[eventId].RecuringDays, Model.Events[eventId].RecuringAt, sep, Model.Events[eventId].EventDateEnd, useEndTime, "until");
			}

			return new MvcHtmlString(eventStr);
        }


		private static string SetDateStringFormatted(DateTime dateTime, bool useTime, String dateTimeSep)
		{
			try
			{
				string outString = string.Empty;
				//Check if epoch start date
				if (dateTime.Year != DateTime.MinValue.Year)
				{
					if (useTime)
					{
						
						if (String.IsNullOrEmpty(dateTimeSep))
						{
							outString = dateTime.DayOfWeek.ToString() + ", " + dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern);

						}
						else
						{
							outString = String.Format("{0}, {1} {2} {3}", dateTime.DayOfWeek.ToString(), dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortDatePattern), dateTimeSep, dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern));

						}
					}
					else
					{
						outString = dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortDatePattern);
					}
				}
				return outString;
			}
			catch
			{
				return string.Empty;
			}
		}

		

		private static string SetTotalString(string starttime, string endtime, string sep)
		{
			string outdate = string.Empty;
			if (!string.IsNullOrEmpty(starttime))
			{
				outdate = starttime;
			}
			if (!string.IsNullOrEmpty(endtime))
			{
				outdate = outdate + " " + sep + " " + endtime;
			}
			return outdate;
		}

		private static string SetRecurring(string inlist, TimeSpan? recurringAt, string sep, DateTime untilEndDate, bool useEndTime, string endSep)
		{		
			string outString = string.Empty;
			string atString = string.Empty;
			string delim = string.Empty;

			if (!string.IsNullOrEmpty(inlist))
			{
				//Need sort the days of the week as the Tridion weekday keywords aren't sorted - nice
				DayOfWeek firstDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
				for (int dayIndex = 0; dayIndex < 7; dayIndex++)
				{
					var currentDay = (DayOfWeek)(((int)firstDay + dayIndex) % 7);

					if (inlist.ToLower().IndexOf(currentDay.ToString().ToLower(), 0) >= 0)
					{
						// Output the day
						outString += delim + currentDay;
						delim = ", ";
					}
				}
			}

			if (recurringAt != null || recurringAt != TimeSpan.MinValue)
			{
				if (recurringAt.HasValue)
				{
					atString = sep + " " + new DateTime(recurringAt.Value.Ticks).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern);
				}
				outString = outString + " " + atString;
			}


			if (untilEndDate != DateTime.MinValue)
			{
				if (useEndTime) {
					outString = outString + " " + endSep + " " + untilEndDate.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.FullDateTimePattern);
				} else {
					outString = outString + " " + endSep + " " + untilEndDate.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern);
				}
			}
			return outString;
		}

    }
}
