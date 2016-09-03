using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModels
{
    public class CreateGroupBindingModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(60)]
        public string Name { get; set; }

        [MinLength(0)]
        [MaxLength(100)]
        public string Description { get; set; }
    }
}