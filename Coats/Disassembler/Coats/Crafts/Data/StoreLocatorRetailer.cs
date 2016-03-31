namespace Coats.Crafts.Data
{
    using Coats.Crafts.Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class StoreLocatorRetailer
    {
        public string Description { get; set; }

        public List<RetailerExtendedInfoImage> Images { get; set; }

        public bool isPromotedRetailer { get; set; }

        public List<RetailerExtendedInfoPromo> Promos { get; set; }

        public IntegrationServiceRetailer RetailerFromWCF { get; set; }

        public string Tcm
        {
            get
            {
                string format = "tcm:{0}-{1}";
                return string.Format(format, this.RetailerFromWCF.PublicationId, this.RetailerFromWCF.ItemId);
            }
        }
    }
}

