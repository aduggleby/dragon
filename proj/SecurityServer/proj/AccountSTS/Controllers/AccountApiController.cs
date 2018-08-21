using System.Threading.Tasks;
using System.Web.Http;
using Dragon.SecurityServer.AccountSTS.Attributes;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;

namespace Dragon.SecurityServer.AccountSTS.Controllers
{
    [ProviderRestriction]
    public class AccountApiController : ApiController
    {
        private readonly IDragonUserStore<AppMember> _userStore;
        private readonly ApplicationUserManager _userManager;

        public AccountApiController(IDragonUserStore<AppMember> userStore, ApplicationUserManager userManager)
        {
            _userStore = userStore;
            _userManager = userManager;
        }

        [HttpGet]
        public IHttpActionResult ClearCache()
        {
            _userStore.ClearCache();
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update([FromBody] UpdateViewModel model)
        {
            var user = await _userStore.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            if (!model.Email.IsEmpty())
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                //user.EmailConfirmed = false; // TODO: resend email confirmation email on change?
            }
            if (!model.Password.IsEmpty())
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(model.Password);
            }
            await _userStore.UpdateAsync(user);
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            // ReSharper disable once UnusedVariable
            var updateSecurityStampResult = await _userManager.UpdateSecurityStampAsync(user.Id);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            // Note: This fails if no security stamp is set!
            var result = await _userManager.ResetPasswordAsync(user.Id, code, model.Password);
            return result.Succeeded ? (IHttpActionResult) Ok() : InternalServerError();
        }
    }
}