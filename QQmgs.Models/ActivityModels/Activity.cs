using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Twitter.Models.UserModels;

namespace Twitter.Models.ActivityModels
{
    public class Activity
    {
        public Activity()
        {
            this.Participents = new HashSet<User>();
            this.ActivityPhotos = new HashSet<ActivityPhoto>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string CreatorId { get; set; }

        public virtual User Creator { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [MaxLength(50)]
        public string Place { get; set; }

        public DateTime PublishTime { get; set; }

        public string ActivityImage { get; set; }

        public virtual ICollection<User> Participents { get; set; }

        public ActivityClassficiation? Classficiation { get; set; }

        [MaxLength(50)]
        public string Organizer { get; set; }

        public bool IsDisplay { get; set; }

        public virtual ICollection<ActivityPhoto> ActivityPhotos { get; set; }

    }

    public enum ActivityClassficiation
    {
        Other,
        Interest,
        Study,
        Recurit,
        Sports
    }
}
