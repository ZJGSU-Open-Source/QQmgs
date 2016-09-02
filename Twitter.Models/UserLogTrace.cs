using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class UserLogTrace
    {
        [Key]
        public int TraceId { get; set; }

        public string IpAddress { get; set; }

        public DateTime DatePosted { get; set; }

        public string RealName { get; set; }

        public string PhoneNumber { get; set; }
        
        public bool IsLoggedSucceeded { get; set; }
    }
}
