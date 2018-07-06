using System.Collections.Generic;
using Dapper.TopHat.Query;
using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Persistence
{
    public class SqlGenerator
    {
        /// <summary> Combine to where. </summary>
        /// <param name="statement"> The statement. </param>
        /// <param name="where">     The where. </param>
        /// <returns> . </returns>
        public string CombineToWhere(string statement, string where)
        {
            return $"{statement} WHERE {@where}";
        }

        /// <summary> Updates the builder described by item. </summary>
        /// <param name="item"> The item. </param>
        /// <returns> . </returns>
        public Persist Update(object item, string name = null)
        {
            var flattener = new Flattener();
            var values = flattener.GetNamesAndValues(item);
            var tableName = name ?? item.GetType().Name;
            return Update(tableName, item, values);
        }

        /// <summary> Updates the builder described by item. </summary>
        /// <param name="name">   The name. </param>
        /// <param name="values"> The values. </param>
        /// <returns> . </returns>
        public Persist Update(string name, object instance, IEnumerable<Property> values)
        {
            var builder = new SqlBuilder();
            return builder.BuildUpdateQuery(name, instance, values);
        }

        public Persist Insert(string tableName, object instance, IEnumerable<Property> values)
        {
            var builder = new SqlBuilder();
            return builder.BuildInsertQuery(tableName, instance, values);
        }

//        /// <summary> Shows the column. </summary>
//        /// <typeparam name="T"> Generic type parameter. </typeparam>
//        /// <param name="type">  The type. </param>
//        /// <param name="value"> The value. </param>
//        /// <returns> true if it succeeds, false if it fails. </returns>
//        public bool ShowColumn<T>(Type type, object value)
//        {
//            var showColumn = true;
//
//            if (typeof(T) == type)
//            {
//                showColumn = Convert.ToString(value) != Convert.ToString(default(T));
//            }
//
//            return showColumn;
//        }
    }
}
