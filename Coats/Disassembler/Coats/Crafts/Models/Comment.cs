namespace Coats.Crafts.Models
{
    using Coats.Crafts.Attributes;

    public class Comment
    {
        [CustomResourceRequired("CommentRequired")]
        //[global::System.ComponentModel.DataAnnotations.MaxLength(2)]
        public string UserComment { get; set; }
    }
}

