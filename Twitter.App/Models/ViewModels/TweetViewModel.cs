using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Twitter.Models;

namespace Twitter.App.Models.ViewModels
{
    public class TweetViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("authorStatus")]
        public string AuthorStatus { get; set; }

        [JsonProperty("is_event")]
        public bool IsEvent { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user_favourite_count")]
        public int UsersFavouriteCount { get; set; }

        [JsonProperty("replies_count")]
        public int RepliesCount { get; set; }

        [JsonProperty("retweets_count")]
        public int RetweetsCount { get; set; }

        [JsonProperty("reply_list")]
        public List<ReplyViewModel> ReplyList { get; set; }

        [JsonProperty("date_posted")]
        public DateTime DatePosted { get; set; }
    }
}