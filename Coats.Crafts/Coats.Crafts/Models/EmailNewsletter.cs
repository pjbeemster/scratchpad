using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Text;

namespace Coats.Crafts.Models
{
    public class EmailNewsletter
    {
        private List<SelectListItem> _techniques;
        public List<SelectListItem> Techniques 
        {
            get
            {
                if (_techniques == null) { _techniques = new List<SelectListItem>(); }
                return _techniques;
            }
            set
            {
                _techniques = value;
            }
        }
        public string EmailAddress { get; set; }
        public bool IsLoggedIn { get; set; }
    }

}