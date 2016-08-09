using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }

        public IEnumerable<TweetViewModel> Tweets { get; set; }

        public IEnumerable<GroupVieModels> Groups { get; set; }
    }
}