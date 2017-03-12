using System.Data.Common;

namespace Dragon.Data.Interfaces
{
    public interface IConnectionInstantiator
    {
        DbConnection Open();
        DbConnection Open<T>();
    }
}