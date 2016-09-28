using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModels
{
    public class CreateActivityBindingModel
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Place { get; set; }

        public int Classfication { get; set; }
    }
}