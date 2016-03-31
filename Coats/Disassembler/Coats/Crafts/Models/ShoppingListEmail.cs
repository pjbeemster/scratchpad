namespace Coats.Crafts.Models
{
    using Coats.Crafts.Interfaces;
    using Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ShoppingListEmail : IEmail
    {
        public string EmailAddress { get; set; }

        public string itemHtml { get; set; }

        public IEnumerable<IGrouping<string, ShoppingListItem>> items { get; set; }
    }
}

