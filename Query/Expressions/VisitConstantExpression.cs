using System.Linq.Expressions;

namespace Dapper.TopHat.Query.Expressions
{
    public class VisitConstantExpression : ExpressionBase
    {
        public object Visit(QueryBuilder state, ConstantExpression expression)
        {
            //var values = new QuotifyValues();
            return GetRightHandValue(state, i => expression.Value ?? "null");
        }
    }
}
