namespace Coats.Crafts.ControllerHelpers
{
    using Coats.Crafts.Models;
    using System;
    using System.Web.Mvc;

    public class UtilityHelper
    {
        public static DateTime FromUnixTime(long unixTime)
        {
            DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return time2.AddMilliseconds((double) unixTime).ToLocalTime();
        }

        public static TridionTcmUri GetTcmUri(string tcmUri)
        {
            string[] strArray = tcmUri.Split(new char[] { '-' });
            return new TridionTcmUri { 
                TcmPublicationID = Convert.ToUInt16(strArray[0].Replace("tcm:", "")),
                TcmItemId = Convert.ToInt32(strArray[1]),
                TcmItemType = (strArray.Length == 3) ? Convert.ToUInt16(strArray[2]) : 0x10,
                TcmId = tcmUri
            };
        }

        public static string WrapUrl(string url, string id)
        {
            TagBuilder builder = new TagBuilder("div");
            builder.GenerateId(id);
            builder.SetInnerText(url);
            return builder.ToString(TagRenderMode.Normal);
        }
    }
}

