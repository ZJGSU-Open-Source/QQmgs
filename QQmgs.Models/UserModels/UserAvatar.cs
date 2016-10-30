using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Models.Interfaces;

namespace Twitter.Models.UserModels
{
    public class UserAvatar : IPhoto
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

        public UserAvatarType? UserAvatarType { get; set; }
    }

    public enum UserAvatarType
    {
        AvatarImage,
        PhotoWall
    }
}
