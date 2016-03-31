namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class GroupedSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }

        public string GroupName { get; set; }
    }
}

