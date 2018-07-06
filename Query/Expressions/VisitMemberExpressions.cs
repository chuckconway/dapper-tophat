using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.TopHat.Query.Expressions
{
    internal class VisitMemberExpressions : ExpressionBase
    {
        public object Visit(QueryBuilder state, MemberExpression expression)
        {
            var val = GetRightHandValue(state, i => (i != '=' ? GetMemberName(expression.Member) : GetValue(expression)));
            return val;
        }

        public string GetMemberName(MemberInfo memberInfo)
        {
            var name = memberInfo.Name;

            if (memberInfo.CustomAttributes.Any())
            {
                var columnAttribute = memberInfo.GetCustomAttributes(typeof(ColumnAttribute), true); //.CustomAttributes.OfType<ColumnAttribute>(); //.SingleOrDefault(s => s.AttributeType == typeof(ColumnAttribute));

                if (columnAttribute.Any())
                {
                    var attribute = columnAttribute.Single() as ColumnAttribute;
                    name = attribute?.Name ?? memberInfo.Name;
                }
            }

            return name;
        }

        private static object GetValue(MemberExpression node)
        {
            object val = string.Empty;

            if (node.Member is PropertyInfo)
            {
                val = GetPropertyInfoValue(node);
            }

            if (node.Member is FieldInfo)
            {
                val = GetFieldInfoValue(node);
            }

            return val;
        }

        /// <summary> Gets a property information value. </summary>
        /// <param name="node"> The node. </param>
        /// <returns> The property information value. </returns>
        private static object GetPropertyInfoValue(Expression node)
        {
            var valueExpression = Expression.Lambda(node).Compile();
            var value = valueExpression.DynamicInvoke();

            return value;
        }

        /// <summary> Gets a field information value. </summary>
        /// <param name="node"> The node. </param>
        /// <returns> The field information value. </returns>
        private static object GetFieldInfoValue(MemberExpression node)
        {
            var container = ((ConstantExpression)node.Expression).Value;
            var value = ((FieldInfo)node.Member).GetValue(container);

            return value;
        }
    }
}
