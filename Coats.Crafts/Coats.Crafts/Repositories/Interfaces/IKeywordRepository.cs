using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Models;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IKeywordRepository
    {
        IList<Keyword> GetKeywordsList(string identifier);
    }
}