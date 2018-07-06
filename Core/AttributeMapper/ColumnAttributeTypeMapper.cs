using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dapper.TopHat.Core.AttributeMapper
{
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper(): base(new SqlMapper.ITypeMap[]
          {
                new CustomPropertyTypeMap(typeof(T), GetCustomAttributes),
                new DefaultTypeMap(typeof(T))
            })
        {}

        public static PropertyInfo GetCustomAttributes(Type type, string columnName)
        {
          return  type.GetProperties().FirstOrDefault(prop =>
                    prop.GetCustomAttributes(false).OfType<ColumnAttribute>()
                    .Any(attribute => attribute.Name == columnName));
        }
    }
}
