using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper.TopHat.Query.Expressions;
using Dapper.TopHat.Query.Filters;
using Dapper.TopHat.Query.Restrictions;

namespace Dapper.TopHat.Query
{
    public class Query<T> : IQuery<T> where T : class, new()
    {
        private readonly DbConnection _connection;
        private readonly IQueryWriter _queryWriter;

        public Query(DbConnection connection, IQueryWriter queryWriter)
        {
            _connection = connection;
            _queryWriter = queryWriter;


            //Clear out older queries if Hypersonic is used as a Singleton.
            _queryWriter.Dispose();
        }

        /// <summary>   Define the where for this query. This method can be called once during the lifetime of this query. </summary>
        ///
        /// <param name="where">   The expression. </param>
        ///
        /// <returns>   . </returns>
        public IQuery<T> Where(Expression<Func<T, bool>> @where)
        {
            var query = RenderExpression(where);
            _queryWriter.AddFilter(new WhereFilter(query));
            return this;
        }

        /// <summary> Likes. </summary>
        /// <param name="like"> The like. </param>
        /// <returns> . </returns>
        public IQuery<T> Like(Expression<Func<T, bool>> like)
        {
            var query = RenderExpression(like);
            _queryWriter.AddFilter(new LikeFilter(query));
            return this;
        }

        /// <summary> Likes. </summary>
        /// <param name="like"> The like. </param>
        /// <returns> . </returns>
        public IQuery<T> Like(string like)
        {
            _queryWriter.AddFilter(new LikeFilter(like));
            return this;
        }

        /// <summary> Renders the expression described by expression. </summary>
        /// <param name="expression"> The expression. </param>
        /// <returns> . </returns>
        private static QueryBuilder RenderExpression(Expression<Func<T, bool>> expression)
        {
            var processor = new ExpressionProcessor();
            var query = processor.Process(expression);

            return query;
        }

        /// <inheritdoc />
        ///  <summary>   Define the where for this query. This method can be called once during the lifetime of this query. </summary>
        ///  <param name="where">   The where. </param>
        ///  <returns>   . </returns>
        public IQuery<T> Where(string @where)
        {
            _queryWriter.AddFilter(new WhereFilter(@where));
            return this;
        }

        /// <summary>   Includes an 'AND' condition for this query. </summary>
        ///
        /// <param name="and">  The and expression. </param>
        ///
        /// <returns>   . </returns>
        public IQuery<T> And(Expression<Func<T, bool>> and)
        {
            var query = RenderExpression(and);
            _queryWriter.AddFilter(new AndFilter(query));
            return this;
        }

        /// <summary>   Includes an 'AND' condition for this query. </summary>
        ///
        /// <param name="and">  The and expression. </param>
        ///
        /// <returns>   . </returns>
        public IQuery<T> And(string and)
        {
            _queryWriter.AddFilter(new AndFilter(and));
            return this;
        }

        /// <summary>   Includes an 'OR' condition for this query.  </summary>
        ///
        /// <param name="or">   The or. </param>
        ///
        /// <returns>   . </returns>
        public IQuery<T> Or(Expression<Func<T, bool>> or)
        {
            var query = RenderExpression(or);
            _queryWriter.AddFilter(new OrFilter(query));
            return this;
        }

        /// <summary> Includes an 'OR' condition for this query. </summary>
        /// <param name="restrictions">
        /// A variable-length parameters list containing restrictions. </param>
        /// <returns> . </returns>
        public IQuery<T> Or(params IRestriction[] restrictions)
        {
            return this;
        }
        
        /// <summary>   Includes an 'OR' condition for this query.  </summary>
        ///
        /// <param name="or">   The or. </param>
        ///
        /// <returns>IOrderBy of type T</returns>
        public IQuery<T> Or(string or)
        {
            _queryWriter.AddFilter(new OrFilter(or));
            return this;
        }

        /// <summary>   Includes an orderby clause to the current query. This can be called as many times as 
        /// 			needed. Orderbys are defined in the order this method is called.</summary>
        ///
        /// <param name="field">    The field. </param>
        ///
        /// <returns>IOrderBy of type T</returns>
        public IOrderBy<T> OrderBy(Expression<Func<T, object>> field)
        {
            LambdaExpression expression = field;
            var memberExpression = expression.Body.NodeType == ExpressionType.Convert
                                                    ? ((UnaryExpression) expression.Body).Operand as MemberExpression
                                                    : expression.Body as MemberExpression;

            IOrderBy<T> orderBy = new OrderBy<T>(memberExpression.Member.Name, this, _queryWriter);
            return orderBy;
        }

        public T Single()
        {
            var query = GetInternalQuery();
            return _connection.QuerySingle<T>(query.Sql, param: query.Parameters);
        }

        public Task<T> SingleAsync()
        {
            var query = GetInternalQuery();
            return _connection.QuerySingleAsync<T>(query.Sql, param: query.Parameters);
        }

        public T SingleOrDefault()
        {
            var query = GetInternalQuery();
            return _connection.QuerySingleOrDefault<T>(query.Sql, param: query.Parameters);
        }

        public Task<T> SingleOrDefaultAsync()
        {
            var query = GetInternalQuery();
            return _connection.QuerySingleOrDefaultAsync<T>(query.Sql, param: query.Parameters);
        }

        public T First()
        {
            var query = GetInternalQuery();
            return _connection.QueryFirst<T>(query.Sql, param: query.Parameters);
        }

        public Task<T> FirstAsync()
        {
            var query = GetInternalQuery();
            return _connection.QueryFirstAsync<T>(query.Sql, param: query.Parameters);
        }

        public T FirstOrDefault()
        {
            var query = GetInternalQuery();
            return _connection.QueryFirstOrDefault<T>(query.Sql, param: query.Parameters);
        }

        public Task<T> FirstOrDefaultAsync()
        {
            var query = GetInternalQuery();
            return _connection.QueryFirstOrDefaultAsync<T>(query.Sql, param: query.Parameters);
        }

        /// <summary>   Gets the list. </summary>
        ///
        /// <returns>List of type T</returns>
        public IEnumerable<T> ToEnumerable()
        {
            var query = GetInternalQuery();
            return _connection.Query<T>(query.Sql, param: query.Parameters);
        }

        public Task<IEnumerable<T>> ToEnumerableAsync()
        {
            var query = GetInternalQuery();
            return _connection.QueryAsync<T>(query.Sql, param: query.Parameters);
        }

        private InternalQuery GetInternalQuery()
        {
            var queryBuilder = _queryWriter.Query<T>();
            var parameters = GetParameters(queryBuilder);
            var query = new InternalQuery {Sql = queryBuilder.Builder.ToString(), Parameters = parameters};
            return query;
        }

        private static DynamicParameters GetParameters(QueryBuilder builder)
        {
            if (builder.Parameters.Any())
            {
                var parmeters = new DynamicParameters();

                foreach (var p in builder.Parameters)
                {
                    parmeters.Add(p.Key, p.Value);
                }

                return parmeters;
            }

            return null;
        }

        private class InternalQuery
        {
            public string Sql { get; set; }

            public DynamicParameters Parameters { get; set; }
        }
    }
}