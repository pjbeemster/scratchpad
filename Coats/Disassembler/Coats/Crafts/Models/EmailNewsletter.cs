namespace Coats.Crafts.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    public class EmailNewsletter
    {
        private List<SelectListItem> _techniques;

        public string EmailAddress { get; set; }

        public bool IsLoggedIn { get; set; }

        public List<SelectListItem> Techniques
        {
            get
            {
                if (this._techniques == null)
                {
                    this._techniques = new List<SelectListItem>();
                }
                return this._techniques;
            }
            set
            {
                this._techniques = value;
            }
        }
    }
}

