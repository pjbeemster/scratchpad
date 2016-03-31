using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Coats.Crafts.Gateway.CraftsIntegrationService;
using DD4T.ContentModel;
using Coats.Crafts.Data;
using System.ComponentModel;
using DataAnnotationsExtensions;
using Coats.Crafts.Attributes;
using Coats.Crafts.Resources;
using Coats.Crafts.Configuration;
namespace Coats.Crafts.Models
{
    public class Scrapbook
    {
        public Scrapbook () { }

        public Scrapbook (string imageUrl, string description, string type, string sourceUrl, string sourceDescription, string returnUrl)
        {
            this.imageUrl = imageUrl;
            this.description = description;
            this.type = type;
            this.sourceUrl = sourceUrl;
            this.sourceDescription = sourceDescription;
            this.returnUrl = returnUrl;
        }

        public List<ScrapbookItem> items { get; set; }

        public string imageUrl { get; set; }

        [CustomResourceRequired("ScrapbookDescriptionRequired")]
        public string description { get; set; }

        [DisplayName("Resource Type")]
        public string type { get; set; }

        [Display(Name = "Source")]
        [CustomResourceRequired("ScrapbookSourceUrlRequired")]
        //[Url]
        [CustomRegularExpression("ScrapbookSourceUrlInvalid", @"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?")]
        public string sourceUrl { get; set; }

        [DisplayName("Resource Link Text")]
        [AllowHtml]
        public string sourceDescription { get; set; }

        //string test = Helper.GetResource("ScrapbookSourceUrlValidation").ToString();

        public string returnUrl { get; set; }
    }
}