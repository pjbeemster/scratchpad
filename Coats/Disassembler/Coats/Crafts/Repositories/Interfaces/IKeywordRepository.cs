namespace Coats.Crafts.Repositories.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;

    public interface IKeywordRepository
    {
        IList<Keyword> GetKeywordsList(string identifier);
    }
}

