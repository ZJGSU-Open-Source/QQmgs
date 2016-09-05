using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Twitter.App
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security;
    using SendGrid;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Twitter.Data;
    using Twitter.Models;

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return ConfigMailSendAsync(message);
        }

        private static Task ConfigMailSendAsync(IdentityMessage message)
        {
            #region formatter
                var text = $"Please click on this link to {message.Subject}: {message.Body}";

            //var html = "Hi,<br/>";
            //html += "请点击此链接确认这是您的邮箱 <a href=\"" + message.Body + "\">here</a><br/><br/>";
            //html += HttpUtility.HtmlEncode("或者复制以下链接至您的浏览器中打开:\n\n" + message.Body) + "\n";
            //html += "Cheers,<br/>Engineering team of QQmgs.com<br/>";

            var html = $@"<!DOCTYPE html><html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""></head><body><tbody><tr><td id=""x_body"" style=""border-collapse:collapse""><table width=""640"" border=""0"" cellspacing=""0"" cellpadding=""0""><tbody><tr id=""x_content""><td align=""left"" valign=""top"" id=""x_activate_account"" style=""border-collapse:collapse; padding:20px 0 20px 0""><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" style=""background-color:#f7f8fa; font-family:&quot;Helvetica Neue&quot;,Helvetica Neue,HelveticaNeue,Helvetica,Arial,sans-serif; margin-top:0""><tbody><tr><td align=""center"" bgcolor=""#f7f8fa"" style=""border-collapse:collapse""><table width=""640"" cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" style=""width:auto; max-width:640px; margin:0""><tbody><tr><td style=""border-collapse:collapse""><table style=""width:100%; padding:0 10px""><tbody><tr><td border=""0"" width=""60"" style=""border-collapse:collapse; padding:10px 0; height:31px; width:65px""><a href=""http://www.qqmgs.com"" target=""_blank"" style=""text-decoration:none; color:#02ADE2; border:0; outline:none"">QQmgs.com</a></td><td align=""right"" style=""border-collapse:collapse; color:#71767a; width:92%; padding-left:8%; text-align:right; font-size:14px""><span></span></td></tr></tbody></table></td></tr><tr><td style=""border-collapse:collapse""><table id=""x_box"" width=""638"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""background:#ffffff; -webkit-border-radius:5px; border:#dce4ea 1px solid; width:100%; border-radius:0px; margin:0; border-right:none; border-left:none""><tbody><tr><td style=""border-collapse:collapse; width:10px""></td><td style=""border-collapse:collapse; margin:auto; text-align:center""><h1 style=""color:#525558; margin:1.5em 20px; font-weight:300; font-size:24px; line-height:120%!important"">很高兴遇见你! </h1><p style=""color:#525558; font-size:18px; line-height:26px"">欢迎注册全球某工商! <br>注册邮箱可以帮助用户在遗忘密码的时候重置密码</p><br><table align=""center"" cellspacing=""0"" border=""0"" cellpadding=""7"" style=""border-radius:3px; font-size:18px; font-weight:normal; color:#ffffff; text-decoration:none; background-color:#02adea; border:1px solid #0099e5; margin:auto; text-align:center""><tbody><tr><td style=""border-collapse:collapse""></td><td style=""border-collapse:collapse""><a href=""{HttpUtility.HtmlEncode(message.Body)}"" target=""_blank"" style=""text-decoration:none; border:0; outline:none; color:#fff!important"">确认注册邮箱</a> </td><td style=""border-collapse:collapse""></td></tr></tbody></table><br><br><p style=""color:#525558; line-height:20px; font-size:14px"">Thanks,<br>全球某工商.com - 连接每一位商大师生</p></td><td style=""border-collapse:collapse; width:10px""></td></tr><tr><td colspan=""3"" style=""border-collapse:collapse; height:20px""></td></tr></tbody></table></td></tr><tr><td align=""center"" style=""border-collapse:collapse; text-align:center; padding:30px 0""><h3 style=""color:#525558; font-weight:normal; line-height:120%!important"">Happy CHATTING!</h3><p style=""color:#525558; line-height:20px; font-size:14px"">&nbsp;QQmgs.com, connecting all the students and teachers.<br>You&rsquo;re receiving this e-mail because you&rsquo;re signed up to receive communications from&nbsp;QQmgs.com. </p></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></body>";
            #endregion

            var msg = new MailMessage { From = new MailAddress(ConfigurationManager.AppSettings["mailAccount"]) };
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            var smtpClient = new SmtpClient("smtp.ym.163.com", Convert.ToInt32(25));
            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                    ex.ToString());
            }

            return Task.FromResult(0);
        }

        public Task SendCustomizedMailAsync(string subject, string body, string destination)
        {
            var msg = new MailMessage { From = new MailAddress("Admin@qqmgs.com") };
            msg.To.Add(new MailAddress(destination));
            msg.Subject = subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

            var smtpClient = new SmtpClient("smtp.ym.163.com", Convert.ToInt32(25));
            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                    ex.ToString());
            }

            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options, 
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<User>(context.Get<QQmgsDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
                                        {
                                            AllowOnlyAlphanumericUserNames = false, 
                                            RequireUniqueEmail = true
                                        };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
                                            {
                                                RequiredLength = 4, 
                                                RequireNonLetterOrDigit = false, 
                                                RequireDigit = false, 
                                                RequireLowercase = false, 
                                                RequireUppercase = false
                                            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider(
                "Phone Code", 
                new PhoneNumberTokenProvider<User> { MessageFormat = "Your security code is {0}" });
            manager.RegisterTwoFactorProvider(
                "Email Code", 
                new EmailTokenProvider<User> { Subject = "Security Code", BodyFormat = "Your security code is {0}" });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<User, string>
    {
        public ApplicationSignInManager(
            ApplicationUserManager userManager, 
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)this.UserManager);
        }

        public static ApplicationSignInManager Create(
            IdentityFactoryOptions<ApplicationSignInManager> options, 
            IOwinContext context)
        {
            return new ApplicationSignInManager(
                context.GetUserManager<ApplicationUserManager>(), 
                context.Authentication);
        }
    }
}