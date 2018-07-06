using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public class AndFilter : Filter, IFilter
    {
        /// <summary> Constructor. </summary>
        /// <param name="query"> The query. </param>
        public AndFilter(QueryBuilder query) : base(query) { }

        public AndFilter(string sql) : base(sql) { }

        public string Operator { get; } = "AND";

        /// <summary> Gets the query. </summary>
        /// <returns> . </returns>
        public QueryBuilder Query()
        {
            return _builder;
        }
    }
}
