namespace Coats.Crafts.Repositories.Interfaces
{
    using Coats.Crafts.Models;
    using System;

    public interface ICatsRepository
    {
        bool SaveCatsFormData(CatsContactForm form);
    }
}

