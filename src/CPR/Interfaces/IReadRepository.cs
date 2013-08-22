using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public interface IReadRepository 
    {
        T Get<T>(Guid id) where T : class;
        IEnumerable<T> GetAll<T>() where T : class;
        IEnumerable<dynamic> Query(string sql, dynamic param = null);
        IEnumerable<T> Query<T>(string sql, dynamic param = null) where T : class;
        TReturn ExecuteScalar<TReturn, TDBObject>(string sql, dynamic param = null);
    }
}
