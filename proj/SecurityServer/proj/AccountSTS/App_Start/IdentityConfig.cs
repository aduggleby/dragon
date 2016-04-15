using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dragon.SecurityServer.AccountSTS.App_Start;
using Dragon.SecurityServer.AccountSTS.Helpers;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;

namespace Dragon.SecurityServer.AccountSTS
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var client = new SmtpClient();

            var @from = new MailAddress(WebConfigurationManager.AppSettings["MailSenderEmail"], WebConfigurationManager.AppSettings["MailSenderName"]);
            var to = new MailAddress(message.Destination);

            var mail = new MailMessage(@from, to)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true,
            };

            client.Send(mail);
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
    public class ApplicationUserManager : UserManager<AppMember>
    {
        public ApplicationUserManager(IUserStore<AppMember> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(Container container, IDataProtectionProvider dataProtectionProvider) 
        {
            var manager = new ApplicationUserManager(container.GetInstance<IDragonUserStore<AppMember>>());

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<AppMember>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            /*
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<AppMember>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<AppMember>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            */
            manager.EmailService = new EmailService();
            // manager.SmsService = new SmsService();
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<AppMember>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<AppMember, string>
    {
        private readonly IDragonUserStore<AppMember> _userStore;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager, IDragonUserStore<AppMember> userStore )
            : base(userManager, authenticationManager)
        {
            _userStore = userStore;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppMember user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        // Customized to make the method service aware
        public new async Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (!await IsUserRegisteredForService(user, RequestHelper.GetCurrentServiceId()))
            {
                // throw until using Identity where the status is more flexible, see https://github.com/aspnet/Identity/issues/176
                throw new NotRegisteredForServiceException();
            }

            return await base.ExternalSignInAsync(loginInfo, isPersistent);
        }

        private async Task<bool> IsUserRegisteredForService(AppMember user, string currentServiceId)
        {
            return user == null || await _userStore.IsUserRegisteredForServiceAsync(user, currentServiceId); // user == null on regstration
        }

        // Customized to make the method service aware
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var status = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if (status == SignInStatus.Success)
            {
                if (!await IsUserRegisteredForService(await UserManager.FindByNameAsync(userName), RequestHelper.GetCurrentServiceId()))
                {
                    // throw until using Identity where the status is more flexible, see https://github.com/aspnet/Identity/issues/176
                    throw new NotRegisteredForServiceException();
                }
            }
            return status;
        }
    }
}
