using Twitter.App.BusinessLogic;
using Twitter.Data.UnitOfWork;

namespace Twitter.App.Controllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Twitter.App.Helper;
    using Twitter.App.Models.ViewModels;
    using Twitter.Models;

    [Authorize]
    public class AccountController : TwitterBaseController
    {
        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;

        public AccountController() : base(new QQmgsData())
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) : base(new QQmgsData())
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return this._signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set
            {
                this._signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this._userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this._userManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            return this.View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var userLoginTrace = new UserLogTrace
            {
                DatePosted = DateTime.Now,
                IpAddress = IPAddressHelper.GetIpAddress(),
                IsLoggedSucceeded = false,
                PhoneNumber = model.UserName ?? "00000000000"
            };

            if (!this.ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(state => state.Errors).ToList();
                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                this.Data.UserLogTrace.Add(userLoginTrace);
                this.Data.SaveChanges();

                return this.View(model);
            }

            // transform string value to bool value
            var isRememberMe = string.Compare(model.RememberMe, "on", StringComparison.OrdinalIgnoreCase) == 0;

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result =
                await this.SignInManager.PasswordSignInAsync(model.UserName, model.Password, isRememberMe, false);

            if (result == SignInStatus.Success)
            {
                userLoginTrace.IsLoggedSucceeded = true;
            }

            this.Data.UserLogTrace.Add(userLoginTrace);
            this.Data.SaveChanges();

            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, Resources.Resources.InvalidLoginAttempt);
                    return this.View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await this.SignInManager.HasBeenVerifiedAsync())
            {
                return this.View("Error");
            }

            return
                this.View(
                    new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result =
                await
                this.SignInManager.TwoFactorSignInAsync(
                    model.Provider,
                    model.Code,
                    model.RememberMe,
                    model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    this.ModelState.AddModelError(string.Empty, "Invalid code.");
                    return this.View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (RedirectToHomepage())
            {
                return RedirectToAction("Index", "Group");
            }

            var model = new RegisterViewModel();

            return this.View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(state => state.Errors).ToList();
                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                return this.View(model);
            }

            // Resolve locale
            var currentCulture = model.Locale == null ? "zh-cn" : "en-us";
            var culture = CultureHelper.GetImplementedCulture(currentCulture);

            // Save culture in a cookie
            var cookie = Request.Cookies["_culture"];

            if (cookie != null)
            {
                cookie.Value = culture; // update cookie value
            }
            else
            {
                cookie = new HttpCookie("_culture")
                {
                    Value = culture,
                    Expires = DateTime.Now.AddYears(1)
                };
            }

            Response.Cookies.Add(cookie);

            // create new user
            var user = new User
            {
                UserName = model.PhoneNumber,
                RealName = model.RealName,
                Email = model.Email,
                RegisteredTime = DateTime.Now,
                Class = model.Class
            };

            var result = await this.UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await this.SignInManager.SignInAsync(user, false, false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url?.Scheme);

                await UserManager.SendEmailAsync(user.Id, "全球某工商 注册确认【系统邮件】", callbackUrl);

                // return View("DisplayEmail");
                return this.RedirectToAction("Question", "Home");
            }

            this.AddErrors(result);

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this.UserManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.UserManager.FindByNameAsync(model.UserName);
                if (user == null || !(await this.UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url?.Scheme);
                await UserManager.SendEmailAsync(user.Id, "全球某工商忘记密码【系统邮件】", callbackUrl);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? this.View("Error") : this.View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.View("ForgotPasswordConfirmation");
            }

            var result = await this.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(
                provider,
                this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await this.SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return this.View("Error");
            }

            var userFactors = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions =
                userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return
                this.View(
                    new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            // Generate the token and send it
            if (!await this.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return this.View("Error");
            }

            return this.RedirectToAction(
                "VerifyCode",
                new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this.SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:

                    // If the user does not have an account, then prompt the user to create an account
                    this.ViewBag.ReturnUrl = returnUrl;
                    this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View(
                        "ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(
            ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await this.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await this.SignInManager.SignInAsync(user, false, false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return this.RedirectToAction("Login", "Account");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            var cookie = Request.Cookies["_culture"];

            if (cookie != null)
            {
                cookie.Value = culture; // update cookie value
            }
            else
            {
                cookie = new HttpCookie("_culture")
                {
                    Value = culture,
                    Expires = DateTime.Now.AddYears(1)
                };
            }

            Response.Cookies.Add(cookie);

            return RedirectToAction("Register");
        }

        [AllowAnonymous]
        public ActionResult GetDailyUserLogins()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentDay = DateTime.Now.Day;
            var loginCounts =
                this.Data.UserLogTrace.All()
                    .Count(
                        login =>
                            login.DatePosted.Year == currentYear && login.DatePosted.Month == currentMonth &&
                            login.DatePosted.Day == currentDay);

            return PartialView(loginCounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._userManager != null)
                {
                    this._userManager.Dispose();
                    this._userManager = null;
                }

                if (this._signInManager != null)
                {
                    this._signInManager.Dispose();
                    this._signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }

        private bool RedirectToHomepage()
        {
            return this.User.Identity.IsAuthenticated;
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            public string LoginProvider { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
                if (this.UserId != null)
                {
                    properties.Dictionary[XsrfKey] = this.UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
            }
        }

        #endregion
    }
}