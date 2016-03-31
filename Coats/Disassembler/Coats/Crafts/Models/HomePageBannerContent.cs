namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    public class HomePageBannerContent
    {
        public IComponentPresentation Banner { get; set; }

        public List<Component> FredHopperComponents { get; set; }

        public List<SelectListItem> Tabs { get; set; }
    }
}

