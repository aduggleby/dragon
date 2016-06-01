using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Http;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;

namespace ManagementService.Controllers
{
    public class AppController : ApiController
    {
        [Import]
        public IAppRepository AppRepository { get; set; }

        // GET: api/App
        public IEnumerable<AppModel> Get()
        {
            return AppRepository.GetAll();
        }

        // GET: api/App/5
        public AppModel Get(int id)
        {
            return AppRepository.Get(id);
        }

        // POST: api/App
        [HttpPost]
        public int Post([FromBody]AppModel value)
        {
            return AppRepository.Insert(value);
        }

        // PUT: api/App/5
        public void Put(int id, [FromBody]AppModel value)
        {
            AppRepository.Update(id, value);
        }

        // DELETE: api/App/5
        public void Delete(int id)
        {
            AppRepository.Delete(id);
        }
    }
}
