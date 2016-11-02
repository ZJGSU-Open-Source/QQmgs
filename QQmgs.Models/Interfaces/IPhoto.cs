using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.Models.PhotoModels;

namespace Twitter.Models.Interfaces
{
    public interface IPhoto
    {
        int Id { get; set; }

        string Name { set; get; }

        DateTime DatePosted { get; set; }

        string AuthorId { get; set; }

        string Description { get; set; }

        bool IsSoftDelete { get; set; }

        int Width { get; set; }

        int Height { get; set; }
    }
}
