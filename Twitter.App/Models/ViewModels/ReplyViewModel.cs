using System.ComponentModel.DataAnnotations;

namespace Twitter.App.Models.ViewModels
{
    using System;

    public class ReplyViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} should be no more than {1} charaters and no less than {2} charaters.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        public string Text { get; set; }

        public DateTime PublishTime { get; set; }
    }
}