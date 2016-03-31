namespace Coats.Crafts.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IKeywordRepository
    {
        IList<Keyword> GetKeywordsList(string identifier);
    }
}

