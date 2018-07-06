using System;
using System.Linq.Expressions;
using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query
{
    public class ExpressionProcessor
    {
        /// <summary>   Processes the Expression, converting it to a SQL string.</summary>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   . </returns>
        public QueryBuilder Process<T>(Expression<Func<T, bool>> expression)
        {
            var expressionVisitor = new WhereExpressionVisitor();
            var filter = expressionVisitor.Visit(expression, new QueryBuilder());
            return filter;
        }
    }
}
