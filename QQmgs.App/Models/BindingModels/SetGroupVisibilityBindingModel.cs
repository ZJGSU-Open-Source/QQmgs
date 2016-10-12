using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModel
{
    public class SetGroupVisibilityBindingModel
    {
        public string IsDisplay { get; set; }

        [Required]
        public int GroupId { get; set; }
    }
}