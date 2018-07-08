using System.Data.Common;

namespace Dapper.TopHat
{
    public class ConnectionFactory<TConnectionProvider> : IConnectionFactory where TConnectionProvider : DbConnection, new()
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection CreateConnection()
        {

            // Null connection string cannot be accepted
            if (string.IsNullOrEmpty(_connectionString))
            {
                return null;
            }

            // Create the DbProviderFactory and DbConnection.
            var connection = new TConnectionProvider {ConnectionString = _connectionString};
            connection.Open();
            // Return the connection.
            return connection;
        }

    }
}
