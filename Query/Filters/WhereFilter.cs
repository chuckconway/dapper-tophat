using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public class WhereFilter : Filter, IFilter
    {
        public WhereFilter(QueryBuilder query) : base(query) { }

        public WhereFilter(string sql) : base(sql) { }

        public string Operator { get; } = "WHERE";

        /// <summary> Gets the query. </summary>
        /// <returns> . </returns>
        public QueryBuilder Query()
        {
            return _builder;
        }
    }
}
