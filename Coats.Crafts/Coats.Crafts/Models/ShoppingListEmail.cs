using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coats.Crafts.Interfaces;
using System.ComponentModel.DataAnnotations;
using Coats.Crafts.Gateway.CraftsIntegrationService;

namespace Coats.Crafts.Models
{
    public class ShoppingListEmail : IEmail
    {
        public string EmailAddress { get; set; }
        public IEnumerable<IGrouping<string, ShoppingListItem>> items { get; set; }
        public string itemHtml { get; set; }
    }
}
