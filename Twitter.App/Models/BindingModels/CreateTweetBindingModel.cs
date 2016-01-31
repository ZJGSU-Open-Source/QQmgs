namespace Twitter.App.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTweetBindingModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string Text { get; set; }
    }
}