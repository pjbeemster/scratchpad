using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using Coats.Crafts.Attributes;

namespace Coats.Crafts.Models
{
    public class Comment
    {
        public Comment()
        {
           // CustomerDetails = new Customer();
        }

        //[ResourceRequired("CommentRequired")]
        [CustomResourceRequired("CommentRequired")]
        [MaxLength(2)]
        public string UserComment { get; set; }
    }
}