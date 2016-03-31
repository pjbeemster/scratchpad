using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Gateway.CraftsIntegrationService;

namespace Coats.Crafts.Data
{
    public class StoreLocatorRetailer
    {
        public IntegrationServiceRetailer RetailerFromWCF { get; set; }
        //trid fields
        public string Description { get; set; }
        public List<RetailerExtendedInfoImage> Images { get; set; }
        public List<RetailerExtendedInfoPromo> Promos { get; set; }
        public bool isPromotedRetailer { get; set; }

        public string Tcm
        {
            get
            {
                string tcmComponentTemplate = "tcm:{0}-{1}";
                return String.Format(tcmComponentTemplate,
                                            RetailerFromWCF.PublicationId,
                                            RetailerFromWCF.ItemId);
            }
        }
    }
}