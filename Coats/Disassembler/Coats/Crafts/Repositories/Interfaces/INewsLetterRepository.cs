﻿namespace Coats.Crafts.Repositories.Interfaces
{
    using Models;
    using System;
    using System.Collections.Generic;

    public interface INewsLetterRepository
    {
        IList<Newsletter> GetNewsLetterList(string identifier);
    }
}

