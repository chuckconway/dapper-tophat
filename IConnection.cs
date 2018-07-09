using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.TopHat
{
    public interface IConnection
    {
        T Open<T>(Func<DbConnection, T> executeWithConnection);

        Task<T> OpenAsync<T>(Func<DbConnection, Task<T>> executeWithConnection);

        T OpenTransaction<T>(Func<DbConnection, DbTransaction, T> executeWithConnection);

        Task<T> OpenTransactionAsync<T>(Func<DbConnection, DbTransaction, Task<T>> executeWithConnection);
    }
}