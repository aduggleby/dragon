using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;

namespace ManagementService.Controllers
{
    public class UserController : ApiController
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        // GET: api/App
        public IEnumerable<UserModel> Get()
        {
            return UserRepository.GetAll();
        }

        // GET: api/App/5
        public UserModel Get(long id)
        {
            return UserRepository.Get(id);
        }

        // POST: api/App
        [HttpPost]
        public long Post([FromBody]UserModel value)
        {
            return UserRepository.Insert(value);
        }

        // PUT: api/App/5
        public void Put(long id, [FromBody]UserModel value)
        {
            UserRepository.Update(id, value);
        }

        // DELETE: api/App/5
        public void Delete(long id)
        {
            UserRepository.Delete(id);
        }
    }
}
