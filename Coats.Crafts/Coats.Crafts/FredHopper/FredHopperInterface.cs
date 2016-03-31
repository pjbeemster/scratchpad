using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.FASWebService;
using System.Text;
using System.Web.Mvc;
using Coats.Crafts.Models;
using System.ServiceModel;

namespace Coats.Crafts.FredHopper
{
    /// <summary>
    /// A simple FredHopper calling class
    /// </summary>
    public class FredHopperInterface
    {
        /// <summary>
        /// Gets the default universe, would you believe? Default universe is typically "catalog01"
        /// </summary>
        public static string DefaultUniverse
        {
            get 
            { 
                // Get from config!!!
                return "catalog01"; 
            }
        }

        /// <summary>
        /// Gets the default locale, would you believe? Default locale is typically "en-US"
        /// </summary>
        public static string DefaultLocale
        {
            get 
            { 
                // Get from config!!!
                return "en_US"; 
            }
        }

        /// <summary>
        /// Returns the full publication path, which consists of a combiation of DefaultUniverse, DefaultLocale 
        /// and the specified publicationId. e.g. "//catalog01/en_US/publicationid=tcm_0_70_1"
        /// </summary>
        /// <param name="publicationId">The current publication Id, usually gleaned from _settings.PublicationId</param>
        /// <returns>The full publication path e.g. "//catalog01/en_US/publicationid=tcm_0_70_1"</returns>
        public static string GetPublicationPath(int publicationId)
        {
            string publicationPath = string.Format("//{0}/{1}/publicationid=tcm_0_{2}_1", DefaultUniverse, DefaultLocale, publicationId);
            return publicationPath;
        }

        /// <summary>
        /// Basic call to FredHopper using the FredHopper WCF
        /// </summary>
        /// <param name="fh_params">
        /// Usually supplied by creating the required query object, then performing a toString() on it.
        /// </param>
        /// <returns>
        /// The currently selected FredHopper Universe containng the result set
        /// </returns>
        public universe CallFredHopper(string fh_params)
        {
            page fhResponse = null;
            universe fhUniverse = null;

            FASWebServiceClient client = new FASWebServiceClient();
            
            // Check for a possible "faulted" state from a previous call.
            // This happened once with the "using (var client = new FASWebServiceClient())" type call,
            // which didn't abort the connection if an error was encountered.
            // Hopefully, the following code will handle any exiting and future faulted state occurances.
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
            }

            try
            {
                if (client.Endpoint.Address != null)
                {
                    fhResponse = client.getAll(fh_params);
                    client.Close();
                }
                else
                {
                    fhResponse = new page();
                }
                try
                {
                    // Get universe: Current universe will be marked with type="Selected"
                    fhUniverse = fhResponse.universes.SingleOrDefault(u => u.type == universeType.selected);
                }
                catch (Exception) { fhUniverse = new universe(); }
                    
            }
            catch (Exception)
            {
                fhResponse = new page();
                client.Abort();
            }
            // Other possible exceptions (if we need to be more specific)
            //catch (CommunicationException)
            //{
            //    fhResponse = new page();
            //    client.Abort();
            //}
            //catch (TimeoutException)
            //{
            //    fhResponse = new page();
            //    client.Abort();
            //}

            return fhUniverse;
        }

    }
}