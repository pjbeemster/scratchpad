namespace Coats.Crafts.Models
{
    using Coats.Crafts.Interfaces;
    using System;
    using System.Runtime.CompilerServices;

    public class CommentEmail : IEmail
    {
        public string ComponentID { get; set; }

        public string ComponentName { get; set; }

        public string EmailAddress { get; set; }

        public string User { get; set; }
    }
}

