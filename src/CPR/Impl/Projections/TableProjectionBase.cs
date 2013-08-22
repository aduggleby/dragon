using System;
using AutoMapper;
using Dragon.CPR.Interfaces;

namespace Dragon.CPR.Impl.Projections
{
    public abstract class MappingProjectionBase<TSrc, TDest> : ProjectionBase<TSrc>
        where TSrc : class
        where TDest : class
    {
        static MappingProjectionBase()
        {
            Mapper.CreateMap<TSrc, TDest>();
        }
    }
}
