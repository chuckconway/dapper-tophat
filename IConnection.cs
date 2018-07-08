using System;
using System.Data.Common;

namespace Dapper.TopHat
{
    public interface IConnection
    {
        T Open<T>(Func<DbConnection, T> executeWithConnection);
    }
}