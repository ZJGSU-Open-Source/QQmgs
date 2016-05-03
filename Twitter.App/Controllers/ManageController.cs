using System;
using Twitter.App.BusinessLogic;
using Twitter.App.Helper;
using Twitter.Data.UnitOfWork;
using Twitter.Models;

namespace Twitter.App.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using Twitter.App.Models.ViewModels;

    [Authorize]
    public class ManageController : TwitterBaseController
    {
        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;

        public ManageController() : base(new TwitterData())
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) : base(new TwitterData())
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

        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage = message == ManageMessageId.ChangePasswordSuccess
                                             ? "Your password has been changed."
                                             : message == ManageMessageId.SetPasswordSuccess
                                                   ? "Your password has been set."
                                                   : message == ManageMessageId.SetTwoFactorSuccess
                                                         ? "Your two-factor authentication provider has been set."
                                                         : message == ManageMessageId.Error
                                                               ? "An error has occurred."
                                                               : message == ManageMessageId.AddPhoneSuccess
                                                                     ? "Your phone number was added."
                                                                     : message == ManageMessageId.RemovePhoneSuccess
                                                                           ? "Your phone number was removed."
                                                                           : string.Empty;

            var userId = this.User.Identity.GetUserId();
            var registeredTime = this.Data.Users.Find(userId).RegisteredTime;
            var registerTimeInterval = (DateTime.Now - registeredTime).Days;

            var user = this.Data.Users.Find(userId);

            var model = new IndexViewModel
                            {
                                HasPassword = this.HasPassword(), 
                                PhoneNumber = await this.UserManager.GetPhoneNumberAsync(userId), 
                                TwoFactor = await this.UserManager.GetTwoFactorEnabledAsync(userId), 
                                Logins = await this.UserManager.GetLoginsAsync(userId), 
                                BrowserRemembered =
                                    await
                                    this.AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                                RegisteredTime = registeredTime,
                                RegisterTimeInterval = registerTimeInterval,
                                HasAvatarImage = user.HasAvatarImage,
                                AvatarImageName = user.AvatarImageName
            };

            return this.View(model);
        }

        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result =
                await
                this.UserManager.RemoveLoginAsync(
                    this.User.Identity.GetUserId(), 
                    new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, false, false);
                }

                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return this.RedirectToAction("ManageLogins", new { Message = message });
        }
        
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await this.UserManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), true);
            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await this.UserManager.SetTwoFactorEnabledAsync(this.User.Identity.GetUserId(), false);
            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", "Manage");
        }

        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code =
                await this.UserManager.GenerateChangePhoneNumberTokenAsync(this.User.Identity.GetUserId(), phoneNumber);

            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null
                       ? this.View("Error")
                       : this.View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this.UserManager.ChangePhoneNumberAsync(this.User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, false, false);
                }

                return this.RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }

            // If we got this far, something failed, redisplay form
            this.ModelState.AddModelError(string.Empty, "Failed to verify phone");
            return this.View(model);
        }

        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await this.UserManager.SetPhoneNumberAsync(this.User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return this.RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return this.View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result =
                await
                this.UserManager.ChangePasswordAsync(
                    this.User.Identity.GetUserId(), 
                    model.OldPassword, 
                    model.NewPassword);
            if (result.Succeeded)
            {
                var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
                if (user != null)
                {
                    await this.SignInManager.SignInAsync(user, false, false);
                }

                return this.RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            this.AddErrors(result);
            return this.View(model);
        }

        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return this.View();
        }

        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.UserManager.AddPasswordAsync(this.User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
                    if (user != null)
                    {
                        await this.SignInManager.SignInAsync(user, false, false);
                    }

                    return this.RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage = message == ManageMessageId.RemoveLoginSuccess
                                             ? "The external login was removed."
                                             : message == ManageMessageId.Error ? "An error has occurred." : string.Empty;
            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user == null)
            {
                return this.View("Error");
            }

            var userLogins = await this.UserManager.GetLoginsAsync(this.User.Identity.GetUserId());
            var otherLogins =
                this.AuthenticationManager.GetExternalAuthenticationTypes()
                    .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider))
                    .ToList();
            this.ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return this.View(new ManageLoginsViewModel { CurrentLogins = userLogins, OtherLogins = otherLogins });
        }

        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(
                provider, 
                this.Url.Action("LinkLoginCallback", "Manage"), 
                this.User.Identity.GetUserId());
        }

        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo =
                await this.AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, this.User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }

            var result = await this.UserManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded
                       ? this.RedirectToAction("ManageLogins")
                       : this.RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }


        // GET: /Manage/ChangeStatus
        public ActionResult ChangeStatus()
        {
            var loggedUserId = this.User.Identity.GetUserId();
            ViewBag.oldStatus = Data.Users.Find(loggedUserId).Status; 

            return this.View();
        }

        // POST: /Manage/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeStatus(ChangeStatusViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var loggedUserId = this.User.Identity.GetUserId();
            
            Data.Users.Find(loggedUserId).Status = model.Status;
            Data.SaveChanges();

            var user = await this.UserManager.FindByIdAsync(this.User.Identity.GetUserId());
            if (user != null)
            {
                await this.SignInManager.SignInAsync(user, false, false);
            }

            return this.RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult UploadAvatarImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatarImage(HttpPostedFileBase file)
        {
            var uploadedFile = FileUploadHelper.UploadFile(file);
            var loggedUserId = this.User.Identity.GetUserId();

            var photo = new Photo
            {
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                Name = uploadedFile,
                IsAvatarImage = true
            };

            this.Data.Photo.Add(photo);
            this.Data.SaveChanges();

            var user = this.Data.Users.Find(loggedUserId);
            user.HasAvatarImage = true;
            user.AvatarImageName = photo.Name;
            this.Data.Users.Update(user);
            this.Data.SaveChanges();

            return RedirectToAction("Index");
        }


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

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this._userManager != null)
            {
                this._userManager.Dispose();
                this._userManager = null;
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

        private bool HasPassword()
        {
            var user = this.UserManager.FindById(this.User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }

            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = this.UserManager.FindById(this.User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }

            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess, 

            ChangePasswordSuccess, 

            SetTwoFactorSuccess, 

            SetPasswordSuccess, 

            RemoveLoginSuccess, 

            RemovePhoneSuccess, 

            Error
        }

        #endregion
    }
}