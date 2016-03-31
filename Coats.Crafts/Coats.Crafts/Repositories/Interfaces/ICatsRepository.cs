using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Data;
using Coats.Crafts.Models;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface ICatsRepository
    {
        bool SaveCatsFormData(CatsContactForm form);
    }
}
