using Dapper.TopHat.Query.Expressions;

namespace Dapper.TopHat.Query.Filters
{
    public interface IFilter
    {
        string Operator { get; }

        /// <summary> Gets the query. </summary>
        /// <returns> string </returns>
        QueryBuilder Query();
    }
}
