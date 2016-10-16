using System.Collections.Generic;
using Newtonsoft.Json;
using Twitter.Models;

namespace Twitter.App.Models.ViewModels
{
    public class UserViewModel
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("has_avatar_image")]
        public bool HasAvatarImage { get; set; }

        [JsonProperty("avatar_image_name")]
        public string AvatarImageName { get; set; }

        [JsonProperty("joined_groups")]
        public IEnumerable<GroupVieModels> JoinedGroups { get; set; }

        [JsonProperty("created_groups")]
        public IEnumerable<GroupVieModels> CreatedGroups { get; set; }

        [JsonProperty("posted_tweets")]
        public IEnumerable<TweetViewModel> PostedTweets { get; set; }

        [JsonProperty("joined_activities")]
        public IEnumerable<ActivityViewModel> JoinedActivities { get; set; }

        [JsonProperty("created_activities")]
        public IEnumerable<ActivityViewModel> CreatedActivities { get; set; }

        [JsonProperty("posted_photos")]
        public IEnumerable<PhotoViewModel> PostedPhotos { get; set; }
    }
}