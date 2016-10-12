using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModel
{
    public class CreateReviewBindingModel
    {
        public string Course { get; set; }

        public string Comment { get; set; }

        public string Teacher { get; set; }
    }
}