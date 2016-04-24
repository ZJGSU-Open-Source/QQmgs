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

        public string CreaterId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedTime { get; set; }

        public virtual ICollection<Tweet> Tweets { get; set; }
    }
}
