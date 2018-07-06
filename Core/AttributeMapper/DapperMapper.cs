using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dapper.TopHat.Core.AttributeMapper
{
    public class DapperCustomerTypeMapper
    {
        public static void Initialize(string assemblyName)
        {
            LoadMappedTypes(assemblyName);
        }

        public static void Initialize(string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                LoadMappedTypes(assemblyName);
            }
        }

        private static void LoadMappedTypes(string assemblyName)
        {
            var types = from type in Assembly.Load(assemblyName).GetTypes()
                where type.IsClass && type.GetProperties().Any(s=>s.GetCustomAttributes<ColumnAttribute>().Any())
                select type;

            types.ToList().ForEach(type =>
            {
                var mapper = (SqlMapper.ITypeMap) Activator.CreateInstance(typeof(ColumnAttributeTypeMapper<>).MakeGenericType(type));

                SqlMapper.SetTypeMap(type, mapper);
            });
        }
    }
}
