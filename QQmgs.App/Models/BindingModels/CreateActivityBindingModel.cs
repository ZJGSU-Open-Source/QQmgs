using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModels
{
    public class CreateActivityBindingModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Place { get; set; }
    }
}