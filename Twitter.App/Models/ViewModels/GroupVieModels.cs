using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.ViewModels
{
    public class GroupVieModels
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CreaterId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int TweetsCount { get; set; }
    }
}