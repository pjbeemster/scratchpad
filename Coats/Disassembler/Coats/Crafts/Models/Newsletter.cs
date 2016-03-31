namespace Coats.Crafts.Models
{
    using DD4T.ContentModel;
    using System;
    using System.Runtime.CompilerServices;

    public class Newsletter
    {
        public string Description { get; set; }

        public string Header { get; set; }

        public string id { get; set; }

        public string imageUrl { get; set; }

        public IField logo { get; set; }

        public bool Selected { get; set; }
    }
}

