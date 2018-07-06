using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public class LikeFilter : Filter, IFilter
    {
        /// <summary> Constructor. </summary>
        /// <param name="query"> The query. </param>
        public LikeFilter(QueryBuilder query) : base(query) { }

        public LikeFilter(string sql) : base(sql) { }

        public string Operator { get; } = "LIKE";

        /// <summary> Gets the query. </summary>
        /// <returns> . </returns>
        public QueryBuilder Query()
        {
            var sql = _builder.Builder.ToString();
            var query = _builder.Builder.ToString();

            if (query.Contains("="))
            {
               sql = query.Replace("=", "LIKE");
            }

            if (query.Contains("<>"))
            {
                sql = query.Replace("<>", "NOT LIKE");
            }

            return _builder.ResetSql(sql);
        }
    }
}
