using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dragon.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get the record by primary key.
        /// T must have a single property marked with KeyAttribute.
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        T Get(dynamic pk);

        /// <summary>
        /// Get the record by primary key.
        /// T must have a single property marked with KeyAttribute.
        /// </summary>
        /// <param name="keyModel"></param>
        /// <returns></returns>
        T Get(T keyModel);

        /// <summary>
        /// Gets a set of records by their primary keys.
        /// T must have a single property marked with KeyAttribute.
        /// </summary>
        /// <param name="pks"></param>
        /// <returns></returns>
        IEnumerable<T> Get(IEnumerable<object> pks);
        
        /// <summary>
        /// Gets all records from the table.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Executes an abitary parametrized SQL query ({TABLE} token will be replaced by the repositories table) and returns the results set.
        /// </summary>
        /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
        /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
        /// <returns></returns>
        IEnumerable<T> Query(string sql, dynamic param = null);

        [Obsolete("You don't need the generic type parameter")]
        IEnumerable<T> Query<TObsolete>(string sql, dynamic param = null) where TObsolete : class;

        /// <summary>
        /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the repositories table) and returns the first record value. Used for scalar queries such as "SELECT COUNT(1) FROM {TABLE}".
        /// </summary>
        /// <typeparam name="TReturn">The data type to convert the scalar return value to.</typeparam>        
        /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
        /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
        /// <returns></returns>
        TReturn ExecuteScalar<TReturn>(string sql, dynamic param = null);
        
        /// <summary>
        /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the generic type TDBObject) and returns the first record value. Used for scalar queries such as "SELECT COUNT(1) FROM {TABLE}".
        /// </summary>
        /// <typeparam name="TReturn">The data type to convert the scalar return value to.</typeparam>
        /// <typeparam name="TDBObject">The data type used to transform the query (in case you are not running a query against the repository generic type T.</typeparam>
        /// <param name="sql">A parametrized SQL Query (i.e. SELECT {cols} FROM {TABLE} WHERE COL1 = @col1value)</param>
        /// <param name="param">The parameter values (i.e. new {col1value="abc"})</param>
        /// <returns></returns>
        TReturn ExecuteScalar<TReturn, TDBObject>(string sql, dynamic param = null);
        
        /// <summary>
        /// Executes an arbitary parametrized SQL command without a result ({TABLE} token will be replaced by the repositories table).
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        void Execute(string sql, dynamic param = null);

        void ExecuteSP(string sql, dynamic param = null);

        /// <summary>
        /// Executes an arbitary parametrized SQL query ({TABLE} token will be replaced by the generic type TDBObject)
        /// and splits the returned columns into two objects (split occurs at the primary key of the second table). 
        /// See "Multi Mapping" in https://code.google.com/p/dapper-dot-net/
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sql"></param>
        /// <param name="mapping">See "Multi Mapping" in https://code.google.com/p/dapper-dot-net/</param>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<TResult> Query<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> mapping,
            dynamic param = null) where TResult : class;
      
        /// <summary>
        /// Inserts a record and expects either database to create key (auto incrementing keys) or a key value to be supplied (e.g. for GUID keys).
        /// Will return the primary key.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key used.</typeparam>
        /// <param name="obj">The record to insert.</param>
        /// <returns></returns>
        TKey Insert<TKey>(T obj);

        /// <summary>
        /// Inserts a record and expects either database to create key (auto incrementing keys) or a key value to be supplied (e.g. for GUID keys).
        /// </summary>
        /// <param name="obj">The record to insert.</param>
        /// <returns></returns>
        void Insert(T obj);

        /// <summary>
        /// Inserts or updates a record depending on wether the key has been set. Will attempt to set the primary key before inserting (only valid for GUID type primary keys).
        /// T must have a single property marked with KeyAttribute.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        TKey Save<TKey>(T obj);

        /// <summary>
        /// Updates a record and requires keys to be set.
        /// </summary>
        /// <param name="obj">The record to update.</param>
        void Update(T obj);

        /// <summary>
        /// Deletes a record and requires keys to be set.
        /// </summary>
        /// <param name="obj">The record to update.</param>
        void Delete(T obj);

        /// <summary>
        /// Performs a query with a set of AND filters.
        /// </summary>
        /// <param name="where">The values to filter for. Each entry will be concatenated with AND.</param>
        /// <returns></returns>
        IEnumerable<T> GetByWhere(Dictionary<string, object> @where);
    }
}