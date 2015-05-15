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
    public class AppController : ApiController
    {
        public IAppRepository AppRepository;
        private SqlConnection _connection;

        public AppController()
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
            var applicationTableName = settings.AppsTableName;
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            AppRepository = new DapperAppRepository(_connection, applicationTableName);
        }

        [NonAction]
        public new void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            base.Dispose();
        }

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
