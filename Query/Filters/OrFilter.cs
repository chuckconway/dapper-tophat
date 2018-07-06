using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public class OrFilter : Filter, IFilter
    {
        /// <summary> Constructor. </summary>
        /// <param name="query"> The query. </param>
        public OrFilter(QueryBuilder query) : base(query) { }

        public OrFilter(string sql) : base(sql) { }

        public string Operator { get; } = "OR";

        /// <summary> Gets the query. </summary>
        /// <returns> . </returns>
        public QueryBuilder Query()
        {
            return _builder;
        }
    }
}
