using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.TopHat.Query.Expressions
{
    public class PrimaryKeysToSql
    {
        /// <summary> Converts the properties to sql. </summary>
        /// <param name="instance"> The properties. </param>
        /// <returns> The given data converted to a sql. </returns>
        public QueryBuilder ConvertToSql(object instance)
        {
            var flattener = new Flattener();
            var properties = flattener.GetPropertiesWithDefaultValues<KeyAttribute>(instance);
            return ConvertToSql(properties);
        }

        /// <summary> Converts the properties to sql. </summary>
        /// <param name="properties"> The properties. </param>
        /// <returns> The given data converted to a sql. </returns>
        public static QueryBuilder ConvertToSql(IEnumerable<Property> properties)
        {
            var expression = DynamicLambdaExpressions(properties);

            var builder = new QueryBuilder();
            var whereExpressions = new WhereExpressionVisitor();
            whereExpressions.Visit(expression, builder);

            return builder;
        }

        /// <summary> Generates lambda expressions from this collection. </summary>
        /// <param name="properties"> The properties. </param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process generate lambda expressions in this
        /// collection.
        /// </returns>
        private static Expression DynamicLambdaExpressions(IEnumerable<Property> properties)
        {
            var array = properties.ToArray();

            return (from t in array
                    let param = Expression.Parameter(t.Instance.GetType(), "x")
                    let value = Expression.Constant(t.Value)
                    // let prop = Expression.PropertyOrField(param, t.Name)
                    let prop = Expression.PropertyOrField(param, t.PropertyDescriptor.Name)
                    select Expression.Equal(prop, value)).Aggregate<BinaryExpression, BinaryExpression>(null, (current, body) => (current == null ? body : Expression.And(current, body)));
        }
    }
}
