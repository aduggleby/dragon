using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Dragon.Interfaces;
using Dragon.SQL;
using Dapper;

namespace Dragon.SQL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected static readonly ILog s_log = LogManager.GetCurrentClassLogger();

        protected static readonly bool m_hasSinglePK;

        protected static readonly TableMetadata m_metadata;
        protected static readonly PropertyMetadata m_keyProperty;
        protected static readonly PropertyMetadata[] m_keyProperties;

        static Repository()
        {
            m_metadata = new TableMetadata();
            MetadataHelper.MetadataForClass(typeof(T), ref m_metadata);

            var keys = m_metadata.Properties.Where(x => x.IsPK).ToList();
            m_hasSinglePK = keys.Count() == 1;
            if (m_hasSinglePK)
                m_keyProperty = keys.First();

            m_keyProperties = keys.ToArray();
        }

        #region Get

        public virtual IEnumerable<T> GetAll()
        {
            using (var c = OpenConnection())
            {
                s_log.Debug(x => x("Fetching all records for type {0}", typeof(T).Name));
                return c.GetAll<T>();
            }
        }

        public virtual void InsertWithCompositeKey(T obj)
        {
            using (var c = OpenConnection())
            {
                s_log.Debug(x => x("Inserted record for type {0} with composite key {1}", typeof(T).Name, KeyStringFor(obj)));
                c.Insert<T>(obj);
            }
        }

        public virtual T Get(dynamic pk)
        {
            if (!m_hasSinglePK) throw new Exception("Can only be used for objects with single primary key.");

            using (var c = OpenConnection())
            {
                s_log.Debug(x => x("Fetching record for type {0} with primary key {1}", typeof(T).Name, pk));
                return c.Get<T>((object)pk);
            }
        }

        public virtual IEnumerable<T> Get(IEnumerable<dynamic> pks)
        {
            if (!m_hasSinglePK) throw new Exception("Can only be used for objects with single primary key.");

            using (var c = OpenConnection())
            {
                s_log.Debug(x => x("Fetching records for type {0} with primary keys {1}", typeof(T).Name, string.Join(",", pks.ToArray())));
                return c.GetList<T>(pks);
            }
        }

        public virtual T Get(T keyModel)
        {
            using (var c = OpenConnection())
            {
                s_log.Debug(x => x("Fetching records for type {0} with primary keys {1}", typeof(T).Name, KeyStringFor(keyModel)));

                var where = new Dictionary<string, object>();

                foreach (var key in m_keyProperties)
                {
                    var val = key.PropertyInfo.GetValue(keyModel, null);
                    if (val == DefaultFor(val.GetType()))
                    {
                        throw new Exception("You must set all key properties of the keyModel to use this method.");
                    }

                    where.Add(key.PropertyName, key.PropertyInfo.GetValue(keyModel, null));
                }

                return GetByWhere(@where).FirstOrDefault();
            }
        }

        #endregion

        #region Read

        /// <summary>
        /// Fills a datatable using a WhereBuilder object.
        /// </summary>
        /// <param name="dt">The datatable to fill.</param>
        /// <param name="where">The WhereBuilder for querying.</param>
        public virtual void Fill(DataTable dt, Where<T> where)
        {
            using (var c = OpenConnection())
            {
                var param = new Dictionary<string, object>();

                var whereSql = where == null ? string.Empty : where.Build(param).ToString();

                var selectClause = TSQLGenerator.BuildSelect(m_metadata) + " " + whereSql;
                var cmd = c.CreateCommand();
                cmd.CommandText = selectClause;
                var sb = new StringBuilder();
                foreach (var p in param)
                {
                    sb.AppendFormat(" ^-- Parameter: {0} = '{1}'\r\n", p.Key, p.Value.ToString());
                    var dbparam = cmd.CreateParameter();
                    dbparam.ParameterName = p.Key;
                    dbparam.Value = p.Value;
                    cmd.Parameters.Add(dbparam);
                }
                s_log.Debug(x => x("Filling datatable for type {0} with '{1}'\r\n{2}", typeof(T).Name, selectClause, sb.ToString().TrimEnd()));

                var sda = CreateAdapter(cmd);
                sda.Fill(dt);
            }
        }

        private static DbDataAdapter CreateAdapter<T>(T cmd) where T : DbCommand
        {
            if (cmd is SqlCommand)
            {
                return new SqlDataAdapter();
            }

            throw new Exception("Cannot create adapter from SqlCommand!");
        }


        /// <summary>
        /// Performs a query with a set of AND filters.
        /// </summary>
        /// <param name="where">The values to filter for. Each entry will be concatenated with AND.</param>
        /// <returns></returns>
        public IEnumerable<T> GetByWhere(Dictionary<string, object> @where)
        {
            using (var c = OpenConnection())
            {
                var param = new Dictionary<string, object>();
                var sql = TSQLGenerator.BuildSelect(m_metadata, @where, ref param);

                return c.QueryFor<T>(sql, new DynamicParameters(param));
            }
        }

        #endregion

        #region Query

        public IEnumerable<T> Query(string sql, dynamic param = null)
        {
            using (var c = OpenConnection())
            {
                return c.Query<T>(PreprocessSQLString<T>(sql), (object)param);
            }
        }

        public IEnumerable<T> Query<TObsolete>(string sql, dynamic param = null) where TObsolete : class
        {
            using (var c = OpenConnection())
            {
                return c.Query<T>(PreprocessSQLString<T>(sql), (object)param);
            }
        }

        public TReturn ExecuteScalar<TReturn, TDBObject>(string sql, dynamic param = null)
        {
            using (var c = OpenConnection())
            {
                return c.Query<TReturn>(PreprocessSQLString<TDBObject>(sql), (object)param).First();
            }
        }

        public IEnumerable<TResult> Query<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> mapping, dynamic param = null) where TResult : class
        {
            using (var c = OpenConnection())
            {
                return c.Query<TFirst, TSecond, TResult>(PreprocessSQLString<TResult>(sql), mapping, (object)param);
            }
        }

        #endregion

        #region Create, Update, Delete

        /// <summary>
        /// Use for objects with a primary key. Key generated by DB is returned.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual TKey Insert<TKey>(T obj)
        {
            if (!m_hasSinglePK) throw new Exception("Can only be used for objects with single primary key. Use Insert overload instead.");

            using (var c = OpenConnection())
            {
                var pk = c.Insert<T, TKey>(obj);
                s_log.Debug(x => x("Inserted record for type {0} with key {1}", typeof(T).Name, KeyStringFor(obj)));
                return pk;
            }
        }

        /// <summary>
        /// Use for objects with composite keys. Keys must be set!
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Insert(T obj)
        {
            using (var c = OpenConnection())
            {
                c.Insert<T>(obj);
                s_log.Debug(x => x("Inserted record for type {0} with keys {1}", typeof(T).Name, KeyStringFor(obj)));
            }
        }

        public virtual TKey Save<TKey>(T obj)
        {
            if (!m_hasSinglePK) throw new Exception("Can only be used for objects with single primary key.");

            var pk = PrimaryKeyFor<TKey>(obj);
            if (pk.Equals(default(TKey)))
            {
                if (typeof (TKey) == typeof (Guid))
                {
                    SetPrimaryKey(obj, Guid.NewGuid());
                }

                return Insert<TKey>(obj);
            }
            else
            {
                Update(obj);
                return pk;
            }
        }

        public virtual void Update(T obj)
        {
            using (var c = OpenConnection())
            {
                c.Update(obj);
                s_log.Debug(x => x("Updated record for type {0} with key {1}", typeof(T).Name, KeyStringFor(obj)));
            }
        }

        public virtual void Delete(T obj)
        {
            using (var c = OpenConnection())
            {
                c.Delete(obj);
                s_log.Debug(x => x("Deleted record for type {0} with key {1}", typeof(T).Name, KeyStringFor(obj)));
            }
        }

        #endregion

        #region Execute

        public void Execute(string sql, dynamic param = null)
        {
            using (var c = OpenConnection())
            {
                c.Execute(PreprocessSQLString<T>(sql), (object)param);
            }
        }

        #endregion

        protected string PreprocessSQLString<T>(string sqlIn)
        {
            return sqlIn.Replace("{TABLE}", string.Format("[" + typeof(T).Name + "]"));
        }

        protected virtual string KeyStringFor(T obj)
        {
            return string.Join(",", m_keyProperties
                .Select(x => x.PropertyName + "=" + x.PropertyInfo.GetValue(obj, null).ToString())
                .ToArray());
        }

        protected virtual TKey PrimaryKeyFor<TKey>(T obj)
        {
            return (TKey)m_keyProperty.PropertyInfo.GetValue(obj, null);
        }

        protected virtual void SetPrimaryKey(T obj, object value)
        {
            m_keyProperty.PropertyInfo.SetValue(obj, value, null);
        }

        protected virtual DbConnection OpenConnection()
        {
            var c = ConnectionHelper.Open();
            return c;
        }

        protected object DefaultFor(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        protected TRet TryWithDeadlockRetry<TRet>(Func<TRet> a)
        {
            int maxRetries = 3;

            var success = false;
            TRet returnValue = default(TRet);
            while (maxRetries > 0 && !success)
            {
                try
                {
                    returnValue = a();
                    success = true;
                }
                catch (SqlException exception)
                {
                    if (exception.Number != 1205)
                    {
                        // a sql exception that is not a deadlock 
                        throw;
                    }

                    System.Threading.Thread.Sleep(100);

                    s_log.Debug(x => x("Deadlock encoutnered. Retryiny {0} more times.", maxRetries));
                    maxRetries--;

                    if (maxRetries == 0) throw;
                }
            }

            return returnValue;
        }


    }
}
