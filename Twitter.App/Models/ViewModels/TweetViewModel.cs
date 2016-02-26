using System;
using System.Collections.Generic;
using Twitter.Models;

namespace Twitter.App.Models.ViewModels
{
    public class TweetViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string AuthorStatus { get; set; }

        public string Text { get; set; }

        public int UsersFavouriteCount { get; set; }

        public int RepliesCount { get; set; }

        public int RetweetsCount { get; set; }

        public List<Reply> ReplyList { get; set; } 

        public DateTime DatePosted { get; set; }
    }
}