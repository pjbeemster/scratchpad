using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Interfaces;

namespace Coats.Crafts.Models
{
    public class ToolService : ICoatsData
    {
        public string Id { get; set; }
        public string NameUntranslated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Authorised { get; set; }
        public object Tags { get; set; }
        public string Uri { get; set; }
        public List<string> RelatedUris { get; set; }
        public string ComponentUrl { get; set; }
        public string ToolUrl { get; set; }
        public string Parent { get; set; }
    }
}