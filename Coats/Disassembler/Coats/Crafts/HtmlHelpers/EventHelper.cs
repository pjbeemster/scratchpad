namespace Coats.Crafts.HtmlHelpers
{
    using Coats.Crafts.Configuration;
    using Coats.Crafts.Models;
    using DD4T.ContentModel;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public static class EventHelper
    {
        public static MvcHtmlString GetEvent(this HtmlHelper helper, IComponentPresentation component)
        {
            bool flag2;
            DateTime time5;
            IFieldSet fields = component.Component.Fields;
            IFieldSet set2 = fields.ContainsKey("date") ? fields["date"].EmbeddedValues[0] : null;
            IField field = set2.ContainsKey("specific") ? set2["specific"] : null;
            DateTime dateTime = field.EmbeddedValues[0].ContainsKey("start") ? field.EmbeddedValues[0]["start"].DateTimeValues[0] : (time5 = new DateTime());
            DateTime time2 = field.EmbeddedValues[0].ContainsKey("end") ? field.EmbeddedValues[0]["end"].DateTimeValues[0] : (time5 = new DateTime());
            string str = string.Empty;
            string starttime = string.Empty;
            string endtime = string.Empty;
            string resource = helper.GetResource("TimeSeperator");
            string str6 = helper.GetResource("DateSeperator");
            string sep = helper.GetResource("and");
            if (dateTime != DateTime.MinValue)
            {
                bool useTime = field.EmbeddedValues[0]["use_start_time"].Values[0] == "Yes";
                flag2 = field.EmbeddedValues[0]["use_end_time"].Values[0] == "Yes";
                try
                {
                    if (dateTime != DateTime.MinValue)
                    {
                        starttime = string.Format(helper.GetResource("StartsOn"), SetDateStringFormatted(dateTime, useTime, helper.GetResource("at")));
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    if (time2 != DateTime.MinValue)
                    {
                        endtime = string.Format(helper.GetResource("EndsOn"), SetDateStringFormatted(time2, flag2, helper.GetResource("at")));
                    }
                }
                catch
                {
                }
                str = SetTotalString(starttime, endtime, sep);
            }
            else
            {
                IField field2 = set2.ContainsKey("repeat") ? set2["repeat"] : null;
                string inlist = string.Join(",", field2.EmbeddedValues[0]["day"].Values.ToList<string>());
                DateTime time3 = field2.EmbeddedValues[0].ContainsKey("time") ? field2.EmbeddedValues[0]["time"].DateTimeValues[0] : (time5 = new DateTime());
                DateTime untilEndDate = field2.EmbeddedValues[0].ContainsKey("until") ? field2.EmbeddedValues[0]["until"].DateTimeValues[0] : new DateTime();
                flag2 = field2.EmbeddedValues[0]["use_until_time"].Values[0] == "Yes";
                if (field2 != null)
                {
                    if (time3 != DateTime.MinValue)
                    {
                        str = SetRecurring(inlist, new TimeSpan?(time3.TimeOfDay), resource, untilEndDate, flag2, helper.GetResource("until"));
                    }
                    else
                    {
                        str = SetRecurring(inlist, null, resource, untilEndDate, flag2, helper.GetResource("until"));
                    }
                    str = string.Format("{0} {1} ", helper.GetResource("RepeatsEvery"), str);
                }
            }
            return new MvcHtmlString(str);
        }

        public static MvcHtmlString GetEventsNearYou(this HtmlHelper helper, EventsNearYou Model, int eventId)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string str = string.Empty;
            string starttime = string.Empty;
            string endtime = string.Empty;
            string resource = helper.GetResource("TimeSeperator");
            string sep = helper.GetResource("DateSeperator");
            if (Model.Events[eventId].EventDateStart.ToShortDateString() != DateTime.MinValue.ToShortDateString())
            {
                try
                {
                    if (Model.Events[eventId].EventDateStart.ToShortDateString() != time.ToShortDateString())
                    {
                        starttime = SetDateStringFormatted(Model.Events[eventId].EventDateStart, Model.Events[eventId].UseStartTime, resource);
                    }
                }
                catch
                {
                }
                try
                {
                    if (Model.Events[eventId].EventDateEnd.ToShortDateString() != time.ToShortDateString())
                    {
                        endtime = SetDateStringFormatted(Model.Events[eventId].EventDateEnd, Model.Events[eventId].UseEndTime, resource);
                    }
                }
                catch
                {
                }
                str = SetTotalString(starttime, endtime, sep);
            }
            else
            {
                bool useEndTime = Model.Events[eventId].UseEndTime;
                str = SetRecurring(Model.Events[eventId].RecuringDays, new TimeSpan?(Model.Events[eventId].RecuringAt), resource, Model.Events[eventId].EventDateEnd, useEndTime, "until");
            }
            return new MvcHtmlString(str);
        }

        private static string SetDateStringFormatted(DateTime dateTime, bool useTime, string dateTimeSep)
        {
            try
            {
                string str = string.Empty;
                if (dateTime.Year != DateTime.MinValue.Year)
                {
                    if (useTime)
                    {
                        if (string.IsNullOrEmpty(dateTimeSep))
                        {
                            str = dateTime.DayOfWeek.ToString() + ", " + dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern);
                        }
                        else
                        {
                            str = string.Format("{0}, {1} {2} {3}", new object[] { dateTime.DayOfWeek.ToString(), dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortDatePattern), dateTimeSep, dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern) });
                        }
                    }
                    else
                    {
                        str = dateTime.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortDatePattern);
                    }
                }
                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SetRecurring(string inlist, TimeSpan? recurringAt, string sep, DateTime untilEndDate, bool useEndTime, string endSep)
        {
            TimeSpan? nullable;
            string str = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            if (!string.IsNullOrEmpty(inlist))
            {
                DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                for (int i = 0; i < 7; i++)
                {
                    DayOfWeek week2 = (firstDayOfWeek + i) % (DayOfWeek.Saturday | DayOfWeek.Monday);
                    if (inlist.ToLower().IndexOf(week2.ToString().ToLower(), 0) >= 0)
                    {
                        str = str + str3 + week2;
                        str3 = ", ";
                    }
                }
            }
            if (recurringAt.HasValue || (!(nullable = recurringAt).HasValue || (nullable.GetValueOrDefault() != TimeSpan.MinValue)))
            {
                if (recurringAt.HasValue)
                {
                    str2 = sep + " " + new DateTime(recurringAt.Value.Ticks).ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.ShortTimePattern);
                }
                str = str + " " + str2;
            }
            if (!(untilEndDate != DateTime.MinValue))
            {
                return str;
            }
            if (useEndTime)
            {
                return (str + " " + endSep + " " + untilEndDate.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.FullDateTimePattern));
            }
            return (str + " " + endSep + " " + untilEndDate.ToString(CultureInfo.CreateSpecificCulture(WebConfiguration.Current.Culture).DateTimeFormat.LongDatePattern));
        }

        private static string SetTotalString(string starttime, string endtime, string sep)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(starttime))
            {
                str = starttime;
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                str = str + " " + sep + " " + endtime;
            }
            return str;
        }
    }
}

