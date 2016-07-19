using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Twitter.App.Models.ViewModels
{
    public class CourseReviewVideModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("Comment")]
        public string Comment { get; set; }

        [JsonProperty("Course")]
        public string Course { get; set; }

        [JsonProperty("Teacher")]
        public string Teacher { get; set; }

        [JsonProperty("date_posted")]
        public DateTime DatePosted { get; set; }
    }
}