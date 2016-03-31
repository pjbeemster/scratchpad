using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoIP
{
    public interface ILookupService
    {
        Location getLocation(String str);
    }
}
