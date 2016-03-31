namespace Coats.Crafts.Data
{
    using System;
    using System.Runtime.CompilerServices;

    public class GoogleMapsMarker
    {
        public GoogleMapsMarker()
        {
        }

        public GoogleMapsMarker(string name, decimal lat, decimal lng)
        {
            this.name = name;
            this.lat = lat;
            this.lng = lng;
        }

        public GoogleMapsMarker(string name, decimal lat, decimal lng, string addr1, string addr2, string addr3, string addr4, string addr5, string addr6, string addr7, string type)
        {
            this.name = name;
            this.lat = lat;
            this.lng = lng;
            this.AddressLine1 = addr1;
            this.AddressLine2 = addr2;
            this.AddressLine3 = addr3;
            this.AddressLine4 = addr4;
            this.AddressLine5 = addr5;
            this.AddressLine6 = addr6;
            this.AddressLine7 = addr7;
            this.type = type;
        }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public string AddressLine6 { get; set; }

        public string AddressLine7 { get; set; }

        public decimal lat { get; set; }

        public decimal lng { get; set; }

        public string name { get; set; }

        public string type { get; set; }
    }
}

