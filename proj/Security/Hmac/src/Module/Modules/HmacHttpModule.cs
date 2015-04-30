using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services;

namespace Dragon.Security.Hmac.Module.Modules
{
    public class HmacHttpModule : IHttpModule
    {
        public IHmacHttpService HmacHttpService { get; set; }
        private IDbConnection _connection;

        public void Init(HttpApplication context)
        {
            if (HmacHttpService == null) HmacHttpService = CreateDefaultHmacService();

            context.BeginRequest += BeginRequestHandler;
        }

        public void BeginRequestHandler(object sender, EventArgs eventArgs)
        {
            var application = (HttpApplication)sender;
            var applicationContext = application.Context;

            var statusCode = HmacHttpService.IsRequestAuthorized(application.Request.RawUrl,
                applicationContext.Request.QueryString);
            if (statusCode == StatusCode.Authorized)
            {
                return;
            }
            Trace.WriteLine(String.Format("Request to '{1}' denied, reason: {0}", statusCode, application.Request.RawUrl));
            Trace.Flush();
            application.Context.Response.StatusCode = 401;
            application.Context.Response.SuppressContent = true;
            application.Context.Response.End();
        }

        private HmacHttpService CreateDefaultHmacService()
        {
            const string hmacSectionName = "dragon/security/hmac";
            var settings = (DragonSecurityHmacSection)ConfigurationManager.GetSection(hmacSectionName);
            if (settings == null)
            {
                throw new HmacInvalidConfigException(String.Format("Section {0} is missing.", hmacSectionName));
            }
            var connectionStringName = settings.ConnectionStringName;
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
            {
                throw new HmacInvalidConfigException(String.Format("Connection string named {0} is missing.", connectionStringName));
            }
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var userTableName = settings.UsersTableName;
            var applicationTableName = settings.AppsTableName;
            var serviceId = settings.ServiceId;
            var signatureParameterKey = settings.SignatureParameterKey;

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            return new HmacHttpService(serviceId, settings.Paths, signatureParameterKey)
            {
                UserRepository = new DapperUserRepository(_connection, userTableName),
                AppRepository = new DapperAppRepository(_connection, applicationTableName),
                HmacService = new HmacSha256Service { SignatureParameterKey = signatureParameterKey }
            };
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}