using System;
using System.ComponentModel.DataAnnotations;
using Twitter.Models.Interfaces;
using Twitter.Models.UserModels;

namespace Twitter.Models.ActivityModels
{
    public class ActivityPhoto : IPhoto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        public string Description { get; set; }

        public PhotoType PhotoType { get; set; }

        public bool IsSoftDelete { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [Required]
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public ActivityPhotoType? ActivityPhotoType { get; set; }
    }

    public enum ActivityPhotoType
    {
        Poster,
        OverView,
        ActivityPhoto
    }
}
