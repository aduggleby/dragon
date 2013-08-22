using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public interface IWriteRepository : IReadRepository
    {
        void Update<T>(T obj) where T : class;
        void Insert<T>(T obj) where T : class;
        int Execute(string sql, dynamic param = null);
    }
}
