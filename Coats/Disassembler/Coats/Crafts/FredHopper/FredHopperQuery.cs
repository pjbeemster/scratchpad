namespace Coats.Crafts.FredHopper
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class FredHopperQuery
    {
        private List<string> _categoryIdList;
        private const string QsCloseBrace = "%7D";
        private const string QsEncoded = "fh_eds=%C3%9F";
        private const string QsForwardSlash = "%2F";
        private const string QsLessThan = "%3C";
        private const string QsOpenBrace = "%7B";
        private const string QueryPath = "fredhopper/query?";

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Server);
            builder.Append(this.Server.EndsWith("/") ? "" : "/");
            builder.Append("fredhopper/query?");
            builder.Append("fh_eds=%C3%9F");
            builder.Append("&");
            try
            {
                builder.AppendFormat("fh_refview={0}&", this.ViewType.ToString());
            }
            catch (Exception)
            {
                builder.AppendFormat("fh_refview={0}&", ViewTypes.lister.ToString());
            }
            builder.AppendFormat("fh_location=%2F%2F{0}%2F{1}%2F", this.Universe, this.Locale);
            builder.Append("categories%3C%7B");
            for (int i = 0; i < this.CategoryIdList.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.Append(this.CategoryIdList[i]);
            }
            builder.Append("%7D");
            return builder.ToString();
        }

        public List<string> CategoryIdList
        {
            get
            {
                if (this._categoryIdList == null)
                {
                    this._categoryIdList = new List<string>();
                }
                return this._categoryIdList;
            }
            set
            {
                this._categoryIdList = value;
            }
        }

        public string Locale { get; set; }

        public string Server { get; set; }

        public string Universe { get; set; }

        public ViewTypes ViewType { get; set; }

        public enum ViewTypes
        {
            home,
            summary,
            lister,
            detail,
            compare,
            search
        }
    }
}

