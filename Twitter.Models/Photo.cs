using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public string Descrption { get; set; }

        public virtual User Author { get; set; }

        public PhotoType PhotoType { get; set; }

        public bool IsSoftDelete { get; set; }

        public int OriginalWidth { get; set; }

        public int OriginalHeight { get; set; }
    }

    public enum PhotoType
    {
        Photo,
        AvatarImage,
        GroupImage
    }
}
