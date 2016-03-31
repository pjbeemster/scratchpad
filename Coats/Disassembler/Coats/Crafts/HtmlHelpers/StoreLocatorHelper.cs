namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections.Generic;

    public static class StoreLocatorHelper
    {
        public static List<int> OffersBasket(int eventsCount, int promoCount, int events, int promo)
        {
            if (events > 0)
            {
                eventsCount++;
                events--;
            }
            if (promo > 0)
            {
                promoCount++;
                promo--;
            }
            if (((eventsCount + promoCount) == 4) || ((events == 0) && (promo == 0)))
            {
                return new List<int> { 
                    eventsCount,
                    promoCount
                };
            }
            return OffersBasket(eventsCount, promoCount, events, promo);
        }
    }
}

