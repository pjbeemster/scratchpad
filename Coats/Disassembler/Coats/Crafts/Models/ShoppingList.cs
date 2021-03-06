﻿namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ShoppingList
    {
        public IComponentPresentation componentPresentation { get; set; }

        public IEnumerable<IGrouping<string, ShoppingListItem>> items { get; set; }
    }
}

