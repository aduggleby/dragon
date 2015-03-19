using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Antlr4.StringTemplate;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Module.Tests.Repositories
{
    public class RepositoryIntegrationTestBase
    {
        private const string TempDbName = "DragonSecurityHmacTest";
        protected IDbConnection Connection;
        private string _dbPath;
        private const string CreateDbScriptFilename = "create_db.sql.st";
        private const string DropDbScriptFilename = "drop_db.sql.st";
        private const string CreateTablesScriptFilename = "create_tables.sql.st";
        private const string SqlScriptRoot = "Scripts";

        [TestInitialize()]
        public void Initialize()
        {
            _dbPath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + TempDbName + ".mdf";
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            QueryFromFile(CreateDbScriptFilename, new Dictionary<string, string> { { "dbPath", _dbPath } });
            Connection.Close();
            Connection.Open();
            Connection.ChangeDatabase(TempDbName);
            QueryFromFile(CreateTablesScriptFilename);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Connection.Close();
            Connection.Open();
            QueryFromFile(DropDbScriptFilename, new Dictionary<string, string> { { "dbPath", TempDbName } });
            Connection.Close();
            Connection.Dispose();
        }

        # region helper

        protected void QueryFromFile(string sqlFilePath, IDictionary<string, string> parameter = null)
        {
            Connection.Query(PrepareQuery(sqlFilePath, parameter));
        }

        protected IEnumerable<T> QueryFromFile<T>(string sqlFilePath, IDictionary<string, string> parameter = null)
        {
            return Connection.Query<T>(PrepareQuery(sqlFilePath, parameter));
        }

        private static string PrepareQuery(string sqlFilePath, IDictionary<string, string> parameter)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var file =
                new FileInfo(baseDir + Path.DirectorySeparatorChar + SqlScriptRoot + Path.DirectorySeparatorChar + sqlFilePath);
            var sqlScript = file.OpenText().ReadToEnd();
            if (parameter != null)
            {
                var template = new Template(sqlScript);
                parameter.ToList().ForEach(entry => template.Add(entry.Key, entry.Value));
                sqlScript = template.Render();
            }
            return sqlScript;
        }

        # endregion
    }
}
