using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.TopHat
{
    public class Connection <TConnectionProvider> : IConnection where TConnectionProvider : DbConnection, new()
    {
        private readonly string _connectionString;

        private DbConnection CreateConnection()
        {
            // Null connection string cannot be accepted
            if (string.IsNullOrEmpty(_connectionString))
            {
                return null;
            }

            // Create the DbProviderFactory and DbConnection.
            var connection = new TConnectionProvider {ConnectionString = _connectionString};
            return connection;
        }

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public T Open<T>(Func<DbConnection, T> executeWithConnection)
        {
            using (var connection = CreateConnection())
            {
                return executeWithConnection(connection);
            }
        }
        
        public async Task<T> OpenAsync<T>(Func<DbConnection, Task<T>> executeWithConnection)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                return await executeWithConnection(connection);
            }
        }
        
        public T OpenTransaction<T>(Func<DbConnection, DbTransaction, T> executeWithConnection)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                
                using (var transaction = connection.BeginTransaction())
                {
                    return executeWithConnection(connection, transaction);
                }
            }
        }
        
        public async Task<T> OpenTransactionAsync<T>(Func<DbConnection, DbTransaction, Task<T>> executeWithConnection)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                
                using (var transaction = connection.BeginTransaction())
                {
                    return await executeWithConnection(connection, transaction);
                }
            }
        }
    }
}