using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Dragon.Data.Interfaces;
using Dragon.CPR.Sql.Filters;

namespace Dragon.CPR.Sql
{
    public class FilterConverter<T>
    {
        private int m_paramIdx = 0;
        private DynamicParameters m_parameters;
        private List<string> m_clauses;
        private IEnumerable<FilterViewModel<T>> m_filterViewModels;

        public FilterConverter(IEnumerable<FilterViewModel<T>> filterViewModels)
        {
            m_filterViewModels = filterViewModels;
            m_parameters = new DynamicParameters();
            m_clauses = new List<string>();

            Convert();
        }

        public string WhereClause
        {
            get
            {
                return " WHERE " + string.Join(" AND ", m_clauses.Select(x => string.Concat("(", x, ")")).ToArray());
            }
        }

        public DynamicParameters Params
        {
            get { return m_parameters;  }
        }
        
        private void Convert()
        {
            m_filterViewModels.OfType<ConstantValueFilterViewModel<T>>().ToList().ForEach(x => Convert(x));
            m_filterViewModels.OfType<SearchFilterViewModel<T>>().ToList().ForEach(x => Convert(x));
        }

        private void Convert(ConstantValueFilterViewModel<T> f)
        {
            m_paramIdx++;
            var param = string.Format("p{0}", m_paramIdx);
            var clause = string.Format("[{0}] = @{1}", f.Column, param);
            m_parameters.Add(param, f.ActivatedKey);
            m_clauses.Add(clause);
        }

        private void Convert(SearchFilterViewModel<T> f)
        {
            m_paramIdx++;
            var param = string.Format("p{0}", m_paramIdx);
            m_parameters.Add(param, "%"+f.Term+"%");

            var q = new List<string>(); 
            foreach (string column in f.Columns)
            { 
                q.Add(string.Format("[{0}] LIKE @{1}", column, param));
            }
            var clause = string.Format("({0})", string.Join(" OR ", q.ToArray()));
            m_clauses.Add(clause);
        }
    }
}
