using System.Collections.Generic;

namespace Dragon.CPR.Sql.Filters
{
    public class SearchFilterViewModel<T> : FilterViewModel<T>
    {
        public IEnumerable<string> Columns { get; set; }
        public string Term { get; set; }

        public SearchFilterViewModel(IEnumerable<string> columns, string term)
        {
            FilterActive = true;
            Columns = columns;
            Term = term;
        }

        public override void ParseFilterString(string filter)
        {
            return;
        }
    }
}
