namespace Twitter.App.Models.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "LoginUserNameRequired")]
        [Display(Name = "UserName", ResourceType = typeof(Resources.Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "UserNameLong", MinimumLength = 2)]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordLong", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "UserNameRequired")]
        [Display(Name = "UserName", ResourceType = typeof (Resources.Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "UserNameLong", MinimumLength = 2)]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof (Resources.Resources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "ClassRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "ClassLong", MinimumLength = 1)]
        [Display(Name = "ClassOrDeparment", ResourceType = typeof (Resources.Resources))]
        public string Class { get; set; }

        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "PasswordLong", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof (Resources.Resources))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof (Resources.Resources))]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", 
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }
}