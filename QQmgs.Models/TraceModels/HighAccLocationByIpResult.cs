using System;

namespace Twitter.Models.TraceModels
{
    public class HighAccLocationByIpResult
    {
        public int Id { get; set; }

        public string IpAddress { get; set; }

        public DateTime DatePosted { get; set; }

        public string LoggedUserPhoneNumber { get; set; }

        public string LoggedUserName { get; set; }

        public bool IsLoggedSucceeded { get; set; }

        public string LocId { get; set; }

        public double? Radius { get; set; }

        public double? Confidence { get; set; }

        public string FormattedAddress { get; set; }

        public string Business { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public int? AdminAreaCode { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        public int? Error { get; set; }

        public string LocalTime { get; set; }
    }
}
