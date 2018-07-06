using System.Collections.Generic;
using Dapper.TopHat.Query;

namespace Dapper.TopHat.Persistence
{
    public class PrimaryKeysNotDefined : IKeysDefined
    {
        /// <summary> Gets a value indicating whether the primary keys exist. </summary>
        /// <value> true if primary keys exist, false if not. </value>
        public bool PrimaryKeysExist => false;

        public Persist GenerateSql(string name, object instance, IList<Property> primaryKeys, IList<Property> properties)
        {
            var generator = new SqlGenerator();
            var sql = generator.Insert(name, instance, properties);
            sql.HasPrimaryKeysPresent = false;

            return sql;
        }
    }
}
