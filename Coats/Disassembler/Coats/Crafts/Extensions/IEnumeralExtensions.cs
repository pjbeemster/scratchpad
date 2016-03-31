namespace Coats.Crafts.Extensions
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class IEnumeralExtensions
    {
        public static IFieldSet ToFieldSet(this IEnumerable<KeyValuePair<string, IField>> ie)
        {
            IFieldSet set = new FieldSet();
            foreach (KeyValuePair<string, IField> pair in ie)
            {
                set.Add(pair);
            }
            return set;
        }
    }
}

