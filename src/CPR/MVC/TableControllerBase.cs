using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Dapper;
using Dragon.CPR;
using Dragon.CPR.Sql;
using Dragon.CPR.Sql.Filters;
using Dragon.CPR.Sql.Grid;
using Dragon.Interfaces;
using Dragon.SQL.Repositories;

namespace Dragon.CPR.MVC
{
    public abstract class TableControllerBase< TTable> : CPRControllerBase
        where TTable : class
    {
        public IRepository<TTable> Repository { get; set; }

        static TableControllerBase()
        {
        }


        public TableControllerBase()
        {

        }

        public virtual ActionResult Index(Guid? id) 
        {
            PreIndex();
            return View();
        }

        public virtual ActionResult FXGrid(SortingPagingViewModel sortingPaging, string filterString)
        {
            var model = new TableViewModel<TTable>(sortingPaging, "Created", false);

            BuildIndex(model);
           
            PopulateTable(model);

            return PartialView("_FXGrid", model);

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
            var activeFilters = model.Filters.OfType<FilterViewModel<T>>().Where(x => x.FilterActive);
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

            using (var c = ConnectionHelper.Open())
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

      
        protected virtual void PreIndex()
        {
        }

        protected virtual void BuildIndex(TableViewModel<TTable> model)
        {
        }

       
    }
}