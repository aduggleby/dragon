using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.Security.Hmac.Module.Configuration;
using Dragon.Security.Hmac.Module.Repositories;
using Dragon.Security.Hmac.Module.Services;
using Dragon.Security.Hmac.Module.Services.Validators;

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
            var useHexEncoding = settings.UseHexEncoding;

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            var hmacHttpService = new HmacHttpService(settings.Paths, signatureParameterKey)
            {
                Validators =
                    CreateValidatorMap(serviceId, signatureParameterKey,
                        new DapperAppRepository(_connection, applicationTableName),
                        new DapperUserRepository(_connection, userTableName),
                        new HmacSha256Service
                        {
                            SignatureParameterKey = signatureParameterKey,
                            UseHexEncoding = useHexEncoding
                        }),
                 StatusCodes = CreateStatusCodeMap(signatureParameterKey)
            };
            return hmacHttpService;
        }

        public Dictionary<string, IValidator> CreateValidatorMap(string serviceId, string signatureParameterKey, IAppRepository appRepository,
            IUserRepository userRepository, IHmacService hmacService)
        {
            var expiryValidator = new ExpiryValidator();
            var serviceValidator = new ServiceValidator(serviceId);
            var appValidator = new AppValidator
            {
                AppRepository = appRepository,
                ServiceValidator = serviceValidator
            };
            var hmacValidator = new HmacValidator
            {
                AppRepository = appRepository,
                AppValidator = appValidator,
                ServiceValidator = serviceValidator,
                HmacService = hmacService
            };
            var userValidator = new UserValidator
            {
                UserRepository = userRepository,
                AppValidator = appValidator,
                ServiceValidator = serviceValidator
            };

            return new Dictionary<string, IValidator>
            {
                {"expiry", expiryValidator},
                {"appid", appValidator},
                {"serviceid", serviceValidator},
                {"userid", userValidator},
                {signatureParameterKey, hmacValidator}
            };
        }

        public Dictionary<string, StatusCode> CreateStatusCodeMap(string signatureParameterKey) {
            return new Dictionary<string, StatusCode>
            {
                {"expiry", StatusCode.InvalidExpiryOrExpired},
                {"appid", StatusCode.InvalidOrDisabledAppId},
                {"serviceid", StatusCode.InvalidOrDisabledServiceId},
                {"userid", StatusCode.InvalidOrDisabledUserId},
                {signatureParameterKey, StatusCode.InvalidSignature}
            };
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}