using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModels
{
    public class InviteUserToGroupBindingModel
    {
        [Required]
        public string UserPhoneNumber { get; set; }

        [Required]
        public int GroupId { get; set; }
    }
}