using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Coats.Crafts.FredHopper
{
    /// <summary>
    /// Utility class to construct a full FredHopper query URL, based on the properties set.
    /// </summary>
    public class FredHopperQuery
    {
        #region Private Constants

        private const string QueryPath = "fredhopper/query?";
        private const string QsEncoded = "fh_eds=%C3%9F";       // Basically tells FredHopper that the query string is encoded.
        private const string QsForwardSlash = "%2F";            // %2F = /
        private const string QsLessThan = "%3C";                // %3C = <
        private const string QsOpenBrace = "%7B";               // %7B = {
        private const string QsCloseBrace = "%7D";              // %7D = }

        #endregion

        #region Private Members

        private List<string> _categoryIdList;

        #endregion

        /// <summary>
        /// Enforce the fh_refview string types by using an enum.
        /// Probably better ways of doing this, but this works fine.
        /// </summary>
        public enum ViewTypes
        {
            home,
            summary,
            lister,
            detail,
            compare,
            search
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the type of view that FredHopper should return. 
        /// Options are home, summary, lister, detail, compare, search.
        /// If not set, will default to lister.
        /// </summary>
        public ViewTypes ViewType { get; set; } // Lister, Page, etc. Mostly, we'll be using "Lister"
        
        /// <summary>
        /// Gets or sets the URL of the server, including port number (if required).
        /// e.g. http://coats-devweb2:8180
        /// </summary>
        public string Server { get; set; }      // Config setting?
        
        /// <summary>
        /// Gets or sets the universe to query.
        /// e.g. catalog01
        /// </summary>
        public string Universe { get; set; }    // Config setting?
        
        /// <summary>
        /// Gets or sets the locale to query.
        /// e.g. en_US
        /// </summary>
        public string Locale { get; set; }      // Config setting?
        
        /// <summary>
        /// Gets or sets a list of category Ids (facet, or keywords) to filter the query.
        /// </summary>
        public List<string> CategoryIdList
        {
            get 
            {
                if (_categoryIdList == null) { _categoryIdList = new List<string>(); }
                return _categoryIdList; 
            }
            set { _categoryIdList = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to build a full query string based on the properties set.
        /// </summary>
        /// <returns>
        /// A string which can be used directly in a web request.
        /// </returns>
        public override string ToString()
        {
            // Example of combined Crochet and Knitting
            // =========================================
            // http://coats-devweb2:8180/fredhopper/query?
            // fh_eds=%C3%9F&                       Leave hard coded. Basically tells FredHopper that the query string is encoded.
            // fh_refview=lister&                   View type (home,summary,lister,detail,compare,search)
            // fh_location=//catalog01/en_US/       Universe & Locale
            // categories<{                         Open the category id list
            // catalog01_tcm_0_70_1_categories_and_keywords_tcm_70_4016_512_tcm_70_17843_1024_tcm_70_17846_1024
            // ,                                    Comma separated        
            // catalog01_tcm_0_70_1_categories_and_keywords_tcm_70_4016_512_tcm_70_17843_1024_tcm_70_17845_1024
            // }                                    Close the category id list 

            // Build the initial URL, e.g. "http://coats-devweb2:8180/fredhopper/query?"
            StringBuilder sb = new StringBuilder();
            sb.Append(Server);
            sb.Append(Server.EndsWith("/") ? "" : "/");
            sb.Append(QueryPath);

            // Now add the query string items...

            // fh_eds=%C3%9F& - Hard coded. Basically tells FredHopper that the query string is encoded.
            sb.Append(QsEncoded);
            sb.Append("&");

            // View type, e.g. "fh_refview=lister&"
            try { sb.AppendFormat("fh_refview={0}&", ViewType.ToString()); }
            catch (Exception) { sb.AppendFormat("fh_refview={0}&", ViewTypes.lister.ToString()); }

            // Create the location using the Universe & Locale, e.g. "fh_location=//catalog01/en_US/"
            sb.AppendFormat("fh_location=%2F%2F{0}%2F{1}%2F", Universe, Locale);

            // Create the category keyword (facet) id list, in the format "categories<{keyword1,keyword2,keyword3,etc}"
            sb.Append("categories%3C%7B"); // "categories<{"
            for (int i = 0; i < CategoryIdList.Count; i++)
            {
                if (i > 0) { sb.Append(","); } // Comma separate the list
                sb.Append(CategoryIdList[i]);  // Add the actual id
            }
            sb.Append("%7D"); // Close the collection with a closing brace "}"

            return sb.ToString(); // Finally, return the whole shabumkin.
        }

        #endregion

    }
}