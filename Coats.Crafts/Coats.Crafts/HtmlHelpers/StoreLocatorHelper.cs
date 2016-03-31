using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Xml;

namespace Coats.Crafts.HtmlHelpers
{
    public static class StoreLocatorHelper
    {

        /// <summary>
        /// Builds up the 4 events and promos that are displayed with the store result
        /// If there are less than 2 events then we add more promos (if they are available) and vice versa
        /// </summary>
        /// <param name="eventsCount">The events count.</param>
        /// <param name="promoCount">The promo count.</param>
        /// <param name="events">The events.</param>
        /// <param name="promo">The promo.</param>
        /// <returns></returns>
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

            if (eventsCount + promoCount == 4 || (events == 0 && promo == 0))
            {
                List<int> offersBasket = new List<int>();

                offersBasket.Add(eventsCount);
                offersBasket.Add(promoCount);

                return offersBasket;

            } else {
                //Let's get some more 
                return OffersBasket(eventsCount, promoCount, events, promo);
            }

            
        }

    }
}