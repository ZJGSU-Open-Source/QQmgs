using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models.Interfaces
{
    public interface IPhoto
    {
        int Id { get; set; }

        string Name { set; get; }

        DateTime DatePosted { get; set; }

        string AuthorId { get; set; }

        string Description { get; set; }

        PhotoType PhotoType { get; set; }

        bool IsSoftDelete { get; set; }

        int Width { get; set; }

        int Height { get; set; }
    }

    public enum PhotoType
    {
        Photo = 0,
        AvatarImage = 1,
        GroupImage = 2,
        ActivityImage = 3
    }
}
