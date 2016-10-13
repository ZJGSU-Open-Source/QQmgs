using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.ViewModels
{
    public class UserBioStatisticsViewModel
    {
        public int TotalUserNumber { get; set; }

        public int TotalTweetNumber { get; set; }

        public int TotalGroupNumber { get; set; }

        public int TotalActivityNumber { get; set; }

        public int TotalPhotoNumber { get; set; }

        public int TotalReplyNumber { get; set; }

        public int TotalReviewNumber { get; set; }
    }
}