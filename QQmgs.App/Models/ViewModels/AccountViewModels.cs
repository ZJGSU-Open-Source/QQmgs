using Glimpse.Core.Extensions;

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
            ErrorMessageResourceName = "LoginPhoneNumberRequired")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Resources.Resources))]
        [StringLength(11, ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PhoneNumberLong", MinimumLength = 11)]
        [RegularExpression(@"^[^ ]+$", ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "PasswordLong", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public string RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "LoginPhoneNumberRequired")]
        [Display(Name = "PhoneNumber", ResourceType = typeof (Resources.Resources))]
        [StringLength(11, ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "PhoneNumberLong", MinimumLength = 11)]
        [RegularExpression(@"^[^ ]+$", ErrorMessageResourceType = typeof(Resources.Resources), 
            ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "UserNameRequired")]
        [Display(Name = "RealName", ResourceType = typeof(Resources.Resources))]
        [StringLength(10, ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "UserNameLong", MinimumLength = 2)]
        [RegularExpression(@"^[^ ]+$", ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "InvalidUserName")]
        public string RealName { get; set; }

        //[Required(ErrorMessageResourceType = typeof (Resources.Resources),
        //    ErrorMessageResourceName = "EmailRequired")]
        //[EmailAddress]
        //[Display(Name = "Email", ResourceType = typeof (Resources.Resources))]
        //public string Email { get; set; }

        //[Required(ErrorMessageResourceType = typeof (Resources.Resources),
        //    ErrorMessageResourceName = "ClassRequired")]
        //[StringLength(30, ErrorMessageResourceType = typeof (Resources.Resources),
        //    ErrorMessageResourceName = "ClassLong", MinimumLength = 1)]
        //[Display(Name = "ClassOrDeparment", ResourceType = typeof (Resources.Resources))]
        //public string Class { get; set; }

        [Required(ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "PasswordLong", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof (Resources.Resources))]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "ConfirmPassword", ResourceType = typeof (Resources.Resources))]
        //[System.ComponentModel.DataAnnotations.Compare("Password",
        //    ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        [Display(Name = @"Locale")]
        public string Locale { get; set; }
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
        [Display(ResourceType = typeof(Resources.Resources), Name = "Phone_Number")]
        public string UserName { get; set; }
    }


    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }


    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }


    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }
}