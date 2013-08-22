

namespace Dragon.CPR.Sql.Filters
{
    public class ConstantValueFilterViewModel<T> : FilterViewModel<T>
    {
        public override bool FilterActive { get { return true; } set { } }
        public object ActivatedKey { get; set; }

        public ConstantValueFilterViewModel(string name, object value)
        {
            var c = typeof(T).GetProperty(name);
            Column = c.Name;

            ActivatedKey = value;
        }

        public override void ParseFilterString(string filter)
        {
            // NOP
        }
    }
}
