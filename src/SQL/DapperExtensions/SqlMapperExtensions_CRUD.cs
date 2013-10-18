using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Dapper;
using Dragon.Common.Attributes.Data;
using Dragon.SQL;

namespace Dapper
{
    // Heavily adapted from: https://raw.github.com/SamSaffron/dapper-dot-net/master/Dapper.Contrib/SqlMapperExtensions.cs
     public  static partial class SqlMapperExtensions
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableMetadata> s_metadata =
            new ConcurrentDictionary<RuntimeTypeHandle, TableMetadata>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_selectAllQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_selectListQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();
         
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_selectQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_insertQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_updateQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_deleteQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> s_existsQueries =
            new ConcurrentDictionary<RuntimeTypeHandle, string>();
        
        private static TableMetadata MetadataFor(Type type)
        {
            TableMetadata metadata;
            if (s_metadata.TryGetValue(type.TypeHandle, out metadata))
            {
                return metadata;
            }

            metadata = new TableMetadata();
            MetadataHelper.MetadataForClass(type, ref metadata);
            s_metadata[type.TypeHandle] = metadata;
            return metadata;
        }

        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var parameters= new Dictionary<string, object>();

            var values = new Dictionary<string, object>();
            var metadata = MetadataFor(type);

            var keys = metadata.Properties.Where(x => x.IsPK);
            if (!keys.Any())
                throw new Exception("This only support entites with a single key property at the moment.");
            if (keys.Count() > 1)
                throw new Exception("This only support entites with a single key property at the moment.");

            values.Add(keys.First().PropertyName, (object)id);

            string sql;
            if (!s_selectQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_selectQueries[type.TypeHandle] = sql = TSQLGenerator.BuildSelect(metadata, values, ref parameters);
            }
            else
            {
                TSQLGenerator.BuildParameters(metadata, values, ref parameters);
            }

            var dynParms = new DynamicParameters(parameters);

            T obj = null;
            obj = connection.Query<T>(sql, dynParms, transaction: transaction, commandTimeout: commandTimeout).FirstOrDefault();
            return obj;
        }

        public static IEnumerable<T> GetList<T>(this IDbConnection connection, IEnumerable<dynamic> ids, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var parameters = new Dictionary<string, object>();

            var values = new Dictionary<string, object>();
            var metadata = MetadataFor(type);

            var keys = metadata.Properties.Where(x => x.IsPK);
            if (!keys.Any())
                throw new Exception("This only support entites with a single key property at the moment.");
            if (keys.Count() > 1)
                throw new Exception("This only support entites with a single key property at the moment.");

            values.Add(keys.First().PropertyName, ids.Select(x=>(object)x));

            string sql;
            if (!s_selectListQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_selectListQueries[type.TypeHandle] = sql = TSQLGenerator.BuildSelect(metadata, values, ref parameters);
            }
            else
            {
                TSQLGenerator.BuildParameters(metadata, values, ref parameters);
            }

            var dynParms = new DynamicParameters(parameters);

            IEnumerable<T> objs = connection.Query<T>(sql, dynParms, transaction: transaction, commandTimeout: commandTimeout);
            return objs;
        }

        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            string sql;
            if (!s_selectAllQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var metadata = MetadataFor(type);
                s_selectAllQueries[type.TypeHandle] = sql = TSQLGenerator.BuildSelect(metadata);
            }

            return connection.Query<T>(sql, transaction: transaction, commandTimeout: commandTimeout);
        }

        public static TKey Insert<T, TKey>(this IDbConnection connection,
            T entityToInsert,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            bool letDbGenerateKey = false) where T : class
        {
            var type = typeof(T);
            var metadata = MetadataFor(type);

            var keys = metadata.Properties.Where(x => x.IsPK);
            if (!keys.Any())
                throw new Exception("This only support entites with a single key property at the moment.");
            if (keys.Count() > 1)
                throw new Exception("This only support entites with a single key property at the moment.");

            string sql;
            if (!s_insertQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_insertQueries[type.TypeHandle] = sql = TSQLGenerator.BuildInsert(metadata, withoutKeys: letDbGenerateKey);
            }

            connection.Execute(
                sql,
                entityToInsert, 
                transaction: transaction, 
                commandTimeout: commandTimeout);

            if (letDbGenerateKey)
            {
                //NOTE: would prefer to use IDENT_CURRENT('tablename') or IDENT_SCOPE but these are not available on SQLCE
                var r = connection.Query("select @@IDENTITY id", transaction: transaction,
                                         commandTimeout: commandTimeout);
                return (TKey)r.First().id;
            }
            else
            {
                return (TKey)keys.First().PropertyInfo.GetValue(entityToInsert, null);
            }
        }

        public static void Insert<T>(this IDbConnection connection,
         T entityToInsert,
         IDbTransaction transaction = null,
         int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var metadata = MetadataFor(type);

            string sql;
            if (!s_insertQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_insertQueries[type.TypeHandle] = sql = TSQLGenerator.BuildInsert(metadata, withoutKeys: false);
            }

            connection.Execute(
                sql,
                entityToInsert,
                transaction: transaction,
                commandTimeout: commandTimeout);
        }

        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var metadata = MetadataFor(type);

            string sql;
            if (!s_updateQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_updateQueries[type.TypeHandle] = sql = TSQLGenerator.BuildUpdate(metadata);
            }

            var updated = connection.Execute(sql, entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        public static TKey UpdateOrInsert<T, TKey>(
            this IDbConnection connection, 
            T entityToUpdate, 
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            bool letDbGenerateKey = true) where T : class
        {
            var updated = connection.Update(entityToUpdate, transaction, commandTimeout);
            if (updated)
            {
                var type = typeof(T);
                var metadata = MetadataFor(type);

                var keys = metadata.Properties.Where(x => x.IsPK);
                if (!keys.Any())
                    throw new Exception("This only support entites with a single key property at the moment.");
                if (keys.Count() > 1)
                    throw new Exception("This only support entites with a single key property at the moment.");

                return (TKey)keys.First().PropertyInfo.GetValue(entityToUpdate, new object[0]);
            }
            else
            {
                return connection.Insert<T, TKey>(entityToUpdate, transaction, commandTimeout, letDbGenerateKey: letDbGenerateKey);
            }
        }

        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var metadata = MetadataFor(type);

            string sql;
            if (!s_deleteQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_deleteQueries[type.TypeHandle] = sql = TSQLGenerator.BuildDelete(metadata);
            }

            var deleted = connection.Execute(sql, entityToDelete, transaction: transaction, commandTimeout: commandTimeout);
            return deleted > 0;
        }

        public static bool Exists<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var metadata = MetadataFor(type);

            var keys = metadata.Properties.Where(x => x.IsPK);
            if (!keys.Any())
                throw new Exception("This only support entites with a single key property at the moment.");
            if (keys.Count() > 1)
                throw new Exception("This only support entites with a single key property at the moment.");

            string sql;
            if (!s_existsQueries.TryGetValue(type.TypeHandle, out sql))
            {
                s_existsQueries[type.TypeHandle] = sql = TSQLGenerator.BuildDelete(metadata);
            }

            var noOfRowsMatching = connection.Execute(
                sql, 
                entity, 
                transaction: transaction, 
                commandTimeout: commandTimeout);

            return noOfRowsMatching > 0;
        }
    }
}
