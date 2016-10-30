using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Twitter.Models.Interfaces;
using Twitter.Models.UserModels;

namespace Twitter.Models.PhotoModels
{
    public class Image : IPhoto
    {
        public Image()
        {
            this.UsersFavourite = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public string Description { get; set; }

        public virtual User Author { get; set; }

        public PhotoType PhotoType { get; set; }

        public PhotoClasscification? PhotoClasscification { get; set; }

        public bool IsSoftDelete { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public virtual ICollection<User> UsersFavourite { get; set; }
    }

    public enum PhotoClasscification
    {
        Ohter = 0,
        View = 1,
        Food = 2,
        Portrait = 3,
        Art = 4
    }
}
