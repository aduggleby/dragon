using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace Dapper
{
    public static class SqlMapperExtensions2
    {
        public static IEnumerable<T> QueryFor<T>(
            this IDbConnection cnn,
            string sql,
            dynamic param = null,
            IDbTransaction transaction = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return cnn.Query<T>(For<T>(sql), (object)param, transaction, buffered, commandTimeout, commandType);
        }

        //public static IEnumerable<T> Query<T>(
        //   this IDbConnection cnn,
        //   string sql,
        //   dynamic param = null,
        //   IDbTransaction transaction = null,
        //   bool buffered = true,
        //   int? commandTimeout = null,
        //   CommandType? commandType = null)
        //{
        //    return cnn.Query<T>(For<T>(sql), param, transaction, buffered, commandTimeout, commandType);
        //}

        public static int ExecuteFor<T>(
            this IDbConnection cnn, 
            string sql, 
            dynamic param = null, 
            IDbTransaction transaction = null,
            int? commandTimeout = null, 
            CommandType? commandType = null
            )
        {
            return cnn.Execute(For<T>(sql), (object)param, transaction, commandTimeout, commandType);
        }

        private static string For<T>(string sql)
        {
            return sql.Replace("{TABLE}", string.Format("[" + typeof(T).Name + "]"));
        }

    }
}
