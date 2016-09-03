using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.ViewModels
{
    public class DevLogViewModel
    {
        public int Id { get; set; }

        public string Log { get; set; }

        public DateTime PublishedTime { get; set; }
    }
}