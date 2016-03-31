namespace Coats.Crafts.Utils
{
    using System;

    public class TokenAdapter
    {
        public static string ReplaceToken(string tridionToken)
        {
            return tridionToken.Replace("[*", "<#").Replace("*]", "#>");
        }
    }
}

