using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dapper.TopHat.Query;
using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Persistence
{
    public class HasPrimaryKeys : IKeysDefined
    {
        public bool PrimaryKeysExist => true;

        public Persist GenerateSql(string name, object instance, IList<Property> primaryKeys, IList<Property> properties)
        {
            var generator = new SqlGenerator();

            var valuesAreDefault = primaryKeys.Any(p => Convert.ToString(p.Value) == Convert.ToString(GetRuntimeDefaultValue(p.PropertyDescriptor.PropertyType)));
            primaryKeys = CreateNewGuidForGuidPrimaryKeyMarkedWithGuidGeneratorFlag(primaryKeys, properties);
            var withoutPrimaryKeys = properties.Except(primaryKeys, new CompareProperty());

            var sql = valuesAreDefault 
                        ? generator.Insert(name, instance, withoutPrimaryKeys) 
                        : GetUpdate(generator.Update(name, instance, withoutPrimaryKeys), primaryKeys);

            sql.HasPrimaryKeysPresent = !valuesAreDefault;

            return sql;
        }

        private static Persist GetUpdate(Persist persist, IList<Property> primaryKeys)
        {
            persist.Sql += " WHERE " + PrimaryKeysToSql.ConvertToSql(primaryKeys);
            return persist;
        }

        private static IList<Property> CreateNewGuidForGuidPrimaryKeyMarkedWithGuidGeneratorFlag(IList<Property> primaryKeys, IList<Property> properties )
        {
            var guids = primaryKeys.Where(p => p.PropertyDescriptor.PropertyType == typeof (Guid)).ToList();

            foreach (var property in guids)
            {
                var attribute = property.PropertyDescriptor.GetAttribute<KeyAttribute>();

                if(attribute != null)
                {
                    var prop = properties.Single(p => p.Name == property.Name);
                    var newId = Guid.NewGuid();
                    prop.Value = newId;

                    prop.PropertyDescriptor.SetValue(prop.Instance, newId);

                    primaryKeys.Remove(property);
                }
            }

            return primaryKeys;
        }

        private static object GetRuntimeDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private class CompareProperty : IEqualityComparer<Property>
        {
            public bool Equals(Property x, Property y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(Property obj)
            {
                return 0;
            }
        }
    }
}
