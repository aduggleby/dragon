using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Dragon.SecurityServer.Identity.Stores;
using Dragon.SecurityServer.ProfileSTS.Client;
using Dragon.SecurityServer.ProfileSTS.Models;

namespace Dragon.SecurityServer.ProfileSTS.Controllers
{
    public class ProfileApiController : ApiController
    {
        private readonly IDragonUserStore<AppMember> _userStore;
        private readonly ApplicationUserManager _userManager;

        public ProfileApiController(IDragonUserStore<AppMember> userStore, ApplicationUserManager userManager)
        {
            _userStore = userStore;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IHttpActionResult> ClearCache()
        {
            await _userStore.ClearCache();
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddClaim([FromBody] AddClaimModel model)
        {
            if (model.UserId == null) return NotFound();
            var user  = await _userStore.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            await _userStore.AddClaimAsync(user, new Claim(model.Type, model.Value));
            return Ok();
        }

        [HttpGet]
        public async Task<IList<Claim>> GetClaims(string userid)
        {
            var user  = await _userStore.FindByIdAsync(userid);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return await _userStore.GetClaimsAsync(user);
        }
    }
}