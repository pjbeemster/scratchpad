using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;
using Coats.Crafts.Data;

namespace Coats.Crafts.Models
{
    public class ShoppingList
    {
        public IEnumerable<IGrouping<string, ShoppingListItem>> items { get; set; }
        public IComponentPresentation componentPresentation { get; set; }
    }
}