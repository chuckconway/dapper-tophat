using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.TopHat.Query;

namespace Dapper.TopHat.Persistence
{
    public interface IPersistence
    {
        /// <summary> Persists the passed in object. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item"> The item. </param>
        /// <returns> . </returns>
        List<Persist> Persist<T>(object item) where T : class, new();

        /// <summary> Persists. </summary>
        /// <param name="item">  The item. </param>
        /// <param name="table"> The table. </param>
        /// <returns> . </returns>
        List<Persist> Persist(object item, string table);

        /// <summary> Persists the passed in object. </summary>
        /// <param name="collection"> The item. </param>
        /// <returns> . </returns>
        List<Persist> Persist(ICollection collection);

        /// <summary>
        /// Persists the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>List&lt;Persist&gt;.</returns>
        List<Persist> Persist(ICollection collection, string tableName);

        /// <summary>
        /// Persists the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="tablename">The tablename.</param>
        /// <param name="where">The where.</param>
        /// <returns>List&lt;Persist&gt;.</returns>
        List<Persist> Persist(object item, string tablename, string @where);

        /// <summary> Persists. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item">   The item. </param>
        /// <param name="where"> The where. </param>
        /// <returns> . </returns>
        List<Persist> Persist<T>(object item, Expression<Func<T, bool>> @where) where T : class, new();

        List<Persist> Persist<T>(object item, string tablename, Expression<Func<T, bool>> @where) where T : class, new();

        /// <summary> Persists. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="collection"> The collection. </param>
        /// <param name="where">     The where. </param>
        /// <returns> . </returns>
        List<Persist> Persist<T>(ICollection collection, Expression<Func<T, bool>> @where) where T : class, new();
    }

    public class Persistence : IPersistence
    {
        /// <summary> Persists the passed in object. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item"> The item. </param>
        /// <returns> . </returns>
        public List<Persist> Persist<T>(object item) where T : class, new()
        {
            var name = GetTableName<T>();
            var sql = GenerateSql(item, name);
            // var persist = new Persist(sql, item, name);

            return new List<Persist>{sql};
        }

        private static string GetTableName<T>() where T : class, new()
        {
            var instance = new T();
            return GetTableNameByType(instance);
        }

        private static string GetTableNameByType(object instance)
        {
            var attribute = instance.GetType().GetCustomAttribute<TableAttribute>(true);

            if (attribute != null)
            {
                var name = string.Empty;

                if (!string.IsNullOrEmpty(attribute.Schema))
                {
                    name = $"{attribute.Schema}.";
                }

                if (string.IsNullOrEmpty(attribute.Name))
                {
                    throw new Exception("Found a Table attribute without a valid name");
                }

                return name + attribute.Name;
            }

            return instance.GetType().Name;
        }

        /// <summary> Persists. </summary>
        /// <param name="item">  The item. </param>
        /// <param name="table"> The name of the table. </param>
        /// <returns> . </returns>
        public List<Persist> Persist(object item, string table)
        {
            var sql = GenerateSql(item, table);
            //var persist = new Persist(sql, item, table);

             return new List<Persist> { sql };
        }

        /// <summary> Persists. </summary>
        /// <param name="collection"> The collection. </param>
        /// <returns> . </returns>
        public List<Persist> Persist(ICollection collection)
        {
            return (from object o in collection
                    let name = GetTableNameByType(o)
                    select GenerateSql(o, name)).ToList();
        }

        /// <summary> Persists. </summary>
        /// <param name="collection"> The collection. </param>
        /// <param name="tableName"> The table Name </param>
        /// <returns> . </returns>
        public List<Persist> Persist(ICollection collection, string tableName)
        {
            return (from object o in collection 
                    let name = tableName 
                    select GenerateSql(o, name) ).ToList();
        }

        /// <summary> Generates a sql. </summary>
        /// <param name="item"> The item. </param>
        /// <param name="name"> The name. </param>
        /// <returns> The sql. </returns>
        private static Persist GenerateSql(object item, string name)
        {
            IKeysDefined[] keysDefineds = {new HasPrimaryKeys(), new PrimaryKeysNotDefined()};

            var flattener = new Flattener();
            var primaryKeys = flattener.GetPropertiesWithDefaultValues<KeyAttribute>(item).ToList();
            var properties = flattener.GetNamesAndValues(item).ToList();
            var nameTable = name ?? GetTableNameByType(item);

            var primaryKeysExists = primaryKeys.Any();
            var first = keysDefineds.First(k => k.PrimaryKeysExist == primaryKeysExists);

            var sql = first.GenerateSql(nameTable, item, primaryKeys, properties);
            return sql;
        }

        /// <summary> Persists. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item">   The item. </param>
        /// <param name="where"> The where. </param>
        /// <returns> . </returns>
        public List<Persist> Persist<T>(object item, Expression<Func<T, bool>> @where) where T : class, new()
        {
            var name = GetTableName<T>();
            var sql = GenerateUpdate(item, name, @where);
            return new List<Persist> { sql };
        }

        /// <summary> Persists. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item">   The item. </param>
        /// <param name="where"> The where. </param>
        /// <returns> . </returns>
        public List<Persist> Persist<T>(object item, string tablename, Expression<Func<T, bool>> @where) where T : class, new()
        {
            var sql = GenerateUpdate(item, tablename, @where);
            return new List<Persist> { sql };
        }

        public List<Persist> Persist(object item, string tablename, string @where)
        {
            var sql = GenerateUpdate(item, tablename, @where);
            return new List<Persist> { sql };
        }

        /// <summary> Persists. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="collection"> The collection. </param>
        /// <param name="where">     The where. </param>
        /// <returns> . </returns>
        public List<Persist> Persist<T>(ICollection collection, Expression<Func<T, bool>> @where) where T : class, new()
        {
            return (from object o in collection
                    let name = GetTableNameByType(o)
                    select GenerateUpdate(o, name, @where)).ToList();
        }

        /// <summary> Generates an update. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item">   The item. </param>
        /// <param name="name">
        /// (optional) the name. This overrides the table name. Which is gotten from the class name. </param>
        /// <param name="where"> The where. </param>
        /// <returns> The update&lt; t&gt; </returns>
        private Persist GenerateUpdate<T>(object item, string name, Expression<Func<T, bool>> @where)
        {
            var generator = new SqlGenerator();
            var processor = new ExpressionProcessor();

            var sql = generator.Update(item, name);
            sql.Sql += " WHERE " + processor.Process(@where);

            return sql;
        }

        /// <summary> Generates an update. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="item">   The item. </param>
        /// <param name="name">
        /// (optional) the name. This overrides the table name. Which is gotten from the class name. </param>
        /// <param name="where"> The where. </param>
        /// <returns> The update&lt; t&gt; </returns>
        private Persist GenerateUpdate(object item, string name, string @where)
        {
            var generator = new SqlGenerator();

            var sql = generator.Update(item, name);
            sql.Sql += " WHERE " + @where;

            return sql;
        }
    }
}
