using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dragon.CPR.Sql.Filters;

namespace Dragon.CPR.Sql.Grid
{
    public class TableViewModel<T> 
    {
        public TableViewModel()
        {
            RenderHeader = true;
            Filters = new List<FilterViewModel>();
            Columns = new List<ColumnViewModel<T>>();
            SortingPaging = new SortingPagingViewModel();

            var subject = typeof(T);
            foreach (var prop in subject.GetProperties())
            {
                Columns.Add(new ColumnViewModel<T>(prop));
            }
            IsPaginationEnabled = true;
            IsSearchingEnabled = false;
        }

        public TableViewModel(SortingPagingViewModel sortingPaging, string defaultSort, bool sortAscending = true)
            : this()
        {
            if (sortingPaging != null)
            {
                // Copy Sorting/Paging Settings from POST
                this.SortingPaging = sortingPaging;
            }

            // Initialize Defaults for Sorting/Paging 
            if (sortingPaging.SortProperty == null)
            {
                this.SortingPaging.SortAscending = this.SortingPaging.SortProperty == null ? sortAscending : this.SortingPaging.SortAscending;
                this.SortingPaging.SortProperty = this.SortingPaging.SortProperty ?? defaultSort;
            }

            if (sortingPaging.Page > sortingPaging.MaxPage) sortingPaging.Page = sortingPaging.MaxPage;
            if (sortingPaging.Page < 1) sortingPaging.Page = 1;
        }

        public object RouteValues { get; set; }
        public SortingPagingViewModel SortingPaging { get; set; }

        public List<FilterViewModel> Filters { get; set; }

        public string FilterString { get; set; }

        public bool RenderHeader { get; set; }


        public List<ColumnViewModel<T>> Columns { get; set; }
        public IEnumerable<ColumnViewModel<T>> VisibleColumns
        {
            get
            {
                return Columns.Where(x => x.Visible);
            }
        }

        public void SetSortingOrDefault(Expression<Func<T, object>> sortingProperty)
        {
                var expression = (MemberExpression)sortingProperty.Body;
                string name = expression.Member.Name;
                SortingPaging.SortProperty = SortingPaging.SortProperty ?? name;            
        }

        public ColumnViewModel<T> Column(Expression<Func<T, object>> action)
        {
            MemberExpression body = null;
            if (action.Body is MemberExpression)
            {
                body = ((MemberExpression)(action.Body));
            }
            if (action.Body is UnaryExpression)
            {
                body = ((MemberExpression)((UnaryExpression)(action.Body)).Operand);
            }

            return Columns.FirstOrDefault(x => x.PropertyName == body.Member.Name);
        }

        public ColumnViewModel<T> AddCustomColumn(string name)
        {
            var newColumn = new ColumnViewModel<T>(name);
            Columns.Add(newColumn);
            return newColumn;
        }

        public ColumnViewModel<T> AddColumn(Expression<Func<T, object>> action)
        {
            MemberExpression body = null;
            if (action.Body is MemberExpression)
            {
                body = ((MemberExpression)(action.Body));
            }
            if (action.Body is UnaryExpression)
            {
                body = ((MemberExpression)((UnaryExpression)(action.Body)).Operand);
            }

            var propType = typeof(T).GetProperty(body.Member.Name);
            
            var col = new ColumnViewModel<T>(propType);
            Columns.Add(col);
            return col;
        }


        public IEnumerable<T> Data { get; set; }

        public void ParseFilterValues(string filters)
        {
            FilterString = filters;

            if (filters == null || string.IsNullOrEmpty(filters))
            {
                return;
            }

            var typedFilters = Filters.OfType<FilterViewModel<T>>();
            typedFilters.All(x => x.FilterActive == false);

            foreach (var columnFilter in filters.Split('#'))
            {
                var keyValue = columnFilter.Split(':');
                if (keyValue.Length < 2) continue;

                var filter = typedFilters.FirstOrDefault(x => x.Column.Equals(keyValue[0], StringComparison.CurrentCultureIgnoreCase));
                if (filter == null) continue;

                filter.ParseFilterString(keyValue[1]);
                filter.FilterActive = true;
            }

        }

        public bool IsPaginationEnabled { get; set; }

        public bool IsSearchingEnabled { get; set; }
        public string SearchableColumnsString { get; set; }

        public void SetupSearch(string searchString = "")
        {
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                SortingPaging.SearchString = searchString;
            }

            var searchable = Columns.Where(c => c.Searchable);
            if (searchable.Count() > 0)
            { 
                IsSearchingEnabled = true;
                var columns = searchable.ToList().ConvertAll(c => c.PropertyName);
                SearchableColumnsString = string.Join(", ", columns.ToArray());

                string term = SortingPaging.SearchString;
                if (!string.IsNullOrWhiteSpace(term))
                {
                    var filter = new SearchFilterViewModel<T>(columns, term);
                    Filters.Add(filter);
                }
            }
        }
    }
}
