using AutoMapper;
using Dragon.Data.Interfaces;

namespace Dragon.CPR.Impl.Projections
{
    public class TableViewProjection<TTable, TView> : SingleProjectionBase<TTable, TView>
        where TTable : class
        where TView : class
    {

    }
}
