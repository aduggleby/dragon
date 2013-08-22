using System;
using AutoMapper;
using Dragon.CPR.Interfaces;

namespace Dragon.CPR.Impl.Projections
{
    public class CommandProjection<TCommand, TTable> : SingleProjectionBase<TCommand, TTable>,
        IUseCommand<TCommand>
        where TCommand : CommandBase
        where TTable : class
    {
        public TCommand Command { get; set; }

        protected override TCommand FetchSrc(Guid id)
        {
            if (!Command.CommandID.Equals(id))
                throw new InvalidOperationException();

            return Command;
        }
    }
}
