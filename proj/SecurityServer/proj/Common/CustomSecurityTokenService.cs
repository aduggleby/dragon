using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Security.Claims;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace Dragon.SecurityServer.Common
{
    public class CustomSecurityTokenService<T> : SecurityTokenService where T: class, IUser, new()
    {
        private readonly string _loginProviderName;
        private readonly IDragonUserStore<T> _userStore;

        public CustomSecurityTokenService(string loginProviderName, SecurityTokenServiceConfiguration securityTokenServiceConfiguration, IDragonUserStore<T> userStore)
            : base(securityTokenServiceConfiguration)
        {
            _loginProviderName = loginProviderName;
            _userStore = userStore;
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            return new Scope(request.AppliesTo.Uri.OriginalString,
                SecurityTokenServiceConfiguration.SigningCredentials) {ReplyToAddress = request.ReplyTo, TokenEncryptionRequired = false};
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (null == principal)
            {
                throw new InvalidRequestException("The caller's principal is null.");
            }

            var userId = principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var user = AsyncRunner.RunNoSynchronizationContext(() =>_userStore.FindByIdAsync(userId));
            if (user == null)
            {
                _userStore.CreateAsync(new T { Id = userId });
                user = AsyncRunner.RunNoSynchronizationContext(() => _userStore.FindByIdAsync(userId));
                _userStore.AddLoginAsync(user, new UserLoginInfo(_loginProviderName, userId));
            }

            var callerIdentity = (ClaimsIdentity) principal.Identity;
            var identity = callerIdentity.Clone();
            var authenticationmethod = "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod"; // this one triggers: The 'AuthenticationInstant' used to create a 'SAML11' AuthenticationStatement cannot be null. Thereforme it is removed.
            foreach (var claim in identity.Claims.Where(claim => claim.Type == authenticationmethod))
            {
                identity.RemoveClaim(claim);
            }
            var claims = AsyncRunner.RunNoSynchronizationContext(() =>(_userStore.GetClaimsAsync(user)));
            identity.AddClaims(claims);
            return identity;
        }
    }
}