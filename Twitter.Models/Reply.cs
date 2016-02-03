using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }

        public int TweetId { get; set; }

        [Required]
        [MinLength(1)]
        public string Content { get; set; }

        [ForeignKey("TweetId")]
        public virtual Tweet Tweet { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        public DateTime PublishTime { get; set; }
    }
}
