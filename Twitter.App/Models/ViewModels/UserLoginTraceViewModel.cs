using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Twitter.App.DataContracts;

namespace Twitter.App.Models.ViewModels
{
    public class UserLoginTraceViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("date_posted")]
        public DateTime DatePosted { get; set; }

        [JsonProperty("logged_user_phone_number")]
        public string LoggedUserPhoneNumber { get; set; }

        [JsonProperty("logged_user_name")]
        public string LoggedUserName { get; set; }

        [JsonProperty("is_logged_succeeded")]
        public bool IsLoggedSucceeded { get; set; }

        [JsonProperty("high_acc_ip_location")]
        public HighAccIpLocation HighAccIpLocation { get; set; }
    }
}