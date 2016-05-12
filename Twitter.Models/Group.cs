using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Group
    {
        public Group()
        {
            this.Tweets = new HashSet<Tweet>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string CreaterId { get; set; }

        public string Name { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }

        public bool HasImageOverview { get; set; }

        public string ImageOverview { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Tweet> Tweets { get; set; }
    }
}
