using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Twitter.App.Models.BindingModel
{
    public class UploadPhotoBindingModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }

        [MaxLength(30)]
        public string Description { get; set; }

        public string Classcification { get; set; }
    }
}