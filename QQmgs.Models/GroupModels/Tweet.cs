using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Twitter.Models.UserModels;

namespace Twitter.Models.GroupModels
{
    public class Tweet
    {
        public Tweet()
        {
            this.UsersFavourite = new HashSet<User>();
            this.Replies = new HashSet<Tweet>();
            this.Reply = new HashSet<Reply>();
            this.Retweets = new HashSet<Tweet>();
            this.Reports = new HashSet<Report>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(250)]
        public string Text { get; set; }

        public bool IsSoftDeleted { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DatePosted { get; set; }

        public bool IsRetweet { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

        public virtual ICollection<User> UsersFavourite { get; set; }

        public virtual ICollection<Tweet> Replies { get; set; }

        public virtual ICollection<Reply> Reply { get; set; } 

        public ICollection<Tweet> Retweets { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}