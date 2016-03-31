namespace Coats.Crafts.ControllerHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class DynamicJsonObject : DynamicObject
    {
        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];
            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if ((result is ArrayList) && ((result as ArrayList) is IDictionary<string, object>))
            {
                result = new List<DynamicJsonObject>(from x in (result as ArrayList).ToArray() select new DynamicJsonObject(x as IDictionary<string, object>));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }
            return this.Dictionary.ContainsKey(binder.Name);
        }

        private IDictionary<string, object> Dictionary { get; set; }
    }
}

