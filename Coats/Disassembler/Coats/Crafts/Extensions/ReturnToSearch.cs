namespace Coats.Crafts.Extensions
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class ReturnToSearch
    {
        public string Referrer { get; set; }

        public string Url { get; set; }
    }
}

