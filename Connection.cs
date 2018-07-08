using System;
using System.Data.Common;

namespace Dapper.TopHat
{
    public class Connection : IConnection
    {
        private readonly IConnectionFactory _factory;

        public Connection(IConnectionFactory factory)
        {
            _factory = factory;
        }
        
        public T Open<T>(Func<DbConnection, T> executeWithConnection)
        {
            using (var connection = _factory.CreateConnection())
            {
                return executeWithConnection(connection);
            }
        }
    }
}