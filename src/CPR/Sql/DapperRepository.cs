using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using Dragon.CPR.Interfaces;
using Dapper;
using System.Diagnostics;
using System.Threading;
using Dragon.CPR.Sql.Grid;

namespace Dragon.CPR.Sql
{
    public class DapperRepository : IReadModelRepository
    {
        private readonly object m_lockConnection = new object();
        private readonly object m_lockTableExistance = new object();
        private List<Type> m_existingTables;
        private ISqlConnectionFactory m_sqlFactory;

        public DapperRepository(ISqlConnectionFactory sqlFactory)
        {
            m_sqlFactory = sqlFactory;
            m_existingTables = new List<Type>();
        }

        public SqlConnection Connection
        {
            get
            {
                var connection = m_sqlFactory.New(this.GetType());
                connection.Open();
                return connection;
            }
        }

        public void EnsureTableExists<T>() where T : class
        {
            if (m_existingTables.Contains(typeof(T))) return;

            lock (m_lockTableExistance)
            {
                using (var c = Connection)
                {
                    c.CreateTableIfNotExists<T>();
                    m_existingTables.Add(typeof(T));
                }
            }

        }

        public void DropTableIfExists<T>() where T : class
        {
            lock (m_lockTableExistance)
            {
                using (var c = Connection)
                {
                    c.DropTableIfExists<T>();
                    m_existingTables.Remove(typeof(T));
                }
            }
        }

        public void DropTableIfExists(string name)
        {
            lock (m_lockTableExistance)
            {
                using (var c = Connection)
                {
                    c.DropTableIfExists(name);
                }
            }
        }


        public T Get<T>(Guid id) where T : class
        {
            using (var c = Connection)
            {
                return c.Get<T>(id);
            }
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            using (var c = Connection)
            {
                return c.GetAll<T>();
            }
        }

        public IEnumerable<dynamic> Query(string sql, dynamic param = null)
        {
            using (var c = Connection)
            {
                return c.Query(sql, (object)param);
            }
        }

        public IEnumerable<T> Query<T>(string sql, dynamic param = null) where T : class
        {
            using (var c = Connection)
            {
                return c.Query<T>(PreprocessSQLString<T>(sql), (object)param);
            }
        }

        public TReturn ExecuteScalar<TReturn, TDBObject>(string sql, dynamic param = null) 
        {
            using (var c = Connection)
            {
                return c.Query<TReturn>(PreprocessSQLString<TDBObject>(sql), (object)param).First();
            }
        }

        public IEnumerable<TResult> Query<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> mapping, dynamic param = null) where TResult : class
        {
            using (var c = Connection)
            {
                return c.Query<TFirst, TSecond, TResult>(PreprocessSQLString<TResult>(sql), mapping, (object)param);
            }
        }

        public int Execute(string sql, dynamic param = null)
        {
            using (var c = Connection)
            {
                return c.Execute(sql, (object)param);
            }
        }

        public void Update<T>(T obj) where T : class
        {
            using (var c = Connection)
            {
                c.Update<T>(obj);
            }
        }

        public void Insert<T>(T obj) where T : class
        {
            using (var c = Connection)
            {
                c.Insert<T,Guid>(obj, letDbGenerateKey: false);
            }
        }

        public void Delete<T>(T entity) where T : class
        {
            using (var c = Connection)
            {
                c.Delete(entity);
            }
        }

