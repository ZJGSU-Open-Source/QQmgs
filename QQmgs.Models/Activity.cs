using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Activity
    {
        public Activity()
        {
            this.Participents = new HashSet<User>();
        }
            
        [Key]
        public int Id { get; set; }

        [Required]
        public string CreatorId { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [MaxLength(50)]
        public string Place { get; set; }

        public DateTime PublishTime { get; set; }

        public string AvatarImage { get; set; }

        public virtual ICollection<User> Participents { get; set; }

        public ActivityClassficiation Classficiation { get; set; }
    }

    public enum ActivityClassficiation
    {
        其他,
        社交,
        兴趣,
        学习,
        运动
    }
}
