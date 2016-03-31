using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DD4T.ContentModel;
using System.Collections;

namespace Coats.Crafts.Extensions
{
    public static class IEnumeralExtensions
    {
        public static IFieldSet ToFieldSet(this IEnumerable<KeyValuePair<string, IField>> ie)
        {
            IFieldSet fieldSet = new FieldSet();
            foreach (var item in ie)
            {
                fieldSet.Add(item);
            }
            return fieldSet;
        }

    }
}