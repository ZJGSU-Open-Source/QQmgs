namespace Twitter.App.Models.ViewModels
{
    using System;

    public class ReplyViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime PublishTime { get; set; }
    }
}