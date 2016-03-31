namespace Coats.Crafts.NewsletterAPI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Item
    {
        public List<Datum> Data { get; set; }

        public string DateCreated { get; set; }

        public string Email { get; set; }

        public int GlobalStatus { get; set; }

        public string LastModified { get; set; }

        public int ListStatus { get; set; }

        public int SubscriberID { get; set; }
    }
}

