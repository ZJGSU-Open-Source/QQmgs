using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class UserLoginTrace
    {
        [Key]
        public int Id { get; set; }

        public string IpAddress { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        public string LoggedUserPhoneNumber { get; set; }

        public bool IsLoggedSucceeded { get; set; }
    }
}
