using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public abstract class Filter
    {
        protected readonly QueryBuilder _builder;

        protected Filter(QueryBuilder builder)
        {
            _builder = builder;
        }

        protected Filter(string sql)
        {
            _builder = new QueryBuilder(sql);
        }
    }
}
