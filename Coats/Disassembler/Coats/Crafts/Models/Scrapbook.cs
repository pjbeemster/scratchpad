namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;

    public class Scrapbook
    {
        public Scrapbook()
        {
        }

        public Scrapbook(string imageUrl, string description, string type, string sourceUrl, string sourceDescription, string returnUrl)
        {
            this.imageUrl = imageUrl;
            this.description = description;
            this.type = type;
            this.sourceUrl = sourceUrl;
            this.sourceDescription = sourceDescription;
            this.returnUrl = returnUrl;
        }

        [CustomResourceRequired("ScrapbookDescriptionRequired")]
        public string description { get; set; }

        public string imageUrl { get; set; }

        public List<ScrapbookItem> items { get; set; }

        public string returnUrl { get; set; }

        [DisplayName("Resource Link Text"), AllowHtml]
        public string sourceDescription { get; set; }

        [CustomResourceRequired("ScrapbookSourceUrlRequired"), CustomRegularExpression("ScrapbookSourceUrlInvalid", @"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?"), Display(Name="Source")]
        public string sourceUrl { get; set; }

        [DisplayName("Resource Type")]
        public string type { get; set; }
    }
}

