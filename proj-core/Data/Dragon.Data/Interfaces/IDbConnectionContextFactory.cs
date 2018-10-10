using System;
using System.Data.Common;

namespace Dragon.Data.Interfaces
{
    public interface IDbConnectionContextFactory
    {
        void InDatabase(Action<DbConnection, DbTransaction> db);
        T InDatabase<T>(Func<DbConnection, DbTransaction, T> db);

    }
}