using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Dapper.TopHat.Query.Expressions;
using Dapper.TopHat.Query.Filters;

namespace Dapper.TopHat.Query
{
    public class QueryWriter : IQueryWriter
    {
        private List<IFilter> _filters = new List<IFilter>();
        private List<Order> _orderBys = new List<Order>();  

        /// <summary> Adds a filter. </summary>
        /// <exception cref="NotImplementedException">
        /// Thrown when the requested operation is unimplemented. </exception>
        /// <param name="filter"> A filter specifying the. </param>
        public void AddFilter(IFilter filter)
        {
            _filters.Add(filter);
        }

        /// <summary> Adds an order by. </summary>
        /// <param name="column">    The column. </param>
        /// <param name="direction"> The direction. </param>
        public void AddOrder(string column, string direction)
        {
            _orderBys.Add(new Order(column, direction));
        }

        /// <summary> Query if this object contains filter type of where. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        public bool ContainsFilterTypeOfWhere()
        {
            return _filters.Any(k=> k.GetType() == typeof(WhereFilter));
        }

        /// <summary> Gets the where. </summary>
        /// <returns> . </returns>
        private QueryBuilder Where()
        {
            var builder = RenderFilter();
            var orderBy = RenderOrderBy();

            builder.Builder.Append(orderBy);

            return builder;
        }

        /// <summary> Renders the order by. </summary>
        /// <returns> . </returns>
        private string RenderOrderBy()
        {
            var builder = new StringBuilder();

            if(_orderBys.Count > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append("Order By ");

                var order = _orderBys.Select(o => o.Column + " " + o.Direction).ToArray();

                builder.Append(string.Join(", ", order));
            }

            return builder.ToString();
        }

        /// <summary> Renders the filter. </summary>
        /// <returns> . </returns>
        private QueryBuilder RenderFilter()
        {
            var builder = new QueryBuilder();

            foreach (var filter in _filters)
            {
                var name = filter.GetType().Name.Replace("Filter", string.Empty);
                var q = filter.Query();
                var val = $"{name} {q.Builder} ";
                builder.Builder.Append(val);
                builder.Parameters.AddRange(filter.Query().Parameters);
            }

            return builder;
        }

        /// <summary> Gets the columns. </summary>
        /// <param name="instance"> The instance. </param>
        /// <returns> The columns. </returns>
        public string[] GetColumns(object instance)
        {
            var flattener = new Flattener();
            return flattener.GetColumnNames(instance).ToArray();
        }

        /// <summary> Gets the select. </summary>
        /// <returns> . </returns>
        public QueryBuilder Query<T>() where T : class, new()
        {
            var instance = new T();
            var tableName = GetTableName(instance);
            var columns = GetColumns(new T());

            columns = columns.Select(s => $"[{s}]").ToArray();

            var columnList = string.Join(", ", columns);
            var filter = Where();
            var sql = $"SELECT {columnList} FROM {tableName} ";

            filter.Builder.Insert(0, sql);

            return filter;
        }

        private string GetTableName<T>(T instance)
        {
            var attributes = instance.GetType().GetCustomAttributes(typeof(TableAttribute), true);

            if (attributes.Any())
            {
                var tableAttribute = attributes.First() as TableAttribute;
                return tableAttribute.Name;
            }

            return $"[{instance.GetType().Name}]";
        }

       
        private class Order
        {
            public Order(string column, string direction)
            {
                Column = column;
                Direction = direction;
            }

            /// <summary> Gets or sets the column. </summary>
            /// <value> The column. </value>
            public string Column { get; private set; }

            /// <summary> Gets or sets the direction. </summary>
            /// <value> The direction. </value>
            public string Direction { get; private set; }
        }

        /// <summary> Dispose of this object, cleaning up any resources it uses. </summary>
        public void Dispose()
        {
            _filters = new List<IFilter>();
            _orderBys = new List<Order>();
        }
    }
}
