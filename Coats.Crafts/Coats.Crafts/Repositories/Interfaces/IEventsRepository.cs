using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Gateway.CraftsIntegrationService;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface IEventsRepository
    {
        List<CraftsEvent> GetCraftsEventsInArea(decimal lat, decimal lng, int radius, int maxResults);
        List<CraftsEvent> GetCraftsEventsInArea();
    }
}
