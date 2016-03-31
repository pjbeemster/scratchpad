namespace Coats.Crafts.Repositories.Interfaces
{
    using Gateway.CraftsIntegrationService;
    using System;
    using System.Collections.Generic;

    public interface IEventsRepository
    {
        List<CraftsEvent> GetCraftsEventsInArea();
        List<CraftsEvent> GetCraftsEventsInArea(decimal lat, decimal lng, int radius, int maxResults);
    }
}

