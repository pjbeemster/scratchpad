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
    public class CommentEmail : IEmail
    {
        public string EmailAddress { get; set; }
        public string ComponentName { get; set; }
        public string ComponentID { get; set; }
        public string User { get; set; }
    }
}