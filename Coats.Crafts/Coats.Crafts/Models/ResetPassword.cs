using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Coats.Crafts.HtmlHelpers;
using System.Collections.Generic;
using Coats.Crafts.Interfaces;
using System.Web.Mvc;
using Coats.Crafts.Resources;

namespace Coats.Crafts.Models
{
    public class ResetPassword
    {
        public string Password { get; set; }
        public string SiteUrl { get; set; }
    }
}