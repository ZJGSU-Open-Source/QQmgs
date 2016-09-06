﻿namespace Twitter.App.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTweetBindingModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(250)]
        public string Text { get; set; }

        public int GroupId { get; set; }
    }
}