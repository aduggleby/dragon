using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

            Trace.WriteLine(String.Format("[" + GetSetting().ServiceId + "] Request to '{1}' denied, reason: {0}", statusCode, application.Request.RawUrl));
            Trace.Flush();
            application.Context.Response.StatusCode = 401;
            application.Context.Response.SuppressContent = true;
            application.Context.Response.End();
        }

        private HmacHttpService CreateDefaultHmacService()
        {
            var settings = GetSetting();
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

            return new HmacHttpService(serviceId, settings.Paths, signatureParameterKey)
            {
                UserRepository = new DapperUserRepository(_connection, userTableName),
                AppRepository = new DapperAppRepository(_connection, applicationTableName),
                HmacService = new HmacSha256Service { SignatureParameterKey = signatureParameterKey, UseHexEncoding = useHexEncoding}
            };
        }

        private static DragonSecurityHmacSection GetSetting()
        {
            const string hmacSectionName = "dragon/security/hmac";
            var settings = (DragonSecurityHmacSection) ConfigurationManager.GetSection(hmacSectionName);
            if (settings == null || !settings.ElementInformation.IsPresent)
            {
                settings = ReadAppSettings(hmacSectionName);
            }
            return settings;
        }

        private static DragonSecurityHmacSection ReadAppSettings(string hmacSectionName)
        {
            var settingPathTypes = ConfigurationManager.AppSettings["Dragon.Security.Hmac.PathTypes"];
            var settingPathNames = ConfigurationManager.AppSettings["Dragon.Security.Hmac.PathNames"];
            var settingPathRegexes = ConfigurationManager.AppSettings["Dragon.Security.Hmac.PathRegexes"];
            var settingExcludeParameter = ConfigurationManager.AppSettings["Dragon.Security.Hmac.ExcludeParameter"];
            AssertPathSettings(settingPathTypes, settingPathNames, settingPathRegexes, hmacSectionName);

            var types = SettingToList(settingPathTypes);
            var names = SettingToList(settingPathNames);
            var regexes = SettingToList(settingPathRegexes);
            var excludeParameter = SettingToList(settingExcludeParameter);
            AssertPaths(types, names, regexes, excludeParameter);

            var serviceId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.ServiceId"];
            var connectionStringName = ConfigurationManager.AppSettings["Dragon.Security.Hmac.ConnectionStringName"];
            var usersTableName = ConfigurationManager.AppSettings["Dragon.Security.Hmac.UsersTableName"];
            var appsTableName = ConfigurationManager.AppSettings["Dragon.Security.Hmac.AppsTableName"];
            var signatureParameterKey = ConfigurationManager.AppSettings["Dragon.Security.Hmac.SignatureParameterKey"];
            var useHexEncoding = ConfigurationManager.AppSettings["Dragon.Security.Hmac.UseHexEncoding"];
            AssertGeneralSettings(serviceId, connectionStringName, hmacSectionName);

            return CreateConfigSection(serviceId, connectionStringName, usersTableName, appsTableName, signatureParameterKey, useHexEncoding,
                CreatePathCollection(types, names, regexes, excludeParameter));
        }

        private static List<string> SettingToList(string setting)
        {
            if (setting == null || !setting.Any()) return new List<string>();
            return setting.Split(';').Select(x => x.Trim()).ToList();
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertGeneralSettings(string serviceId, string connectionStringName, string hmacSectionName)
        {
            if (string.IsNullOrWhiteSpace(serviceId) || string.IsNullOrWhiteSpace(connectionStringName))
            {
                throw new HmacInvalidConfigException($"Configuration section ({hmacSectionName}) or app settings are missing.");
            }
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertPaths(List<string> types, List<string> names, List<string> regexes, List<string> excludeParameter)
        {
            if ((types.Count != names.Count || types.Count != regexes.Count) ||
                (excludeParameter.Any() && excludeParameter.Count != types.Count))
            {
                throw new HmacInvalidConfigException(
                    "Invalid app settings have been detected. For each path type, name, and regex are required settings.");
            }
        }

        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private static void AssertPathSettings(string settingPathTypes, string settingPathNames, string settingPathRegexes,
            string hmacSectionName)
        {
            if (string.IsNullOrWhiteSpace(settingPathTypes) || string.IsNullOrWhiteSpace(settingPathNames) ||
                string.IsNullOrWhiteSpace(settingPathRegexes))
            {
                throw new HmacInvalidConfigException($"Configuration section ({hmacSectionName}) or app settings are missing.");
            }
        }

        private static DragonSecurityHmacSection CreateConfigSection(string serviceId, string connectionStringName,
            string usersTableName, string appsTableName, string signatureParameterKey, string useHexEncoding, PathCollection pathCollection)
        {
            var section = new DragonSecurityHmacSection
            {
                ServiceId = serviceId,
                ConnectionStringName = connectionStringName,
                Paths = pathCollection,
            };
            if (!string.IsNullOrWhiteSpace(usersTableName))
            {
                section.UsersTableName = usersTableName;
            }
            if (!string.IsNullOrWhiteSpace(appsTableName))
            {
                section.AppsTableName = appsTableName;
            }
            if (!string.IsNullOrWhiteSpace(signatureParameterKey))
            {
                section.SignatureParameterKey = signatureParameterKey;
            }
            if (!string.IsNullOrWhiteSpace(useHexEncoding))
            {
                section.UseHexEncoding = useHexEncoding.ToLower() == "true" || useHexEncoding == "1";
            }
            return section;
        }

        private static PathCollection CreatePathCollection(List<string> types, List<string> names, List<string> regexes, List<string> excludeParameter)
        {
            var paths = new PathCollection();
            for (var i = 0; i < types.Count; ++i)
            {
                if (types[i] != "Include" && types[i] != "Exclude")
                {
                    throw new HmacInvalidConfigException(
                        "AppSetting Dragon.Security.Hmac.PathTypes is invalid, allowed values are: Include, Exclude");
                }

                var type = types[i] == "Include" ? PathConfig.PathType.Include : PathConfig.PathType.Exclude;
                paths.Add(new PathConfig
                {
                    Type = type,
                    Name = names[i],
                    Path = regexes[i],
                    ExcludeParameters = excludeParameter.Any() ? excludeParameter[i] : ""
                });
            }

            return paths;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}