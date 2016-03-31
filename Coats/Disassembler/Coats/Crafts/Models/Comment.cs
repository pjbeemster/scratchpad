namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class Comment
    {
        [CustomResourceRequired("CommentRequired"), System.ComponentModel.DataAnnotations.MaxLength(2)]
        public string UserComment { get; set; }
    }
}

