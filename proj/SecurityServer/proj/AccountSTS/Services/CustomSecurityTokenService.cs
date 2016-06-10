using System;
using System.Configuration;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public class CustomSecurityTokenService : SecurityTokenService
    {
        private readonly EncryptingCredentials _encryptingCredentials;
        private readonly IDragonUserStore<AppMember> _userStore;

        public CustomSecurityTokenService(SecurityTokenServiceConfiguration securityTokenServiceConfiguration, EncryptingCredentials encryptingCredentials, IDragonUserStore<AppMember> userStore)
            : base(securityTokenServiceConfiguration)
        {
            _encryptingCredentials = encryptingCredentials;
            _userStore = userStore;
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials)
            {
                ReplyToAddress = request.ReplyTo,
                TokenEncryptionRequired = false,
            };
            if (_encryptingCredentials != null)
            {
                scope.EncryptingCredentials = _encryptingCredentials;
            }
            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            var id = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var availableProviders =
                ConfigurationManager.AppSettings["AuthenticationProviders"].Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            var user = AsyncRunner.RunNoSynchronizationContext(() => _userStore.FindByIdAsync(id));
            var connectedProviders = AsyncRunner.RunNoSynchronizationContext(() => _userStore.GetLoginsAsync(user)).Select(x => x.LoginProvider).ToList();
            var loginClaims = connectedProviders.Select(x => new Claim(Consts.ManagementConnectedAccountType, x)).Concat(
                availableProviders.Except(connectedProviders).Select(x => new Claim(Consts.ManagementDisconnectedAccountType, x)));
            var serviceClaims = AsyncRunner.RunNoSynchronizationContext(() => _userStore.GetServicesAsync(user)).Select(x => new Claim(Consts.RegisteredServiceType, x));
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, id),
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, principal.Identity.Name),
                new Claim(Consts.ManagementUrlType, System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path) + "?action=[action]&data=[data]"), 
            };
            return new ClaimsIdentity(claims.Concat(loginClaims).Concat(serviceClaims));
        }
    }
}