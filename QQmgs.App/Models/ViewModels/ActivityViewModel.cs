﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Twitter.Models.ActivityModels;

namespace Twitter.App.Models.ViewModels
{
    public class ActivityViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("creator_id")]
        public string CreatorId { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_status")]
        public string CreatorStatus { get; set; }

        [JsonProperty("creator_avatar_image")]
        public string CreatorAvatarImage { get; set; }

        [JsonProperty("has_creator_avatar_image")]
        public bool HasCreatorAvatarImage { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTime EndTime { get; set; }

        [JsonProperty("place")]
        public string Place { get; set; }

        [JsonProperty("publish_time")]
        public DateTime PublishTime { get; set; }

        [JsonProperty("avatar_image")]
        public string AvatarImage { get; set; }

        [JsonProperty("classficiation")]
        public string Classficiation { get; set; }

        [JsonProperty("participations")]
        public IEnumerable<ParticipationViewModel> Participations { get; set; }

        [JsonProperty("organizer")]
        public string Organizer { get; set; }

        [JsonProperty("is_display")]
        public bool IsDisplay { get; set; }
    }

    public class ParticipationViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("avatar_image")]
        public string AvatarImage { get; set; }

        [JsonProperty("has_avatar_image")]
        public bool HasAvatarImage { get; set; }

        [JsonProperty("join_time")]
        public DateTime JoinTime { get; set; }
    }

    public class ActivityPhotoViewModel
    {
        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        public ActivityPhotoType? ActivityPhotoType { get; set; }
    }
}