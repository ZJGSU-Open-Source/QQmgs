using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class DevLog
    {
        public DevLog()
        {
        }

        [Key]
        public int Id { get; set; }

        public DateTime PublishedTime { get; set; }

        [MaxLength(1000)]
        public string Log { get; set; }
    }
}
