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
            this.Users = new HashSet<User>();
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

        [Display(Name = "Update Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastTweetUpdateTime { get; set; }

        public bool HasImageOverview { get; set; }

        public string ImageOverview { get; set; }

        public string Description { get; set; }

        public bool IsDisplay { get; set; }

        public bool IsPrivate { get; set; }

        public Classification Classification { get; set; }

        public virtual ICollection<Tweet> Tweets { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual GroupPlugin GroupPlugin { get; set; }
    }

    public enum Classification
    {
        兴趣爱好,
        学院组织,
        我爱学习,
        社团组织,
        社交聊天,
        吃货俱乐部,
        未分类
    };
}
