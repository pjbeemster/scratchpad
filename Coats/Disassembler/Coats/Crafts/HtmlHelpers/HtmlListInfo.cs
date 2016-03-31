namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class HtmlListInfo
    {
        public HtmlListInfo(HtmlTag htmlTag, int columns = 0, object htmlAttributes = null)
        {
            this.htmlTag = htmlTag;
            this.Columns = columns;
            this.htmlAttributes = htmlAttributes;
        }

        public int Columns { get; set; }

        public object htmlAttributes { get; set; }

        public HtmlTag htmlTag { get; set; }
    }
}

