using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.ViewModels
{
    public class UserStatisticsViewModel
    {
        public DateTime Date { get; set; }

        public int NewUserNumber { get; set; }

        public double UserIncreasingRatio { get; set; }

        public int ActiveUserNumber { get; set; }

        public int NewActivities { get; set; }

        public int NewPosts { get; set; }

        public int NewReplies { get; set; }

        public int NewPhotos { get; set; }
    }
}