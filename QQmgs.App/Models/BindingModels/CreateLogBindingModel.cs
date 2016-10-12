namespace Twitter.App.Models.BindingModel
{
    using System.ComponentModel.DataAnnotations;

    public class CreateLogBindingModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Log { get; set; }
    }
}