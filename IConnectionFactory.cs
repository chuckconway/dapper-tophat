using System.Data.Common;

namespace Dapper.TopHat
{
    public interface IConnectionFactory
    {
        DbConnection CreateConnection();
    }
}