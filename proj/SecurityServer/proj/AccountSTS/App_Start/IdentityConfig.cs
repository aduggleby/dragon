using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dragon.Data.Interfaces;
using Dragon.SecurityServer.AccountSTS.Controllers;
using Dragon.SecurityServer.AccountSTS.Helpers;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.AccountSTS.Services;
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
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
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
        private readonly IRepository<UserActivity> _userActivityRepository;
        private readonly IAppService _appService;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager, IDragonUserStore<AppMember> userStore, IRepository<UserActivity> userActivityRepository, IAppService appService)
            : base(userManager, authenticationManager)
        {
            _userStore = userStore;
            _userActivityRepository = userActivityRepository;
            _appService = appService;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppMember user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public override async Task SignInAsync(AppMember user, bool isPersistent, bool rememberBrowser)
        {
            await base.SignInAsync(user, isPersistent, rememberBrowser);
            await PostSignIn(user);
        }

        private async Task PostSignIn(AppMember user)
        {
            var appId = RequestHelper.GetCurrentAppId();
            if (_appService.GetOtherRegisteredAppsInSameGroup(Guid.Parse(user.Id), Guid.Parse(appId)).Any())
            {
                throw new AppNotAllowedException();
            }
            // Only multiple apps per user and group are not allowed, automatically connect services and apps to the user otherwise.
            // The service and app ids are validated by Dragon.Security.Hmac
            await AddCurrentServiceIdToUserIfNotAlreadyAdded(user);
            await AddCurrentAppIdToUserIfNotAlreadyAdded(user);
            AddActivity(user);
        }

        private void AddActivity(AppMember user)
        {
            if (user == null)
            {
                return;
            }
            _userActivityRepository.Insert(new UserActivity
            {
                AppId = RequestHelper.GetCurrentAppId(),
                ServiceId = RequestHelper.GetCurrentServiceId(),
                DateTime = DateTime.UtcNow,
                Type = "Login",
                UserId = user.Id
            });
        }

        private async Task AddCurrentServiceIdToUserIfNotAlreadyAdded(AppMember user)
        {
            var currentServiceId = RequestHelper.GetCurrentServiceId();
            if (user != null && !await _userStore.IsUserRegisteredForServiceAsync(user, currentServiceId)) // user == null on registration
            {
                await _userStore.AddServiceToUserAsync(user, currentServiceId);
            }
        }

        private async Task AddCurrentAppIdToUserIfNotAlreadyAdded(AppMember user)
        {
            var currentAppId = RequestHelper.GetCurrentAppId();
            if (user != null && !await _userStore.IsUserRegisteredForAppAsync(user, currentAppId)) // user == null on registration
            {
                await _userStore.AddAppToUserAsync(user, currentAppId);
            }
        }
    }
}
