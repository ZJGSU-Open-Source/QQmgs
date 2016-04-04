namespace Twitter.App.Models.BindingModels
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