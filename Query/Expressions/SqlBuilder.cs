using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.TopHat.Persistence;

namespace Dapper.TopHat.Query.Expressions
{
    public class SqlBuilder
    {
        public Persist BuildUpdateQuery(string name, object instance, IEnumerable<Property> values)
        {
            var sql = $"UPDATE {name} SET ";

            var parameters = BuildQuery(values);
            sql += string.Join(", ", parameters.Select(s=>$"[{s.Name}] = @{s.Name}").ToArray());

            return new Persist(sql, instance, name)
            {
                Parameters = parameters
            };
        }

        public Persist BuildInsertQuery(string tableName, object instance, IEnumerable<Property> values)
        {
            var filtered = values.Where(s => ShowColumn<int>(s.PropertyDescriptor.PropertyType, s.Value))
                .Where(s => ShowColumn<Guid>(s.PropertyDescriptor.PropertyType, s.Value))
                .Where(s => s.Value != null).ToList();
            
            var parameters = BuildQuery(filtered);

              var queryColumns = filtered.Select(p => $"[{p.Name}]").ToArray();
              var queryValues = filtered.Select(p => $"@{p.Name}").ToArray();

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", queryColumns)}) VALUES ({string.Join(", ", queryValues)})";
            
            return new Persist(sql, instance, tableName)
            {
                Parameters = parameters
            };
        }

        private static IList<QueryParameter> BuildQuery(IEnumerable<Property> values)
        {
            IList<QueryParameter> keyValues = new List<QueryParameter>();
           
            foreach (var v in values)
            {
                //If the value is null, use DBNull
                if (v.Value == null)
                {
                    keyValues.Add(new QueryParameter()
                    {
                        Name = v.Name,
                        Value = null
                    });
                }
                else
                {
                    keyValues.Add(new QueryParameter()
                    {
                        Name = v.Name,
                        Value = v.Value
                    });
                }
            }

            return keyValues;
        }

        private static bool ShowColumn<T>(Type type, object value)
        {
            var showColumn = true;

            if (typeof(T) == type)
            {
                showColumn = Convert.ToString(value) != Convert.ToString(default(T));
            }

            return showColumn;
        }
    }
}