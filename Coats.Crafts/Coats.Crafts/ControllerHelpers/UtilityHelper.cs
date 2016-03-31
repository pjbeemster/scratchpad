using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Models;
using System.Web.Mvc;

namespace Coats.Crafts.ControllerHelpers
{
    public class UtilityHelper
    {

        /// <summary>
        /// Converts the unix time to a local time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTime).ToLocalTime();
        }


        /// <summary>
        /// Gets the TCM URI.
        /// </summary>
        /// <param name="tcmUri">The TCM URI.</param>
        /// <returns></returns>
        public static TridionTcmUri GetTcmUri(string tcmUri)
        {
            string[] tcm = tcmUri.Split('-');

            TridionTcmUri tcmuri = new TridionTcmUri();
            tcmuri.TcmPublicationID = Convert.ToUInt16(tcm[0].Replace("tcm:", ""));
            tcmuri.TcmItemId = Convert.ToInt32(tcm[1]);

            tcmuri.TcmItemType = tcm.Length == 3 ? Convert.ToUInt16(tcm[2]) : 16;

            tcmuri.TcmId = tcmUri;

            return tcmuri;


        }


        /// <summary>
        /// Wrap a url in <div> </div> with id = passed id to send back in ajax request
        /// to allow Jquery to find it
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string WrapUrl(string url, string id)
        {
            var divUrl = new TagBuilder("div");
            divUrl.GenerateId(id);
            divUrl.SetInnerText(url);
            return divUrl.ToString(TagRenderMode.Normal);
        }
    }
}