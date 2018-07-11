using System.Linq.Expressions;

namespace Dapper.TopHat.Query.Expressions
{
    public class VisitConstantExpression : ExpressionBase
    {
        public object Visit(QueryBuilder state, ConstantExpression expression)
        {
            var value = ConvertBooleanToSqlServerValues(expression.Value);
            
            return GetRightHandValue(state, i => value ?? "null");
        }

        private static object ConvertBooleanToSqlServerValues(object value)
        {
            if (value is bool v)
            {
                const int trueValue = 1;
                const int falseValue = 0;
                
                return (v ? trueValue : falseValue);
            }

            return value;
        }
    }
}
