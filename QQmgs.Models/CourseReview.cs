using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class CourseReview
    {
        [Key]
        public int Id { get; set; }

        public string Comment { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public DateTime DatePosted { get; set; }

        public virtual User Author { get; set; }

        [Required]
        public string Course { get; set; }

        public string Teacher { get; set; }
    }
}
