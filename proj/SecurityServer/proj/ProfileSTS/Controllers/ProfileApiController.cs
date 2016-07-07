using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Dragon.SecurityServer.Identity.Stores;
using Dragon.SecurityServer.ProfileSTS.Models;
using Dragon.SecurityServer.ProfileSTS.Shared.Models;

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
            var claims = (await _userStore.GetClaimsAsync(user)).Where(x => x.Type == model.Type).ToList();
            if (claims.Any())
            {
                throw new ArgumentException("Claim already exists.");
            }
            await _userStore.AddClaimAsync(user, new Claim(model.Type, model.Value));
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateClaim([FromBody] UpdateClaimModel model)
        {
            if (model.UserId == null) return NotFound();
            var user = await _userStore.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var claims = (await _userStore.GetClaimsAsync(user)).Where(x => x.Type == model.Type).ToList();
            if (!claims.Any()) return NotFound();
            if (claims.Count > 1) return InternalServerError();
            await _userStore.RemoveClaimAsync(user, claims.First());
            await _userStore.AddClaimAsync(user, new Claim(model.Type, model.Value));
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddOrUpdateClaims([FromBody] AddOrUpdateClaimsModel model)
        {
            if (model.UserId == null) return NotFound();
            var user = await _userStore.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var claims = (await _userStore.GetClaimsAsync(user)).ToList();
            foreach (var claim in model.Claims)
            {
                foreach (var existingClaim in claims.Where(x => x.Type == claim.Type).ToList())
                {
                    await _userStore.RemoveClaimAsync(user, existingClaim);
                }
                await _userStore.AddClaimAsync(user, claim);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> RemoveClaim([FromBody] RemoveClaimModel model)
        {
            if (model.UserId == null) return NotFound();
            var user = await _userStore.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            var claims = (await _userStore.GetClaimsAsync(user)).Where(x => x.Type == model.Type).ToList();
            if (!claims.Any()) return NotFound();
            if (claims.Count > 1) return InternalServerError();
            await _userStore.RemoveClaimAsync(user, claims.First());
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