        public void PopulateTable<T>(TableViewModel<T> model) where T : class
        {
            var tablename = typeof(T).Name;
            var itemsPerPage = model.SortingPaging.ItemsPerPage;
            var greaterThanRow = (model.SortingPaging.Page - 1) * model.SortingPaging.ItemsPerPage;

            var parameters = new DynamicParameters();

            StringBuilder sbInner = new StringBuilder();
            StringBuilder sbOuter = new StringBuilder();

            sbOuter.AppendFormat("SELECT TOP {0} (CNTRNREV + CNTRN - 1) AS ResultCount, ", itemsPerPage);

            // SELECT

            var sortPropInsecureInput = model.SortingPaging.SortProperty;
            var sortPropSafe = (string)null;

            sbInner.Append("SELECT ");
            bool first = true;
            var firstColumn = (string)null;

            if (model.Columns.Count() == 0)
                throw new InvalidOperationException("Cannot generate table for zero columns.");

            foreach (var c in model.Columns.Where(x => !string.IsNullOrEmpty(x.Column)))
            {
                if (!string.IsNullOrEmpty(sortPropInsecureInput) &&
                    c.Column.Equals(sortPropInsecureInput.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    sortPropSafe = c.Column;
                }

                if (!first)
                {
                    sbInner.Append(",");
                    sbOuter.Append(",");
                }
                else
                {
                    // first
                    firstColumn = c.Column;
                }

                sbInner.Append(string.Format("[{0}]", c.Column));
                sbOuter.Append(string.Format("[{0}]", c.Column));
                first = false;
            }

            if (string.IsNullOrEmpty(sortPropSafe))
            {
                sortPropSafe = model.Columns.First().Column;
            }

            if (!string.IsNullOrEmpty(sortPropInsecureInput) &&                 // sort property was provided
                (sortPropSafe == null ||                                          // and not found
                sortPropSafe.ToLower() != sortPropInsecureInput.ToLower()))     // or does not match what was found
            {
                throw new InvalidOperationException("The sort property specified did not match a column in the query.");
            }

            var rowIdentifer = model.Columns.FirstOrDefault(x => x.IsRowIdentifier);

            if (rowIdentifer == null) throw new InvalidOperationException("Each table needs a property with [Key] attribute.");

            // ADD Counters
            sbInner.AppendFormat(" ,ROW_NUMBER() OVER (ORDER BY [{0}] {1}) AS RN ", sortPropSafe, model.SortingPaging.SortAscending ? "ASC" : "DESC");
            sbInner.AppendFormat(" ,ROW_NUMBER() OVER (ORDER BY [{0}] {1}) AS CNTRN ", rowIdentifer.Column, model.SortingPaging.SortAscending ? "ASC" : "DESC");
            sbInner.AppendFormat(" ,ROW_NUMBER() OVER (ORDER BY [{0}] {1}) AS CNTRNREV ", rowIdentifer.Column, !model.SortingPaging.SortAscending ? "ASC" : "DESC");

            sbInner.AppendFormat(" FROM [{0}] ", tablename);

            // Filters
            var activeFilters = model.Filters.OfType<Filters.FilterViewModel<T>>().Where(x => x.FilterActive);
            if (activeFilters.Count() > 0)
            {
                var filters = new FilterConverter<T>(activeFilters);
                sbInner.Append(filters.WhereClause);
                parameters = filters.Params;
            }

            // Construct Outer
            sbOuter.AppendFormat(" FROM ( ");
            sbOuter.Append(sbInner);
            sbOuter.AppendFormat(" ) InnerQuery WHERE RN > {0} ORDER BY RN ASC", greaterThanRow);

            var sql = sbOuter.ToString();

            using (var c = Connection)
            {
                var res = c.Query<ResultCountHelper<T>, T, ResultCountHelper<T>>(sql,
                    (rc, d) => { rc.Data = d; return rc; }, parameters, splitOn: firstColumn);

                var count = res.FirstOrDefault();
                model.SortingPaging.MaxPage = 1;
                if (count != null)
                {
                    var d = Convert.ToDecimal(count.ResultCount) / Convert.ToDecimal(itemsPerPage.Value /* null handled in getter */);
                    model.SortingPaging.MaxPage = Convert.ToInt32(Math.Ceiling(d));
                }
                model.Data = res.Select(x => x.Data);
            }

            /*
             *       SELECT TOP {3} (RNREV + RN - 1) AS ResultCount, ToposID, Location FROM 
                (
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY T.CreatedAt DESC) AS RN, 
                        ROW_NUMBER() OVER (ORDER BY T.CreatedAt) AS RNREV, 
                        T.ToposID, Location
                    FROM Topos T JOIN ToposMain TM ON T.ToposID = TM.ToposID
                    WHERE Location.STIntersects(GEOGRAPHY::STGeomFromText('{0}',{1})) = 1 
                    {2}
                ) InnerQuery WHERE RN > {4} ORDER BY RN ASC";
             * 
             */
        }

        private string PreprocessSQLString<T>(string sqlIn)
        {
            return sqlIn.Replace("{TABLE}", string.Format("[" + typeof(T).Name + "]"));
        }
    }
}
