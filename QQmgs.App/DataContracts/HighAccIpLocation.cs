using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Twitter.App.DataContracts
{
    public class HighAccIpLocation
    {
        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public class Content
    {
        [JsonProperty("locid")]
        public string LocId { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("address_component")]
        public AddressComponent AddressComponent { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("business")]
        public string Business { get; set; }
    }

    public class AddressComponent
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("street_number")]
        public string StreetNumber { get; set; }

        [JsonProperty("admin_area_code")]
        public int AdminAreaCode { get; set; }
    }

    public class Location
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Result
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("loc_time")]
        public string LocalTime { get; set; }
    }
}