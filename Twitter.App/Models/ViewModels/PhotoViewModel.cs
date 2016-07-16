using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Twitter.App.Models.ViewModels
{
    public class PhotoViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Author")]
        public string Author { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}