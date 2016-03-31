namespace Coats.Crafts.FredHopper
{
    using Coats.Crafts.FASWebService;
    using System;
    using System.Linq;
    using System.ServiceModel;

    public class FredHopperInterface
    {
        public universe CallFredHopper(string fh_params)
        {
            page page = null;
            universe universe = null;
            FASWebServiceClient client = new FASWebServiceClient();
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
            }
            try
            {
                if (client.Endpoint.Address != null)
                {
                    page = client.getAll(fh_params);
                    client.Close();
                }
                else
                {
                    page = new page();
                }
                try
                {
                    universe = page.universes.SingleOrDefault<universe>(u => u.type == universeType.selected);
                }
                catch (Exception)
                {
                    universe = new universe();
                }
            }
            catch (Exception)
            {
                page = new page();
                client.Abort();
            }
            return universe;
        }

        public static string GetPublicationPath(int publicationId)
        {
            return string.Format("//{0}/{1}/publicationid=tcm_0_{2}_1", DefaultUniverse, DefaultLocale, publicationId);
        }

        public static string DefaultLocale
        {
            get
            {
                return "en_US";
            }
        }

        public static string DefaultUniverse
        {
            get
            {
                return "catalog01";
            }
        }
    }
}

