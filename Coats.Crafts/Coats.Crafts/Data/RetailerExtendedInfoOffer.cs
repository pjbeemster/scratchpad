using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DD4T.ContentModel;

namespace Coats.Crafts.Data
{
    public class RetailerExtendedInfoPromo
    {
        public string title { get; set; }
        public string description { get; set; }
        public string buttonText { get; set; }
        public string buttonURL { get; set; }
        public string buttonTarget { get; set; }
        public string buttonLinkType { get; set; }
        public IField img { get; set; }
    }
}