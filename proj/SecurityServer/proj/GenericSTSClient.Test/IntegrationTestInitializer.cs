using System;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.GenericSTSClient.Test.Models;
using Dragon.SecurityServer.GenericSTSClient.Test.Properties;

namespace Dragon.SecurityServer.GenericSTSClient.Test
{
    public class IntegrationTestInitializer
    {
        public static Action<SqlConnection> DatabaseCallback { get; set; }
        public static Action<Guid, Guid, Guid, string, string> TestDataCallback { get; set; }

        public static void CreateTestDatabase(Guid serviceId, Guid userId, Guid appId, string testDbName, string secret, string targetDbPath)
        {
            var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
            const string connectionStringInitial = @"Data Source=(LocalDB)\v11.0;Initial Catalog=Master;Integrated Security=True";
            var mdfFile = new FileInfo(path + testDbName + ".mdf");
            var ldfFile = new FileInfo(path + testDbName + "_log.ldf");
            using (var conn = new SqlConnection(connectionStringInitial))
            {
                conn.Open();
                DetachDb(testDbName, conn);
                RecreateTestDb(testDbName, conn, mdfFile, ldfFile, path);
            }
            var connectionString = string.Format(@"Data Source=(LocalDB)\v11.0;AttachDbFilename={0}{1}.mdf;Initial Catalog={1};Integrated Security=True", path, testDbName);
            ConnectionHelper.DefaultConnectionString = () => connectionString;
            ConnectionHelper.ConnectionFactory = s => new SqlConnection(connectionString);
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                CreateTables(conn);
                CreateExceptionsTable(conn);
                if (DatabaseCallback != null) DatabaseCallback(conn);
                InsertTestData(serviceId, userId, appId, testDbName, secret);
                if (TestDataCallback != null) TestDataCallback(serviceId, userId, appId, testDbName, secret);
                DetachDb(testDbName, conn);
            }
            PublishTestDb(mdfFile, ldfFile, targetDbPath);
        }

        private static void RecreateTestDb(string testDbName, SqlConnection conn, FileSystemInfo mdfFile, FileSystemInfo ldfFile,
            string path)
        {
            new SqlCommand(string.Format(
                "IF EXISTS(SELECT name FROM sys.databases WHERE name = '{0}') DROP DATABASE {0}", testDbName), conn)
                .ExecuteNonQuery();
            if (mdfFile.Exists) mdfFile.Delete();
            if (ldfFile.Exists) ldfFile.Delete();
            new SqlCommand(string.Format("CREATE DATABASE {1} ON PRIMARY (Name={1}, filename = '{0}{1}.mdf')",
                path, testDbName), conn).ExecuteNonQuery();
        }

        private static void PublishTestDb(FileInfo mdfFile, FileInfo ldfFile, string targetDbPath)
        {
            var targetMdfFile =
                new FileInfo(AppDomain.CurrentDomain.BaseDirectory + targetDbPath);
            var targetLdfFile =
                new FileInfo(AppDomain.CurrentDomain.BaseDirectory + targetDbPath.Replace(".mdf", "_log.ldf"));
            if (targetMdfFile.Exists) targetMdfFile.Delete();
            if (targetLdfFile.Exists) targetLdfFile.Delete();
            mdfFile.MoveTo(targetMdfFile.ToString());
            ldfFile.MoveTo(targetLdfFile.ToString());
        }

        private static void CreateTables(SqlConnection conn)
        {
            conn.CreateTableIfNotExists<AppModel>();
            conn.CreateTableIfNotExists<UserModel>();
        }

        private static void CreateExceptionsTable(SqlConnection conn)
        {
            var sql = Resources.CreateExceptionsTable;
            new SqlCommand(sql, conn).ExecuteNonQuery();
        }

        private static void InsertTestData(Guid serviceId, Guid userId, Guid appId, string dbName, string secret)
        {
            var hmacUserRepository = new Repository<UserModel>();
            hmacUserRepository.Insert(new UserModel { Id = 1, ServiceId = serviceId, UserId = userId, AppId = appId, Enabled = true, CreatedAt = DateTime.UtcNow });

            var hmacAppRepository = new Repository<AppModel>();
            hmacAppRepository.Insert(new AppModel { Id = 1, ServiceId = serviceId, AppId = appId, CreatedAt = DateTime.UtcNow, Enabled = true, Name = dbName, Secret = secret });
        }

        private static void DetachDb(string dbName, SqlConnection conn)
        {
            new SqlCommand(string.Format(
                "IF EXISTS(SELECT name FROM sys.databases WHERE name = '{0}') ALTER DATABASE {0} SET OFFLINE WITH ROLLBACK IMMEDIATE; " +
                "IF EXISTS(SELECT name FROM sys.databases WHERE name = '{0}') EXEC SP_DETACH_DB {0}; ", dbName), conn)
                .ExecuteNonQuery();
        }
    }
}
