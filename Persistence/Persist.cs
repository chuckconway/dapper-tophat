using System.Collections.Generic;

namespace Dapper.TopHat.Persistence
{
    public class Persist
    {
        public Persist(string sql, object instance, string tableName)
        {
            Sql = sql;
            Instance = instance;
            TableName = tableName;
        }

        public Persist(SqlResult result, object instance, string tableName)
        {
            Sql = result.Sql;
            HasPrimaryKeysPresent = result.HasPrimaryKeysPresent;
            Instance = instance;
            TableName = tableName;
        }

        public string Sql { get; set; }
        
        public IList<QueryParameter> Parameters { get; set; } = new List<QueryParameter>();

        public object Instance { get; set; }
        
        public string TableName { get; set; }

        public bool HasPrimaryKeysPresent { get; set; }
    }

    public class QueryParameter
    {
        public string Name { get; set; }
        
        public object Value { get; set; }
    }
}