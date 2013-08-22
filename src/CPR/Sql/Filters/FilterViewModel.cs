using Dragon.CPR.Interfaces;

namespace Dragon.CPR.Sql.Filters
{
    public class FilterViewModel
    {

    }

    
    public abstract class FilterViewModel<T> : FilterViewModel
    {
        public virtual bool FilterActive { get; set; }
        public string Column { get; set; }

        public abstract void ParseFilterString(string filter);
    }
}
