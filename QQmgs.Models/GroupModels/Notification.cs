using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Twitter.Models.UserModels;

namespace Twitter.Models.GroupModels
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public NotificationType Type { get; set; }
    }

    public enum NotificationType
    {
        Retweet,
        FavouriteTweet,
        NewFollower
    }
}
