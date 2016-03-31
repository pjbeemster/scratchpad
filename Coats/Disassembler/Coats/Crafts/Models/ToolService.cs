namespace Coats.Crafts.Models
{
    using Coats.Crafts.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ToolService : ICoatsData
    {
        public bool Authorised { get; set; }

        public string ComponentUrl { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string NameUntranslated { get; set; }

        public string Parent { get; set; }

        public List<string> RelatedUris { get; set; }

        public object Tags { get; set; }

        public string ToolUrl { get; set; }

        public string Uri { get; set; }
    }
}

