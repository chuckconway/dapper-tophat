using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using Dapper.TopHat.Query.Attributes;

namespace Dapper.TopHat.Query
{
    internal class Flattener
    {

        /// <summary> Columns the names from property names. </summary>
        /// <param name="entity"> The instance. </param>
        /// <returns> . </returns>
        public IEnumerable<string> GetColumnNames(object entity)
        {
            //1. recursively discover all propery infos, expect for types such as string, and data tables
            //2. do not include the properties with the ignore attribe

            var collection = TypeDescriptor.GetProperties(entity);
            return GetNamesAndValues(entity, false, string.Empty, collection).Select(v => v.Name);
        }

        /// <summary> Gets  properties. </summary>
        /// <param name="instance"> The instance. </param>
        /// <returns> The simple properties. </returns>
        public IEnumerable<Property> GetNamesAndValues(object instance)
        {
            var collection = TypeDescriptor.GetProperties(instance);
            return GetNamesAndValues(instance, false, string.Empty, collection);
        }

        /// <summary> Gets  properties. </summary>
        /// <param name="instance"> The instance. </param>
        /// <returns> The simple properties. </returns>
        public IEnumerable<Property> GetPropertiesWithDefaultValues(object instance)
        {
            return GetNamesAndValues(instance, true, string.Empty, TypeDescriptor.GetProperties(instance));
        }

        /// <summary> Gets  properties. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="instance"> The instance. </param>
        /// <returns> The simple properties. </returns>
        public IEnumerable<Property> GetPropertiesWithDefaultValues<T>(object instance) where T: Attribute
        {
            var collection = TypeDescriptor.GetProperties(instance);
            return GetNamesAndValues(instance, true, string.Empty, new PropertyDescriptorCollection(collection
                .Cast<PropertyDescriptor>()
                .Where(property => property.GetAttribute<T>() != null)
                .ToArray()));
        }

        private string ByteArrayToHexidecimal(byte[] bytes)
        {
            return "0x" + BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary> Gets  properties. </summary>
        /// <param name="instance">                 The instance. </param>
        /// <param name="instantiateDefaultValues"> true to instantiate default values. </param>
        /// <param name="parent">                   The parent. </param>
        /// <param name="descriptors">              The infos. </param>
        /// <returns> The simple properties. </returns>
        private IEnumerable<Property> GetNamesAndValues(object instance, bool instantiateDefaultValues, string parent, PropertyDescriptorCollection descriptors)
        {
            var names = new List<Property>();

            foreach (PropertyDescriptor property in descriptors)
            {
                var valid = property.PropertyType.IsClass &&
                             property.PropertyType != typeof (string) &&
                             property.PropertyType != typeof (DataTable) &&
                             property.GetAttribute<IgnoreAttribute>() == null &&
                             property.PropertyType != typeof (byte[]);

                
                if (valid)
                {
                    var value = property.GetValue(instance);

                    if(instantiateDefaultValues)
                    {
                       value = Activator.CreateInstance(property.PropertyType);
                       property.SetValue(instance, value);
                    }

                    var lineage = parent + "." + property.Name;
                    var extractedNames = GetNamesAndValues(value, instantiateDefaultValues, lineage, TypeDescriptor.GetProperties(value));
                    names.AddRange(extractedNames);
                }
                else if (!property.PropertyType.IsClass || property.PropertyType == typeof(string) || property.PropertyType == typeof(DataTable) || property.PropertyType == typeof(byte[]))
                {
                    var value = property.GetValue(instance);

                    if (property.PropertyType == typeof(byte[]) && value != null)
                    {
                        value = ByteArrayToHexidecimal((byte[])value);
                    }

                    var name = property.GetAttribute<ColumnAttribute>()?.Name ?? property.Name;
                    names.Add(new Property(name, value, property, instance, parent));
                }
            }

            return names;
        }
    }
}
