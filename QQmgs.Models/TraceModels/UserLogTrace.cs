using System;
using System.ComponentModel.DataAnnotations;

namespace Twitter.Models.TraceModels
{
    public class UserLogTrace
    {
        [Key]
        public int TraceId { get; set; }

        public string IpAddress { get; set; }

        public DateTime DatePosted { get; set; }

        public string PhoneNumber { get; set; }
        
        public bool IsLoggedSucceeded { get; set; }

        public bool IsCached { get; set; }
    }
}
