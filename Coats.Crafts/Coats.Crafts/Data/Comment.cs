using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coats.Crafts.Data
{
    public class Comment
    {
        public long id { get; set; }
        public int? score { get; set; }
        public int status { get; set; }
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public DateTime CommentDate { get; set; }
    }
}