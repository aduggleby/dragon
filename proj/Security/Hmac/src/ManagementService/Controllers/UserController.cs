using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Modules;
using Dragon.Security.Hmac.Module.Repositories;

namespace ManagementService.Controllers
{
    public class UserController : ApiController
    {
        public IUserRepository UserRepository;
        private SqlConnection _connection;

        public UserController()
        {
            InitDefaults();
        }

        private void InitDefaults()
        {
            const string hmacSectionName = "dragon/security/hmac";
            var settings = (DragonSecurityHmacSection) ConfigurationManager.GetSection(hmacSectionName);
            if (settings == null)
            {
                throw new HmacInvalidConfigException(String.Format("Section {0} is missing.", hmacSectionName));
            }
            var connectionStringName = settings.ConnectionStringName;
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
            {
                throw new HmacInvalidConfigException(String.Format("Connection string named {0} is missing.",
                    connectionStringName));
            }
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var usersTableName = settings.UsersTableName;
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            UserRepository = new DapperUserRepository(_connection, usersTableName);
        }

        [NonAction]
        public new void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            base.Dispose();
        }

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
