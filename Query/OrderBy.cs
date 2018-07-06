namespace Dapper.TopHat.Query
{
    internal class OrderBy<T> : IOrderBy<T> where T : class, new()
    {
        private readonly string _column;
        private readonly IQuery<T> _query;
        private readonly IQueryWriter _writer;

        public OrderBy(string column, IQuery<T> query, IQueryWriter writer)
        {
            _column = column;
            _query = query;
            _writer = writer;
        }

        public IQuery<T> Asc()
        {
            _writer.AddOrder(_column, "ASC");
            return _query;
        }

        public IQuery<T> Desc()
        {
            _writer.AddOrder(_column, "DESC");
            return _query;
        }
    }
}