using System;
using System.Collections.Generic;

namespace Dragon.CPR.Interfaces
{
    public interface IReadModelRepository : IReadRepository, IWriteRepository, ISetupRepository, IDropRepository, ICommandRepository, IPopulateRepository
    {

    }
}
