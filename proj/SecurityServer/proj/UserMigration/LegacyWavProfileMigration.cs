using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using NLog;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace UserMigration
{
    public class LegacyWavProfileMigration<T> where T : class, IUser, new()
    {
        private const string ProfileClaimNamespace = "http://whataventure.com/schemas/identity/claims/profile/";
        private readonly string[] _properties = {"FirstName", "LastName", "Company", "Picture", "Description"};
        private readonly string _loginProvider = "Dragon";

        private readonly UserStore<T> _userStore;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LegacyWavProfileMigration(UserStore<T> userStore)
        {
            _userStore = userStore;
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
            var user = await _userStore.FindByIdAsync(userId);
            if (user == null)
            {
                await _userStore.CreateAsync(new T {Id = userId});
                user = await _userStore.FindByIdAsync(userId);
                if (user == null)
                {
                    Logger.Error("Unable to create user: " + userId);
                    return;
                }
                Logger.Trace("New user, adding login info...");
                await _userStore.AddLoginAsync(user, new UserLoginInfo(_loginProvider, userId));
            }
            IList<Claim> existingClaims = await _userStore.GetClaimsAsync(user);
            var data = (IDictionary<string, object>)userData;
            foreach (var property in _properties)
            {
                var type = ProfileClaimNamespace + property.ToLower();
                if (existingClaims.Any(x => x.Type == type))
                {
                    await _userStore.RemoveClaimAsync(user, existingClaims.First(x => x.Type == type));
                    Logger.Trace("Existing claim removed: " + type);
                }
                var value = data[property]?.ToString() ?? "";
                await _userStore.AddClaimAsync(user, new Claim(type, value));
            }
        }
    }
}
