namespace Twitter.App.Models.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class ReplyViewModel
    {
        public List<SingleReply> Replies { get; set; }
    }

    public class SingleReply
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime PublishTime { get; set; }
    }
}