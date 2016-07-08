using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Dragon.SecurityServer.ProfileSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using NLog;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace UserMigration
{
    public class LegacyWavProfileMigration<T> where T : class, IUser, new()
    {
        private const string ProfileClaimNamespace = "http://whataventure.com/schemas/identity/claims/profile/";
        private readonly string[] _properties = {"FirstName", "LastName", "Company", "Picture", "Description"};

        private readonly UserStore<AppMember> _userStore;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LegacyWavProfileMigration(IDragonUserStore<T> userStore)
        {
            _userStore = (UserStore<AppMember>) userStore; // avoid RuntimeBinderException: does not contain a definition, TODO: remove
        }

        public async Task Migrate()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WAV"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var usersData = connection.Query(@"
                    SELECT UserID, " + string.Join(", ", _properties) + @"
                    FROM [User] u
                    WHERE Email like 'whataventure.test%' -- TODO: test, remove
                    ").ToList();
                Logger.Info($"Found {usersData.Count} users, migrating...");
                foreach (var userData in usersData)
                {
                    try
                    {
                        Logger.Trace("Migrating {0}...", userData.UserID);
                        await MigrateUser(userData);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                }
                Logger.Info("Done.");
            }
        }

        private async Task MigrateUser(dynamic userData)
        {
            var userId = userData.UserID.ToString();
            await _userStore.CreateAsync(new AppMember {Id = userId});
            var user = await _userStore.FindByIdAsync(userId);
            if (user == null)
            {
                Logger.Error("User not found: " + userId);
                return;
            }
            var data = (IDictionary<string, object>)userData;
            foreach (var property in _properties)
            {
                var value = data[property]?.ToString() ?? ""; // TODO: or just do not set the claim at all?
                _userStore.AddClaimAsync(user, new Claim(ProfileClaimNamespace + property.ToLower(), value));
            }
        }
    }
}
