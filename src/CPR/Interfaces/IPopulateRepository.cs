using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.CPR.Sql.Grid;

namespace Dragon.CPR.Interfaces
{
    public interface IPopulateRepository
    {
        void PopulateTable<T>(TableViewModel<T> model) where T : class;
    }
}
