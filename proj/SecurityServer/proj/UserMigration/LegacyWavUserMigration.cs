using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using NLog;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace UserMigration
{
    public class LegacyWavUserMigration<T> where T : class, IUser, new()
    {
        private readonly IDragonUserStore<T> _userStore;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _serviceId;

        public LegacyWavUserMigration(IDragonUserStore<T> userStore)
        {
            _userStore = userStore;
            _serviceId = ConfigurationManager.AppSettings["ServiceId"];
        }

        public async Task Migrate(Func<dynamic, T> convertUser)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WAV"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var usersData = connection.Query(@"
                    SELECT u.UserID, Service, Email, [Key], Secret
                    FROM [DragonRegistration] dr JOIN [User] u ON dr.UserID = u.UserID
                    WHERE Email like 'whataventure.test%' -- TODO: test, remove
                    ").ToList();
                Logger.Info($"Found {usersData.Count} users, migrating...");
                foreach (var userData in usersData)
                {
                    try
                    {
                        Logger.Trace("Migrating {0}...", userData.Email);
                        await MigrateUser(convertUser, userData);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                }
                Logger.Info("Done.");
            }
        }

        private async Task MigrateUser(Func<dynamic, T> convertUser, dynamic userData)
        {
            var userId = userData.UserID.ToString();
            var service = userData.Service;
            var isLocalAccount = service == "LOCALACCOUNT";
            userData.Email = !isLocalAccount ? userData.Email : userData.Key;
            var user = convertUser(userData);
            Logger.Trace("\"{0}\", \"{1}\"", user.Email, userData.Secret);

            var store = (UserStore<AppMember>) _userStore; // avoid RuntimeBinderException: does not contain a definition, TODO: remove
            await store.CreateAsync(user);
            await store.SetSecurityStampAsync(user, Guid.NewGuid().ToString()); // required for token verification
            if (!string.IsNullOrEmpty(_serviceId))
            {
                await store.AddServiceToUserAsync(user, _serviceId);
            }
            user = await store.FindByIdAsync(userId);

            if (!isLocalAccount)
            {
                var key = userData.Key;
                string externalServiceName = service.Replace("EXTERNAL_", "");
                externalServiceName = externalServiceName.First().ToString().ToUpper() +
                                      externalServiceName.Substring(1).ToLower();
                store.AddLoginAsync(user, new UserLoginInfo(externalServiceName, key));
                Logger.Trace("\"{0}\", \"{1}\"", externalServiceName, key);
            }
        }
    }
}
