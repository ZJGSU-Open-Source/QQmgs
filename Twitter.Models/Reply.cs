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

        [ForeignKey("TweetId")]
        public virtual Tweet Tweet { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(250)]
        public string Content { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public virtual User Author { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublishTime { get; set; }
    }
}
