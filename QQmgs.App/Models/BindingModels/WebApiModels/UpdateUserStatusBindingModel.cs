using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Twitter.App.Models.BindingModels.WebApiModels
{
    public class UpdateUserStatusBindingModel
    {
        [Required]
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}