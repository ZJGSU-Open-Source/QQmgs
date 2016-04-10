using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Twitter.App.Models.ViewModels
{
    using System;

    public class ReplyViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} should be no more than {1} charaters and no less than {2} charaters.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("publish_time")]
        public DateTime PublishTime { get; set; }
    }
}