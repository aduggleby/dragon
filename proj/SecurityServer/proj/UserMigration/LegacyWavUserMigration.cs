using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace UserMigration
{
    public class LegacyWavUserMigration<T> where T : class, IUser, new()
    {
        private readonly IDragonUserStore<T> _userStore;

        public LegacyWavUserMigration(IDragonUserStore<T> userStore)
        {
            _userStore = userStore;
        }

        public async Task Migrate(Func<dynamic, T> convertUser)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WAV"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var usersData = connection.Query("SELECT u.UserID, Service, Email, [Key], Secret FROM [DragonRegistration] dr JOIN [User] u ON dr.UserID = u.UserID").ToList();
                foreach (var userData in usersData)
                {
                    var userId = userData.UserID.ToString();
                    var service = userData.Service;
                    var isLocalAccount = service == "LOCALACCOUNT";
                    userData.Email = !isLocalAccount ? userData.Email : userData.Key;
                    var user = convertUser(userData);

                    var store = (UserStore<AppMember>)_userStore; // avoid RuntimeBinderException: does not contain a definition, TODO: remove
                    await store.CreateAsync(user);
                    user = await store.FindByIdAsync(userId);

                    if (!isLocalAccount)
                    {
                        var key = userData.Key;
                        var externalServiceName = service.Replace("EXTERNAL_", "");
                        externalServiceName = externalServiceName.First().ToString().ToUpper() + externalServiceName.Substring(1).ToLower();
                        store.AddLoginAsync(user, new UserLoginInfo(externalServiceName, key));
                    }
                }
            }
        }
    }
}
